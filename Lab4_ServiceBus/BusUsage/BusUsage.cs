﻿using System;
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
    static IReceivingRawEndpoint serverEndpoint;
    static IReceivingRawEndpoint subscriber1Endpoint;
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

    public async static Task InitSubOneEndpoint(string endpointName)
    {
        if (subscriber1Endpoint == null)
        {
            var senderConfig = RawEndpointConfiguration.Create(
            endpointName: endpointName,
            onMessage: Sub1OnMessage,
            poisonMessageQueue: "error");
            senderConfig.UseTransport<MsmqTransport>();
            senderConfig.AutoCreateQueue();
            subscriber1Endpoint = await RawEndpoint.Start(senderConfig)
                 .ConfigureAwait(false);
        }
    }
    
    public static async Task SendToSubscriber1(MessageContext context, IReceivingRawEndpoint endpoint)
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

    public static async Task SendCreate(string recv, Operation op)
    {
        var body = Serialize(op);
        var headers = new Dictionary<string, string>
        {
            ["Create"] = "Create",
            ["To"] = "Subscriber1"
        };
        var request = new OutgoingMessage(
            messageId: Guid.NewGuid().ToString(),
            headers: headers,
            body: body);

        var operation = new TransportOperation(
            request,
            new UnicastAddressTag(recv));

        await serverEndpoint.Dispatch(
               outgoingMessages: new TransportOperations(operation),
               transaction: new TransportTransaction(),
               context: new ContextBag())
           .ConfigureAwait(false);
    }

    static Task OnMessage(MessageContext context, IDispatchMessages dispatcher)
    {
        var h = context.Headers;
        var b = context.Body;
        var message = Deserialize(context.Body);      
        string create = "", destination = "";
             
        if(h.TryGetValue("Create", out create))
        {
            if(h.TryGetValue("To", out destination))
                {
                InitSubOneEndpoint("Subscriber1").GetAwaiter().GetResult();
                SendToSubscriber1(context, subscriber1Endpoint).GetAwaiter().GetResult(); 
                    Console.WriteLine("Header: " + h["Create"] + " " + message.ToString() +" Destination: " + destination);
                    Logger.WriteLog("Header: " + h["Create"] + " " + message.ToString() + " Destination: " + destination);
                }
        }
        return Task.CompletedTask;
    }

    static Task Sub1OnMessage(MessageContext context, IDispatchMessages dispatcher)
    {
        var b = context.Body;
        var message = Deserialize(context.Body);
        messagesQueue.Add(message);
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

static public class Logger
{
    public static void WriteLog(string info)
    {
        StreamWriter stream = File.AppendText("d:\\log.txt");
        stream.WriteLineAsync(DateTime.Now.ToString() + " " + info);
        stream.Close();
    }
}