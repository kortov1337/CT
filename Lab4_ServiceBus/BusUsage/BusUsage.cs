using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Extensibility;
using NServiceBus.Raw;
using NServiceBus.Routing;
using NServiceBus.Transport;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Text;
using DAL;
public class BusUsage
{
    #region Configuration
    static IReceivingRawEndpoint endpointInstance;
    public static async Task InitServerEndpoint(string endpointName)
    {
        if(endpointInstance==null)
        { 
        var senderConfig = RawEndpointConfiguration.Create(
            endpointName: endpointName,
            onMessage: OnMessage,
            poisonMessageQueue: "error");
        senderConfig.UseTransport<MsmqTransport>();
        endpointInstance = await RawEndpoint.Start(senderConfig)
             .ConfigureAwait(false);
        }
    }
    #endregion

    #region Sending
    public static async Task SendCreate(string recv, Operation op)
    {
        var body = Serialize(op);
        var headers = new Dictionary<string, string>
        {
            ["Create"] = "true"
        };
        var request = new OutgoingMessage(
            messageId: Guid.NewGuid().ToString(),
            headers: headers,
            body: body);

        var operation = new TransportOperation(
            request,
            new UnicastAddressTag(recv));

        await endpointInstance.Dispatch(
               outgoingMessages: new TransportOperations(operation),
               transaction: new TransportTransaction(),
               context: new ContextBag())
           .ConfigureAwait(false);


    }
    #endregion

    #region Receiving

    static Task OnMessage(MessageContext context, IDispatchMessages dispatcher)
    {
        //Console.WriteLine(message);
        // Can use dispatcher to send messages from here

        var h = context.Headers;
        var b = context.Body;
        var message = Deserialize(context.Body);
        Console.WriteLine(h["Create"]);
        return Task.CompletedTask;
    }


    #endregion

    public static async Task Stop()
    {
        await endpointInstance.Stop().ConfigureAwait(false);
    }
    static byte[] Serialize(object item)
    {
        MemoryStream stream1 = new MemoryStream();
        DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Operation));
        ser.WriteObject(stream1, item);
        stream1.Position = 0;
        StreamReader sr = new StreamReader(stream1);  
        var res = Encoding.ASCII.GetBytes(sr.ReadToEnd());
        stream1.Close();
        return res;
    }

    static Operation Deserialize(byte[] item)
    {
        Encoding.ASCII.GetString(item);
        MemoryStream stream1 = new MemoryStream(item);
        stream1.Position = 0;
        DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Operation));
        var res = (Operation)ser.ReadObject(stream1);
        stream1.Close();
        return res;
    }
}