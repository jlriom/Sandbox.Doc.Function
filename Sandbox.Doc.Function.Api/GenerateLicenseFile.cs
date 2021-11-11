using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Sandbox.Doc.Function.Api
{
    public static class GenerateLicenseFile
    {
        [FunctionName("GenerateLicenseFile")]
        public static async Task Run(
            [QueueTrigger("Orders", Connection = "AzureWebJobsStorage")]Order order, 
            IBinder binder,
            ILogger log)
        {


            var outputBlob = await binder.BindAsync<TextWriter>(new BlobAttribute($"licenses/{order.OrderId}.lic")
            {
                Connection = "AzureWebJobsStorage"
            });

            outputBlob.WriteLine($"OrderId: {order.OrderId}");
            outputBlob.WriteLine($"Email: {order.Email}");
            outputBlob.WriteLine($"ProductId: {order.ProductId}");
            outputBlob.WriteLine($"PurchaseDate: {DateTime.UtcNow}");
            var md5 = System.Security.Cryptography.MD5.Create();

            var hash = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(order.Email + "secret"));

            outputBlob.WriteLine($"SecretCode: {BitConverter.ToString(hash).Replace("-", "")}");

            log.LogInformation($"C# Queue trigger function processed: {order.Email}");
        }
    }
}
