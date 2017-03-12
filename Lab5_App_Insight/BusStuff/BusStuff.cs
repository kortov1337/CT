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

public class BusStuff
{
    static IReceivingRawEndpoint serverEndpoint;
    static IReceivingRawEndpoint subscriber1Endpoint;
    static IReceivingRawEndpoint subscriber2Endpoint;
    static List<Operation> messagesQueue = new List<Operation>();

    public static async Task InitServerEndpoint(string endpointName)
    {
        if (serverEndpoint == null)
        {
            var senderConfig = RawEndpointConfiguration.Create(
                endpointName: endpointName,
                onMessage: OnMessage,
                poisonMessageQueue: "error");
            senderConfig.UseTransport<MsmqTransport>();
            serverEndpoint = await RawEndpoint.Start(senderConfig)
                 .ConfigureAwait(false);
        }
    }

    public async static Task InitSubOneEndpoint(string endpointName, Func<MessageContext,IDispatchMessages,Task> func)
    {
        if (subscriber1Endpoint == null)
        {
            var senderConfig = RawEndpointConfiguration.Create(
            endpointName: endpointName,
            onMessage: func,
            poisonMessageQueue: "error");
            senderConfig.UseTransport<MsmqTransport>();
            senderConfig.AutoCreateQueue();
            subscriber1Endpoint = await RawEndpoint.Start(senderConfig)
                 .ConfigureAwait(false);
        }
    }

    public async static Task InitSubTwoEndpoint(string endpointName, Func<MessageContext, IDispatchMessages, Task> func)
    {
        if (subscriber2Endpoint == null)
        {
            var senderConfig = RawEndpointConfiguration.Create(
            endpointName: endpointName,
            onMessage: func,
            poisonMessageQueue: "error");
            senderConfig.UseTransport<MsmqTransport>();
            senderConfig.AutoCreateQueue();
            subscriber2Endpoint = await RawEndpoint.Start(senderConfig)
                 .ConfigureAwait(false);
        }
    }

    public static async Task SendToSubscriber(MessageContext context, IReceivingRawEndpoint endpoint)
    {
        var headers = context.Headers;
        var request = new OutgoingMessage(
            messageId: Guid.NewGuid().ToString(),
            headers: headers,
            body: context.Body);

        var operation = new TransportOperation(
            request,
            new UnicastAddressTag(endpoint.EndpointName));

        await endpoint.Dispatch(
               outgoingMessages: new TransportOperations(operation),
               transaction: new TransportTransaction(),
               context: new ContextBag())
           .ConfigureAwait(false);
    }

    public static Task Send(string servName, Operation op)
    {
        var body = Serialize(op);
        var headers = new Dictionary<string, string>
        {
            ["Operation"] = op.OperationType,
            ["To"] = "Subscriber1"
        };
        var request = new OutgoingMessage(
            messageId: Guid.NewGuid().ToString(),
            headers: headers,
            body: body);

        var operation = new TransportOperation(
            request,
            new UnicastAddressTag(servName));

        serverEndpoint.Dispatch(
              outgoingMessages: new TransportOperations(operation),
              transaction: new TransportTransaction(),
              context: new ContextBag())
          .ConfigureAwait(false);
        return Task.CompletedTask;
    }

    static Task OnMessage(MessageContext context, IDispatchMessages dispatcher)
    {
        var h = context.Headers;
        var b = context.Body;
        var message = Deserialize(context.Body);
        string operation = "", destination = "";

        if (h.TryGetValue("Operation", out operation))
        {
          if (h.TryGetValue("To", out destination))
            {
                InitSubOneEndpoint("Subscriber1", null).GetAwaiter().GetResult();
                InitSubTwoEndpoint("Subscriber2", null).GetAwaiter().GetResult();
                SendToSubscriber(context, subscriber1Endpoint).GetAwaiter().GetResult();
                SendToSubscriber(context, subscriber2Endpoint).GetAwaiter().GetResult();
                Logger.WriteLog("Header: " + operation + " " + message.ToString() + " Destination: " + destination);
            }      
        }
        return Task.CompletedTask;
    }

    public static async Task Stop()
    {
        await serverEndpoint.Stop().ConfigureAwait(false);
    }

    public static List<Operation> GetMsgQueue()
    {
        return messagesQueue;
    }

    public static byte[] Serialize(object item)
    {
        MemoryStream stream1 = new MemoryStream();
        DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Operation));
        ser.WriteObject(stream1, item);
        stream1.Position = 0;
        StreamReader sr = new StreamReader(stream1);       
        var res = Encoding.UTF8.GetBytes(sr.ReadToEnd());
        stream1.Close();
        return res;
    }

    public static Operation Deserialize(byte[] item)
    {
        MemoryStream stream1 = new MemoryStream(item);
        stream1.Position = 0;
        DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Operation));
        var res = (Operation)ser.ReadObject(stream1);
        stream1.Close();
        return res;
    }

    public static void LogCall(string info)
    {
        Logger.WriteLog(info);
    }
}

static public class Logger
{
    public static void WriteLog(string info)
    {
        try
        {
            StreamWriter stream = File.AppendText("d:\\log.txt");
            stream.WriteLineAsync(DateTime.Now.ToString() + " " + info);
            stream.Close();
        }
        catch(Exception e)
        {
           
        }
    }
}