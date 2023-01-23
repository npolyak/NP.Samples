using Grpc.Core;
using static Greet.Greeter;

var channel = new Channel("localhost", 5555, ChannelCredentials.Insecure);

var client = new GreeterClient(channel);

var reply = await client.SayHelloAsync(new Greet.HelloRequest { Name = "C#" });

Console.WriteLine(reply.Msg);