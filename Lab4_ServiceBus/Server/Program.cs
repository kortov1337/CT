using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Extensibility;
using NServiceBus.Raw;
using NServiceBus.Routing;
using NServiceBus.Transport;
using NServiceBus.Logging;

namespace Server
{
    class Program
    {
        static ILog log = LogManager.GetLogger<Program>();
        static String endpointName = "ServerUI";

        static void Main(string[] args)
        {
            BusUsage.InitServerEndpoint(endpointName).GetAwaiter().GetResult();
            //BusUsage.Send(endpointName);
            Console.WriteLine("Press Enter to exit...");
            Console.ReadLine();
            //AsyncMain().GetAwaiter().GetResult();
            BusUsage.Stop();
        }

     
}
}
