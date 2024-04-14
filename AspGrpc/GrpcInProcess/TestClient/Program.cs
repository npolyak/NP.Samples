using Grpc.Net.Client;
using simple;

// get the channel connecting the client to the server
var channel =
    GrpcChannel.ForAddress("https://localhost:7116");

// create the GreeterClient service
var client = new Greeter.GreeterClient(channel);

// call SetHello RPC on the server asynchronously and wait for the reply.
var reply =
    await client.SayHelloAsync(new HelloRequest { Name = "C# Client" });

Console.WriteLine("Greeting: " + reply.Msg);
