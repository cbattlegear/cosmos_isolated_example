using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace cosmos_isolated_example
{
    public class GetPart
    {
        private readonly ILogger _logger;

        public GetPart(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<GetPart>();
        }

        [Function("GetPart")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "part/{PartId}")] HttpRequestData req,
            [CosmosDBInput(databaseName: "demo",
                       containerName: "test",
                       Connection = "CosmosConnection",
                       Id ="{PartId}",
                       PartitionKey ="{PartId}")] Part part
        )
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var response = req.CreateResponse(HttpStatusCode.OK);
            //response.Headers.Add("Content-Type", "application/json; charset=utf-8");

            await response.WriteAsJsonAsync(part);

            return response;
        }
    }
}
