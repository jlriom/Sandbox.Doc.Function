using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SendGrid.Helpers.Mail;
using System;
using System.Text.RegularExpressions;

namespace Sandbox.Doc.Function.Api
{
    public static class EmailLicenseFile
    {
        [FunctionName("EmailLicenseFile")]
        public static void Run(
            [BlobTrigger("licenses/{orderId}.lic", Connection = "AzureWebJobsStorage")]string licenseFileContents, 
            [SendGrid(ApiKey = "SendGridApiKey")] ICollector<SendGridMessage> sender,
            [Table("orders", "orders", "{orderId}")] Order order,
            string orderId, 
            ILogger log)
        {
            var email = order.Email;

            if (email.Equals("jlriom@hotmail.es"))
            {
                log.LogInformation($"Got order from { email}\n License file name: {orderId}");
                var message = new SendGridMessage();
                message.From = new EmailAddress(Environment.GetEnvironmentVariable("EmailSender"));
                message.AddTo(email);
                var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(licenseFileContents);
                var base64 = Convert.ToBase64String(plainTextBytes);
                message.AddAttachment(orderId, base64, "text/plain");
                message.Subject = "Your licencse file";
                message.HtmlContent = "Thank you for your order";
                sender.Add(message);
            }
        }
    }
}
