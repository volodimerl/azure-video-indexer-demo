﻿#r "Newtonsoft.Json"
#r "Microsoft.WindowsAzure.Storage"
#r "System.Web"

#load "../Shared/mediaServicesHelpers.csx"
#load "../Shared/ingestAssetConfigHelpers.csx"
#load "../Shared/jobHelpers.csx"

using System;
using System.Net;
using System.Web;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.MediaServices.Client;
using Microsoft.WindowsAzure.Storage;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

// Read values from the App.config file.
static readonly string _AADTenantDomain = Environment.GetEnvironmentVariable("AMSAADTenantDomain");
static readonly string _RESTAPIEndpoint = Environment.GetEnvironmentVariable("AMSRESTAPIEndpoint");

static readonly string _mediaservicesClientId = Environment.GetEnvironmentVariable("AMSClientId");
static readonly string _mediaservicesClientSecret = Environment.GetEnvironmentVariable("AMSClientSecret");

// Field for service context.
private static CloudMediaContext _context = null;
private static CloudStorageAccount _destinationStorageAccount = null;

public static async Task<object> Run(HttpRequestMessage req, TraceWriter log)
{
    log.Info($"Webhook was triggered!");
    string jsonContent = await req.Content.ReadAsStringAsync();
    dynamic data = JsonConvert.DeserializeObject(jsonContent);
    log.Info("Request : " + jsonContent);

    // Validate input objects
    if (data.AssetId == null)
        return req.CreateResponse(HttpStatusCode.BadRequest, new { error = "Please pass AssetId in the input object" });
    if (data.IngestAssetConfigJson == null)
        return req.CreateResponse(HttpStatusCode.BadRequest, new { error = "Please pass IngestAssetConfigJson in the input object" });
    log.Info("Input - Asset Id : " + data.AssetId);
    log.Info("Input - IngestAssetConfigJson : " + data.IngestAssetConfigJson);

    string assetid = data.AssetId;
    string ingestAssetConfigJson = data.IngestAssetConfigJson;
    IngestAssetConfig config = ParseIngestAssetConfig(ingestAssetConfigJson);
    if (!ValidateIngestAssetConfig(config))
        return req.CreateResponse(HttpStatusCode.BadRequest, new { error = "Please pass a valid IngestAssetConfig as FileContent" });
    log.Info("Input - Valid IngestAssetConfig was loaded.");


    int taskindex = 0;
    IJob job = null;
    IAsset outputAsset = null;
    IAsset outputEncoding = null;
    try
    {
        // Load AMS account context
        log.Info($"Using Azure Media Service Rest API Endpoint : {_RESTAPIEndpoint}");

        AzureAdTokenCredentials tokenCredentials = new AzureAdTokenCredentials(_AADTenantDomain,
                                  new AzureAdClientSymmetricKey(_mediaservicesClientId, _mediaservicesClientSecret),
                                  AzureEnvironments.AzureCloudEnvironment);

        AzureAdTokenProvider tokenProvider = new AzureAdTokenProvider(tokenCredentials);

        _context = new CloudMediaContext(new Uri(_RESTAPIEndpoint), tokenProvider);

        // Get the Asset
        var asset = _context.Assets.Where(a => a.Id == assetid).FirstOrDefault();
        if (asset == null)
        {
            log.Info("Asset not found - " + assetid);
            return req.CreateResponse(HttpStatusCode.BadRequest, new { error = "Asset not found" });
        }
        log.Info("Asset found, Asset ID : " + asset.Id);

        // Declare a new Media Processing job
        job = _context.Jobs.Create("Azure Functions - Media Processing Job - " + assetid);
        ITask taskEncoding = null;
        // Encoding Jobs
        IngestAssetEncoding encodingConfig = config.IngestAssetEncoding;

        switch (encodingConfig.Encoder)
        {
            case "MES":
                {
                    string mesConfiguration = encodingConfig.EncodingConfiguration;
                    // Get a media processor reference, and pass to it the name of the processor to use for the specific task.
                    IMediaProcessor processor = GetLatestMediaProcessorByName("Media Encoder Standard");
                    // Create a task with the encoding details, using a string preset.
                    taskEncoding = job.Tasks.AddNew("Azure Functions: Media Standard encoding task", processor, mesConfiguration, TaskOptions.None);
                    // Specify the input asset to be encoded.
                    taskEncoding.InputAssets.Add(asset);
                    // Add an output asset to contain the results of the job.
                    // This output is specified as AssetCreationOptions.None, which means the output asset is not encrypted. 
                    outputEncoding = taskEncoding.OutputAssets.AddNew(asset.Name + " - Media Standard encoded (by Functions Workflow)", AssetCreationOptions.None);
                    taskindex++;
                }
                break;
            case "MEPW":
                {
                    // Get the Premium Workflow Asset
                    var workflowAsset = _context.Assets.Where(a => a.Id == encodingConfig.PremiumWorkflowAssetId).FirstOrDefault();
                    if (workflowAsset == null)
                    {
                        log.Info("Premium Workflow Asset not found - " + assetid);
                        return req.CreateResponse(HttpStatusCode.BadRequest, new { error = "Premium Workflow Asset not found" });
                    }

                    string mepwConfiguration = encodingConfig.EncodingConfiguration;
                    // Get a media processor reference, and pass to it the name of the processor to use for the specific task.
                    IMediaProcessor processor = GetLatestMediaProcessorByName("Media Encoder Premium Workflow");
                    // Create a task with the encoding details, using a string preset.
                    taskEncoding = job.Tasks.AddNew("Azure Functions: Media Premium Workflow encoding task", processor, mepwConfiguration, TaskOptions.None);
                    // Specify the input asset to be encoded.
                    taskEncoding.InputAssets.Add(workflowAsset);
                    taskEncoding.InputAssets.Add(asset);
                    // Add an output asset to contain the results of the job.
                    // This output is specified as AssetCreationOptions.None, which means the output asset is not encrypted. 
                    outputEncoding = taskEncoding.OutputAssets.AddNew(asset.Name + " - Media Premium Workflow encoded (by Functions Workflow)", AssetCreationOptions.None);
                    taskindex++;
                }
                break;
            default:
                log.Info("Azure Functions - Encoding task created : " + taskEncoding.Id);
                break;
        }

        // Media Analytics
        //AddTask(job, asset, (string)data.indexV1Language, "Azure Media Indexer", "IndexerV1.xml", "English", ref taskindex);
        AddTask(job, asset, (string)data.indexV2Language, "Azure Media Indexer 2 Preview", "IndexerV2.json", "EnUs", ref taskindex);
        AddTask(job, asset, (string)data.ocrLanguage, "Azure Media OCR", "OCR.json", "AutoDetect", ref taskindex);
        AddTask(job, asset, (string)data.faceDetectionMode, "Azure Media Face Detector", "FaceDetection.json", "PerFaceEmotion", ref taskindex);
        AddTask(job, asset, (string)data.faceRedactionMode, "Azure Media Redactor", "FaceRedaction.json", "combined", ref taskindex);
        AddTask(job, asset, (string)data.motionDetectionLevel, "Azure Media Motion Detector", "MotionDetection.json", "medium", ref taskindex);
        AddTask(job, asset, (string)data.summarizationDuration, "Azure Media Video Thumbnails", "Summarization.json", "0.0", ref taskindex);

        job.Submit();
        log.Info("Job Submitted");

        outputAsset = job.OutputMediaAssets.FirstOrDefault();
        //outputAsset = outputEncoding;
    }
    catch (Exception ex)
    {
        log.Info("Exception " + ex);
        return req.CreateResponse(HttpStatusCode.BadRequest);
    }

    log.Info("Job Id: " + job.Id);
    log.Info("Output Asset Id: " + outputAsset.Id);

    return req.CreateResponse(HttpStatusCode.OK, new
    {
        JobId = job.Id,
        OutputAssetId = outputAsset.Id
    });
}