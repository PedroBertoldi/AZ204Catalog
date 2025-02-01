using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;
using Azure.Storage.Blobs;
using System.Web.Http;

namespace Az204Catalog
{
    public static class fnDataStorage
    {
        [FunctionName("DataStorage")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Processing file");

            try
            {
                if (!req.Headers.TryGetValue("file-type", out var fileTypeHeader))
                {
                    return new BadRequestObjectResult("file-type header is required");
                }

                var form = await req.ReadFormAsync();
                var file = form.Files["file"];

                if (file is null || file.Length == 0)
                {
                    return new BadRequestObjectResult("File is required");
                }

                var connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
                var containerName = fileTypeHeader.ToString();
                var containerClient = new BlobContainerClient(connectionString, containerName);

                await containerClient.CreateIfNotExistsAsync();
                await containerClient.SetAccessPolicyAsync(Azure.Storage.Blobs.Models.PublicAccessType.BlobContainer);

                var blob = containerClient.GetBlobClient(file.FileName);
                await using (var stream = file.OpenReadStream()) {
                    await blob.UploadAsync(stream, true);
                }

                return new OkObjectResult(blob.Uri);
            }
            catch (System.Exception)
            {
                return new InternalServerErrorResult();
            }
        }
    }
}
