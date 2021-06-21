using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PostmarkDotNet;
using PostmarkDotNet.Model;

namespace BulkyBook.Utility
{
    public class EmailSender : IEmailSender
    {

        private readonly EmailOptions _emailOptions;

        public EmailSender(IOptions<EmailOptions> options)
        {
            _emailOptions = options.Value;
        }


        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            return Execute(_emailOptions.PostMarkClient, subject, htmlMessage, email);
        }


        private async Task Execute(string postMarkClient, string subject, string htmlmessage, string email)
        {

            var client = new PostmarkClient(postMarkClient);

            var message = new PostmarkMessage()
            {
                To = email,
                From = "Declan.Gregorczyk@alsglobal.com",
                TrackOpens = true,
                Subject = subject,
                HtmlBody = htmlmessage,
            };


            var sendResult = await client.SendMessageAsync(message);

            if (sendResult.Status == PostmarkStatus.Success)
            {
                /* Handle success */ 
            }
            else 
            {
                /* Resolve issue.*/ 
            }

            /*
                    //  var apiKey = Environment.GetEnvironmentVariable("NAME_OF_THE_ENVIRONMENT_VARIABLE_FOR_YOUR_SENDGRID_KEY");
                    var client = new SendGridClient(sendGridKey);
                    var from = new EmailAddress("Admin@bulky.com", "BulkyBook");
                    var to = new EmailAddress(email, "End User");
                    //  var plainTextContent = "and easy to do anywhere, even with C#";
                    //  var htmlContent = "<strong>and easy to do anywhere, even with C#</strong>";
                    var msg = MailHelper.CreateSingleEmail(from, to, subject, "", message);
                    return client.SendEmailAsync(msg);
            */
        }

        /*
        private Task Execute(string sendGridKey, string subject, string message, string email)
        {
            //  var apiKey = Environment.GetEnvironmentVariable("NAME_OF_THE_ENVIRONMENT_VARIABLE_FOR_YOUR_SENDGRID_KEY");
            var client = new SendGridClient(sendGridKey);
            var from = new EmailAddress("Admin@bulky.com", "BulkyBook");
            var to = new EmailAddress(email, "End User");
            //  var plainTextContent = "and easy to do anywhere, even with C#";
            //  var htmlContent = "<strong>and easy to do anywhere, even with C#</strong>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, "", message);
            return client.SendEmailAsync(msg);
        }
        */


    }
}
