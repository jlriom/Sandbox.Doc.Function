using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace Sandbox.Doc.Function.Api
{
    public static class OnPaymentReceived
    {
        [FunctionName("OnPaymentReceived")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        [OpenApiRequestBody("application/json", typeof(Order))]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            [Queue("orders", Connection = "AzureWebJobsStorage")] IAsyncCollector<Order> orderQueue, 
            [Table("orders", Connection = "AzureWebJobsStorage")] IAsyncCollector<Order> orderTable,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var order  = JsonConvert.DeserializeObject<Order>(requestBody);

            await orderQueue.AddAsync(order);

            order.PartitionKey = "orders";
            order.RowKey = order.OrderId;
            await orderTable.AddAsync(order);

            log.LogInformation($"Order {order.OrderId} received from {order.Email} for product {order.ProductId}");

            return new OkObjectResult($"Thank you for your purchase");
        }
    }

    public class Order
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public string OrderId { get; set; }
        public string ProductId { get; set; }
        public string Email { get; set; }
        public decimal Price { get; set; }
    }
}

