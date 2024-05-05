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

var serverStreamingCall = greeterGrpcClient.ServerStreamHelloReplies(new HelloRequest { Name = greetingName });

await foreach(var response in serverStreamingCall.ResponseStream.ReadAllAsync())
{
    Console.WriteLine(response.Msg);
}

Console.WriteLine();
Console.WriteLine();

Console.WriteLine($"Streaming Server Sample with Error:");
var serverStreamingCallWithError = greeterGrpcClient.ServerStreamHelloRepliesWithError(new HelloRequest { Name = greetingName });
try
{
    await foreach (var response in serverStreamingCallWithError.ResponseStream.ReadAllAsync())
    {
        Console.WriteLine(response.Msg);
    }
}
catch(RpcException exception)
{
    Console.WriteLine(exception.Message);
}

Console.WriteLine();
Console.WriteLine();

Console.WriteLine($"Streaming Client Sample:");

var clientSreamingCall = greeterGrpcClient.ClientStreamHelloRequests();

for(int i = 0; i < 3;  i++)
{
    await clientSreamingCall.RequestStream.WriteAsync(new HelloRequest { Name = $"Client_{i + 1}" });
}

await clientSreamingCall.RequestStream.CompleteAsync();
var clientStreamingResponse = await clientSreamingCall;

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

    await Task.Delay(20);
}

await Task.WhenAll(readTask, clientServerStreamingCall.RequestStream.CompleteAsync());