using Greet;
using Grpc.Net.Client;
using System;
using System.ComponentModel;

// get the channel connecting the client to the server
var channel = 
    GrpcChannel.ForAddress("https://localhost:50051"); 

// create the GreeterClient service
var client = new Greeter.GreeterClient(channel);

// call SetHello RPC on the server asynchronously and wait for the reply.
var reply = 
    await client.SayHelloAsync(new Greet.HelloRequest { Name = "C# Client" });

Console.WriteLine("Greeting: " + reply.Msg);

Console.WriteLine("Shutting down");
Console.WriteLine("Press any key to exit...");
Console.ReadKey();
