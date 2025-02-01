using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Functions.Worker;
using fnPostDatabase;
using System.Web.Http;

namespace Az204Catalog
{
    public static class fnPostDatabase
    {
        [FunctionName("Movie")]
        [CosmosDBOutput("%DatabaseName%", "%ContainerName%", Connection = "ConnectionString", CreateIfNotExists = true, PartitionKey = "Id")]
        public static async Task<object> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            try
            {
                using var reader = new StreamReader(req.Body);
                var content = JsonConvert.DeserializeObject<MovieRequest>(await reader.ReadToEndAsync());

                return JsonConvert.SerializeObject(content);
            }
            catch (System.Exception)
            {
                return new InternalServerErrorResult();
            }

        }
    }
}
