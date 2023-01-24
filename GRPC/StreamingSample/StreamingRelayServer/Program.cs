using Grpc.Core;
using Service;
using StreamingRelayServer;

// bind RelayServiceImplementations to the gRPC server.
Server server = new Server
{
    Services = { RelayService.BindService(new RelayServiceImplementations()) }
};

// set the server to be connected to port 5555 on the localhost without 
// any security
server.Ports.Add(new ServerPort("localhost", 5555, ServerCredentials.Insecure));

// start the server
server.Start();

// prevent the server from exiting.
Console.ReadLine();