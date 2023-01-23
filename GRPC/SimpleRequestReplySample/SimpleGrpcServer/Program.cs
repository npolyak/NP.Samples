
using Greet;
using Grpc.Core;
using SimpleGrpcServerTest;

// create GreeterImplementation object providing the 
// RPC SayHello implementation
GreeterImplementation greeterImplementation = new GreeterImplementation();

// bind the server with the greeterImplementation so that SayHello RPC called on 
// the server will be channeled over to greeterImplementation.SayHello
Server server = new Server
{
    Services = { Greeter.BindService(greeterImplementation) }
};

// set the server host, port and security (insecure)
server.Ports.Add(new ServerPort("localhost", 5555, ServerCredentials.Insecure));

// start the server
server.Start();

// wait with shutdown until the user presses a key
Console.ReadLine();

// shutdown the server
server.ShutdownAsync().Wait();  