using System;
using PimFullItemCatalog.Swagger;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PimItemFullCatalog.Interfaces;
using AzureFunctions.Extensions.Swashbuckle.Attribute;

namespace PimItemFullCatalog.Services
{
    public class PimFullItemCatalog
    {
        const string _containerName = "pimitemexports";
        private readonly IBlobService _blobService;
        public PimFullItemCatalog(IBlobService blobService)
        {
            _blobService = blobService;
        }

        [FunctionName("ListAllItemFiles")]
        public async Task<IActionResult> ListAllItemFiles(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            var result = await _blobService.ListAllFilesAsync(_containerName);

            return new OkObjectResult(result);
        }

        [FunctionName("DownloadItemFile")]
        [QueryStringParameter("itemNumber", "The item number for which you need the pim data.", DataType = typeof(Int64), Required = true)]
        public async Task<IActionResult> DownloadItemFile(
    [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
    ILogger log)
        {
            var filename = req.Query["itemNumber"].ToString() + ".json";

            var result = await _blobService.DownloadFileAsync(filename, _containerName);

            return new FileStreamResult(result, "application/octet-stream")
            {
                FileDownloadName = filename
            };
        }

        [FunctionName("UploadFile")]
        [HttpPost]
        [SwaggerParameter("jsonFile", "A file", Required = true, Type = "file")]
        public async Task<IActionResult> UploadFile(
     [HttpTrigger(AuthorizationLevel.Anonymous,  "post", Route = null)] HttpRequest req,
          ILogger log, ExecutionContext context)
        {
            var file = req.Form.Files["File"];
            var myBlob = file.OpenReadStream();
            await _blobService.UploadFileAsync(myBlob, _containerName, file.FileName, file.ContentType);
            return new OkObjectResult("file uploaded successfylly");
        }

    }
}
