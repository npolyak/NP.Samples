using Grpc.Core;
using Grpc.Net.Client;
using simple;

// get the channel connecting the client to the server
var channel =
    GrpcChannel.ForAddress("https://localhost:55003");

// create the GreeterClient service
var greeterGrpcClient = new Greeter.GreeterClient(channel);

string greetingName = "Joe Doe";

Console.WriteLine($"Unary server call sample:");
// call SetHello RPC on the server asynchronously and wait for the reply.
var reply =
    await greeterGrpcClient.SayHelloAsync(new HelloRequest { Name = greetingName });

Console.WriteLine(reply.Msg);


Console.WriteLine();
Console.WriteLine();

Console.WriteLine($"Streaming Server Sample:");

// get the serverStreaming call containing an asynchronous stream
var serverStreamingCall = greeterGrpcClient.ServerStreamHelloReplies(new HelloRequest { Name = greetingName });

await foreach(var response in serverStreamingCall.ResponseStream.ReadAllAsync())
{
    // for each async response, print its Msg property
    Console.WriteLine(response.Msg);
}

Console.WriteLine();
Console.WriteLine();

Console.WriteLine($"Streaming Server Sample with Error:");
// get the serverStreaming call containing an asynchronous stream
var serverStreamingCallWithError = greeterGrpcClient.ServerStreamHelloRepliesWithError(new HelloRequest { Name = greetingName });
try
{
    await foreach (var response in serverStreamingCallWithError.ResponseStream.ReadAllAsync())
    {
        // for each async response, print its Msg property
        Console.WriteLine(response.Msg);
    }
}
catch(RpcException exception)
{
    // prints the exception message
    Console.WriteLine(exception.Message);
}

Console.WriteLine();
Console.WriteLine();

Console.WriteLine($"Streaming Client Sample:");

var clientSreamingCall = greeterGrpcClient.ClientStreamHelloRequests();

for(int i = 0; i < 5;  i++)
{
    // stream requests from the client to server
    await clientSreamingCall.RequestStream.WriteAsync(new HelloRequest { Name = $"Client_{i + 1}" });
    await Task.Delay(20);
}

// inform the server that the client streaming ended
await clientSreamingCall.RequestStream.CompleteAsync();

// get the resulting HelloReply from the server
var clientStreamingResponse = await clientSreamingCall;

// print the resulting message
Console.WriteLine(clientStreamingResponse.Msg);


Console.WriteLine();
Console.WriteLine();

Console.WriteLine($"Bidirectional Streaming Client/Server Sample:");
var clientServerStreamingCall = greeterGrpcClient.ClientAndServerStreamingTest();
var readTask = Task.Run(async () =>
{
    await foreach (var reply in clientServerStreamingCall.ResponseStream.ReadAllAsync())
    {
        Console.WriteLine(reply.Msg);
    }
});

for (int i = 0; i < 3; i++)
{
    await clientServerStreamingCall.RequestStream.WriteAsync(new HelloRequest { Name = $"Client_{i + 1}" });

    await Task.Delay(1000);
}

await Task.WhenAll(readTask, clientServerStreamingCall.RequestStream.CompleteAsync());