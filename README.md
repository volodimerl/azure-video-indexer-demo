---
page_type: sample
languages:
- csharp
products:
- azure
- azure-functions
- azure-media-services
azureDeploy: https://raw.githubusercontent.com/Azure-Samples/media-services-dotnet-functions-integration/master/azuredeploy.json
---

# Integrating Azure Media Services v2 with Azure Functions and Logic Apps
This project contains examples of using Azure Functions with Azure Media Services.

The project includes several folders of sample Azure Functions for use with Azure Media Services that show workflows related
to ingesting content directly from blob storage, encoding, and writing content back to blob storage. It also includes examples of
how to monitor job notifications via WebHooks and Azure Queues.

## Deploying to Azure
It is **REQUIRED** that you first fork the project and update the "sourceCodeRepositoryURL" in the [azuredeploy.json](azuredeploy.json) template parameters
when deploying to your own Azure account.  That way you can more easily update, experiment and edit the code and see changes
reflected quickly in your own Functions deployment.

We are doing this to save you from our future updates that could break your functions due to continuous integration.

**WARNING**: If you attempt to deploy from the public samples Github repo, and not your own fork, you will see an Error during deployment with a "BadRequest" and an OAuth exception. 

## Questions & Help

If you have questions about Azure Media Services and Functions, we encourage you to reach out and participate in our community.
The Media Services engineering and product management team monitors the following community sites and is available to help.

 - For all questions and technical help, [our MSDN forums](https://social.msdn.microsoft.com/forums/azure/en-US/home?forum=MediaServices) are an easy place to have a conversation with our product team.
 - For questions which fit the Stack Overflow format ("*how* does this work?"), we monitor the [azure-media-services](http://stackoverflow.com/questions/tagged/azure%20media%20service) tag.
 - You can also tweet/follow [@MSFTAzureMedia](https://twitter.com/MSFTAzureMedia).
 
While we do our best to help out in a timely basis, we don't have any promise around the above resources. If you need an SLA on support from us, it's recommended you invest in an [Azure Support plan](https://azure.microsoft.com/en-us/support/options/).


## Contributions and Best Practices

Ideas and contributions are always welcome. We are trying to build a community around creating unique Media workflows that combine
the power of Azure Media Services with Azure Functions and Logic Apps. 

Please follow the **Contribution Guides** in the "1-CONTRIBUTION-GUIDE" folder. 
+ Read the [Contribution Guide](/1-CONTRIBUTION-GUIDE/README.md)
+ Follow the [Best Practices](/1-CONTRIBUTION-GUIDE/best-practices.md)
+ Get started quickly with the [Git Tutorial](/1-CONTRIBUTION-GUIDE/git-tutorial.md)

If you have questions or ideas, please reach out to us on our [MSDN forum](https://social.msdn.microsoft.com/forums/azure/en-US/home?forum=MediaServices), Twitter at [@MSFTAzureMedia](https://twitter.com/MSFTAzureMedia), or on StackOverflow using the tag [azure-media-services](http://stackoverflow.com/questions/tagged/azure-media-services)

## How to edit the code

To edit the code, you have a few options:

* [Visual Studio Code](https://code.visualstudio.com/) (VS Code)

    VS Code may have some issues with validating run.csx files, as it attempts to resolve some dependencies. You might want to use VS Code in combination with the [Azure Functions CLI](https://www.npmjs.com/package/azure-functions-cli). For more information, see [this blog](https://blogs.msdn.microsoft.com/appserviceteam/2016/12/01/running-azure-functions-locally-with-the-cli/).
    
* Visual Studio 2015.

    To use VS 2015, you need to install the following:
    
    * [Visual Studio 2015 Update 3](https://www.visualstudio.com/news/releasenotes/vs2015-update3-vs) with Microsoft Web Developer Tools. 
    * [Azure 2.9.6 .NET SDK](https://go.microsoft.com/fwlink/?LinkId=518003&clcid=0x409) 
    * [Visual Studio Tools for Azure Functions](https://aka.ms/azfunctiontools)

    For more detailed information, see [this blog](https://blogs.msdn.microsoft.com/webdev/2016/12/01/visual-studio-tools-for-azure-functions/).

* Visual Studio 2017

    To use VS 2017, you need to install the following:
    * [Visual Studio 2017 15.5 or later](https://www.visualstudio.com/vs/)
    * [Azure Functions Tools for Visual Studio](https://docs.microsoft.com/en-us/azure/azure-functions/functions-develop-vs)
        * Azure Functions Tools is included in the Azure development workload
        * Make sure you include the Azure development workload in your Visual Studio 2017 installation.

    You can develop pre-compiled functions in this platform. See [this article](https://docs.microsoft.com/en-us/azure/azure-functions/functions-develop-vs) for more detailed information.

## How to run the sample

<a href="https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2FAzure-Samples%2Fmedia-services-dotnet-functions-integration%2Fmaster%2Fazuredeploy.json" target="_blank">
    <img src="http://azuredeploy.net/deploybutton.png"/>
</a>

To run the samples:
+ Make sure that you have a Media Services Account created, and configure a Service Principal to access it ([Follow this article](https://docs.microsoft.com/en-us/azure/media-services/media-services-portal-get-started-with-aad#service-principal-authentication))
+ first fork this project into your own repository, and then deploy the Functions with the [azuredeploy.json](azuredeploy.json) template
+ Make sure to update the path to point to your github fork
+ Set the **Project** app setting to the desired folder name for the solution sample that you wish to deploy.  
+ If you wish to switch sample projects after deployment, you can simple update the **Project** app setting and then force a GIT Sync through the Continuous Integration settings of your deployed Functions App

The deployment template will automatically create the following Azure resources:
* This Azure Functions application with your source code configured for continuous integration.
* A storage account to run with the functions.
* The required function's application settings will be updated to point to the new resources automatically. You can modify any of these settings after deployment.

Note : if you never provided your GitHub account in the Azure portal before, the continous integration probably will probably fail and you won't see the functions. In that case, you need to setup it manually. Go to your azure functions deployment / Functions app settings / Configure continous integration. Select GitHub as a source and configure it to use your fork.

### Function Application Settings 
The following applications settings are created upon deployment and are automatically linked to the resources
deployed with the azuredeploy.json template.

- **Project** - this tells the Continuous Integration system which folder to sync the Function App to.  You can modify this at any time to point to a different folder in the main solution repo to try other sample Functions. 
- **AMSAADTenantDomain** - your Media Services Account Azure AD Tenant Domain.
- **AMSRESTAPIEndpoint** - your Media Services Account REST API endpoint.
- **AMSClientId** - your Service Principal Client Id.
- **AMSClientSecret** - your Service Principal Client Secret.
- **MediaServicesStorageAccountName** - the storage account name tied to your Media Services account.
- **MediaServicesStorageAccountKey** - the storage account key tied to your Media Services account.
- **MediaServicesAttachedStorageCredentials** - list of attached storage accounts with the key, separated by ';'. This is used by some functions. Example "amstore01;gdsgdhjgj=;amstore02;dqghjqfqfjfld="
- **StorageConnection** -  the functions.json file contains a "StorageConnection" property which must be set to an App Setting value that
  contains a connection string for your input storage account. Otherwise, you may end up with an error message at startup.
  Make sure to add a new AppSetting to your Functions project with the storage account name and connection string, and update
  the functions.json file if you see this error:
- **SigningKey** - the 64-byte Base64 encoded signing key to use to protect and secure your WebHooks callbacks from Azure Media Services.
    This key is for sample purposes only and you should replace this key with your own.
    
    Example value: `wOlDEUJ4/VN1No8HxVxpsRvej0DZrO5DXvImGLjFhfctPGFiMkUA0Cj8HSfJW7lePX9XsfHAMhw30p0yYqG+1A==`
* **WebHookEndpoint** - the Webhook URL endpoint for the deployed Notification_Webhook_Function in this project to be used by Azure Media Services
  to callback to your Function from the Encoding job Functions.
  

  ### Connection Strings:
  If you are adjusting the deployment settings to use an existing Media Services account or storage account, 
  you can find the connection string for your storage account in the Azure portal(Ibiza). Go to Access Keys in Settings. In the Access Keys blade
  go to Key1, or Key2, click the "..." menu and select "view connection string". Copy the connection string.
  
  ### Code Modifications Required:
  The output container name can be modifed in run.csx by changing the value of the static string _outputContainerName.
  It's set to "output" by default.

## 100 Basic Encoding

### EncodeBlob_SingleOut_Function
The EncodeBlob_SingleOut_Function demonstrates how to use an Output binding and the "InOut" direction binding to 
allow the Azure functions framework to create the output blob for you automatically. 

In the function.json, you will notice that we use a binding direction of "InOut" and also set the name to "outputBlob".
The path is also updated to point to a specific output container, and a pattern is provided for naming the output file. 
Notice that we are binding the input {filename} to the output {filename} pattern match, and also specifying a default
extension of "-Output.mp4".

    {
      "name": "outputBlob",
      "type": "blob",
      "direction": "InOut",
      "path": "output/{fileName}-Output.mp4",
      "connection": "StorageConnection"
    }

In the run.csx file, we then bind this outputBlob to the Run method signature as a CloudBlockBlob. 

    public static void Run( CloudBlockBlob inputBlob,
                            string fileName, 
                            string fileExtension, 
                            CloudBlockBlob outputBlob,
                            TraceWriter log)

To output data to this outputBlob, we have to copy data into it. The CopyBlob() helper method (in 'Shared/copyBlobHelpers.csx') is used to copy the stream 
from the source blob to the output blob. Since the copy is done async, we have to call Wait() and halt the function execution until the copy is complete.

    CopyBlob(jobOutput,outputBlob).Wait();

Finally, we can set a few properties on the outputBlob before the function returns, and the blob is written to the configured 
output storage account set in the function.json binding.

          
    // Change some settings on the output blob.
    outputBlob.Metadata["Custom1"] = "Some Custom Metadata";
    outputBlob.Properties.ContentType = "video/mp4";
    outputBlob.SetProperties();

### EncodeBlob_Notify_Webhook_Function

This function demonstrates how to use WebHooks to listen to a basic encoding job's progress.  
The function works in combination with the Notification_Webhook_Function, which acts as that "callback" for the Job status
Notifications.

When setting up the Job in this function, you will note that the webhook is passed in as a Notification endpoint along with its
signing key for securing the payload.  You must set the signingKey and the WebHook endpoint in the App settings as specified above.

This workflow for this function waits for content to be copied into the input container in blob storage. 
This is configured in the function.json file's bindings.

    {
        "name": "inputBlob",
        "type": "blobTrigger",
        "direction": "in",
        "path": "input/{fileName}.{fileExtension}",
        "connection": "StorageConnection"
    }

The name property sets the name of the CloudBlockBlob property that is passed into the Run method. 
The path property sets the container name and file matching pattern to use. In this example,
we set the {fileName} and {fileExtension} matching patterns to pass the two values into the Run function.

    public static void Run(CloudBlockBlob inputBlob, TraceWriter log, string fileName, string fileExtension)

You can monitor the callbacks in the Notification_Webhook_Function logs while the job is running. To test the method, drop 
a new media file into the container specified in the binding's input path. 


### EncodeBlob_MultiOut_Function

This function can call a Logic App at the end.
Specify the call back Url in **LogicAppCallbackUrl** in your function's Application Settings.


### EncodeBlob_MultiOut_MultiFilesInput_Function (Multiple files / single asset Function)
This function will upload several files into a single asset.
A json file must be uploaded to the blob container withh the referenced files.

The format of the json file is:

    [
      {
        "fileName": "BigBuckBunny.mp4",
        "isPrimary": true
      },
      {
        "fileName": "Logo.png"
      }
    ]

## Aspera Ingest
### 103-aspera-ingest

This function provides a template that you can use to deploy an entire solution for ingesting and encoding using the Aspera On Demand high speed ingest services on Azure.

Aspera On Demand for Microsoft Azure is a product offering that provides access to Aspera’s patented FASP® high-speed transfer software and the Microsoft Azure cloud platform as part of an online usage-based subscription, offered as a service through the [Microsoft Azure Store](http://cloud.asperasoft.com/aspera-on-demand/aspera-on-demand-for-microsoft-azure/)
An [FAQ](http://cloud.asperasoft.com/ja/aspera-on-demand/aspera-on-demand-for-microsoft-azure-faq/) is provided by Aspera. 

Aspera is seperately priced through the Azure Marketplace and different tiers. For a promotional code to test the services out, you can reach out to Aspera through their [contact form](http://cloud.asperasoft.com/ja/contact-us/)

Use the Deploy to Azure button to launch the template that will install the following resources in your Azure Account:
    - Aspera On Demand service from the Marketplace
    - Azure Storage Account
    - Azure Function with basic code sample to ingest, encode, create an asset

See the [Read Me for 103-aspera-ingest](/103-aspera-ingest/README.md) for more details. 

## Media Services Functions for Logic Apps
### media-functions-for-logic-app
Functions : add-textfile-to-asset, check-blob-copy-to-asset-status, check-blob-copy-to-container-status, check-job-status, check-task-status, create-empty-asset, delete-asset-files, delete-entity, generate-ism-manifest, list-asset-files, live-subclip-analytics, publish-asset, return-analytics, return-subtitles, set-media-ru, start-asset-copy-to-container, start-blob-copy-to-asset, submit-job, sync-asset.
These functions are designed to be called by a Logic App.

One specific patterns to pay attention to here include the check-job-status function which is used to poll for job status from a Logic App workflow.

Five logic apps samples are available as ARM templates in [media-functions-for-logic-app](/media-functions-for-logic-app): One basic VOD worflow (that does encoding), one bacic VOD workflow with blob trigger, one more advanced (that does encoding, indexing, subtitles translation), one that processes a live stream for analytics and one which imports pre-encoded files as single asset in Azure Media Services. They can be easily deployed through a "Deploy to Azure" button in this section.

In order to practice the deployment of Azure functions for Media Services and the deployment of a Logic App advanced workflow, a detailed hands-on guide was written. You can find the document [here](/media-functions-for-logic-app/Lab).

### 201-logic-app-workflow-1
This set of functions shows how to build a complex workflow using Logic Apps and Azure Functions.
For details on setting up the Logic App and using these functions please refer to the 
documentation page [201-logic-app-workflow-1](201-logic-app-workflow-1/README.md)

## License
This sample project is licensed under [the MIT License](https://github.com/Azure-Samples/media-services-dotnet-functions-integration/blob/master/LICENSE).

## To-DO and Roadmap
- [ ] The Azure Queue notification function is not yet complete
- [ ] Copy Blobs currently is using Streams, and copies in an inefficient way.
- [X] Contribution Guide and Best practices
- [X] Document the Logic Apps functions

## History
- 08/28/2017 - Updated all code to use new AAD Service Principal authentication
- 11/29/2017 - Updated functions for logic app (from webhook to standard, SDK updates)

---
_This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/). For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments._
