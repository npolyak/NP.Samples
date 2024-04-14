using Grpc.Core;
using Grpc.Net.Client;
using simple;

// get the channel connecting the client to the server
var channel =
    GrpcChannel.ForAddress("https://localhost:55003");

// create the GreeterClient service
var client = new Greeter.GreeterClient(channel);



Console.WriteLine($"Unary server call sample:");
// call SetHello RPC on the server asynchronously and wait for the reply.
var reply =
    await client.SayHelloAsync(new HelloRequest { Name = "C# Client" });

Console.WriteLine("Greeting: " + reply.Msg);


Console.WriteLine();
Console.WriteLine();

Console.WriteLine($"Streaming Server Sample:");

var serverStreamingCall = client.SayManyHellos(new HelloRequest { Name = "C# Client" });

await foreach(var response in serverStreamingCall.ResponseStream.ReadAllAsync())
{
    Console.WriteLine("Greeting: " + response.Msg);
}
Console.WriteLine();
Console.WriteLine();


Console.WriteLine($"Streaming Client Sample:");

var clientSreamingCall = client.SayHelloToMany();

for(int i = 0; i < 3;  i++)
{
    await clientSreamingCall.RequestStream.WriteAsync(new HelloRequest { Name = $"Client_{i + 1}" });
}

await clientSreamingCall.RequestStream.CompleteAsync();
var clientStreamingResponse = await clientSreamingCall;

Console.WriteLine(clientStreamingResponse.Msg);