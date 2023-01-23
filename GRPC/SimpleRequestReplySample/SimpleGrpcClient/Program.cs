using Grpc.Core;
using static Greet.Greeter;

// get the channel connecting the client to the server
var channel = new Channel("localhost", 5555, ChannelCredentials.Insecure);

// create the GreeterClient service
var client = new GreeterClient(channel);

// call SetHello RPC on the server asynchronously and wait for the reply.
var reply = 
    await client.SayHelloAsync(new Greet.HelloRequest { Name = "C# Client" });

// print the Msg within the reply.
Console.WriteLine(reply.Msg);