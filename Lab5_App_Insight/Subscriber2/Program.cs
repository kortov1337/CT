using NServiceBus.Transport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;

namespace Subscriber2
{
    class Program
    {
       
        const string fromPassword = "12345678Q";
        static string subject = "";
        static string body = "";

        static void Main(string[] args)
        {
            Init();
            Console.ReadKey();
        }
        //
        public static async void Init()
        {
            await BusStuff.InitSubTwoEndpoint("Subscriber2", OnMessage);
        }
        public static Task OnMessage(MessageContext context, IDispatchMessages dispatcher)
        {
            try { 
            var fromAddress = new MailAddress("fpahov@gmail.com", "Subscriber2");
            var toAddress = new MailAddress("fpahov@gmail.com", "To Admin");
            var message = BusStuff.Deserialize(context.Body);
            subject = message.OperationType;
            body=message.ToString();
            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };
            using (var msg = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                smtp.Send(msg);
                BusStuff.LogCall("Emailed: " + msg.Body);
            }         
            return Task.CompletedTask;
            }
            catch(Exception e)
            {
                BusStuff.LogCall(e.Message + Environment.NewLine + "Failed for " + subject + " : " + body);
                return Task.CompletedTask;
            }
        }
    }
}
