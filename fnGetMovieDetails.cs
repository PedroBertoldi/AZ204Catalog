using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Cosmos;
using fnPostDatabase;
using System.Collections.Generic;
using System.Web.Http;

namespace Az204Catalog
{
    public static class fnGetMovieDetails
    {
        [FunctionName("details")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log,
            CosmosClient cosmosClient)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            try
            {
                var container = cosmosClient.GetContainer(Environment.GetEnvironmentVariable("DatabaseName"), Environment.GetEnvironmentVariable("ContainerName"));
                var id = req.Query[nameof(MovieRequest.Id)];
                var query = new QueryDefinition("SELECT * FROM c WHERE c.Id = @id")
                    .WithParameter("@id", id);
                var response = container.GetItemQueryIterator<MovieRequest>(query);
                var results = new List<MovieRequest>();
                while (response.HasMoreResults) {
                    foreach (var item in await response.ReadNextAsync())
                    {
                        results.Add(item);
                    }
                }

                return new OkObjectResult(results);
            }
            catch (System.Exception)
            {
                return new InternalServerErrorResult();
            }
        }
    }
}
