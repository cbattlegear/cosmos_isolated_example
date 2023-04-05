using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace cosmos_isolated_example
{
    public class CreatePart
    {
        private readonly ILogger _logger;

        public CreatePart(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<CreatePart>();
        }

        [Function("CreatePart")]
        public async Task<CreatePartOutput> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            //Read request body
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Part part = JsonConvert.DeserializeObject<Part>(requestBody);

            part.id = Guid.NewGuid();

            IReadOnlyList<Part> readonlyparts = new List<Part>() { part };

            var response = req.CreateResponse(HttpStatusCode.OK);



            return new CreatePartOutput() 
            { 
                CosmosOut = readonlyparts,
                HttpResponse = response
            };
        }

        // To do multiple output bindings (http and cosmos) you have to create an intermediate class
        // https://learn.microsoft.com/en-us/azure/azure-functions/dotnet-isolated-process-guide#multiple-output-bindings
        public class CreatePartOutput 
        {
            [CosmosDBOutput("demo", "test", Connection = "CosmosConnection")]
            public IReadOnlyList<Part> CosmosOut { get; set; }
            public HttpResponseData HttpResponse { get; set; }
        }
    }
}
