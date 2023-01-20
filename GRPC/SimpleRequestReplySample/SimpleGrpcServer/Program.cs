
using Greet;
using Grpc.Core;
using SimpleGrpcServerTest;

GreeterImplementation greeterImplementation = new GreeterImplementation();

Server server = new Server
{
    Services = { Greeter.BindService(new GreeterImplementation()) }
};

server.Ports.Add(new ServerPort("localhost", 5555, ServerCredentials.Insecure));

server.Start();

Console.ReadLine();

server.ShutdownAsync().Wait();  