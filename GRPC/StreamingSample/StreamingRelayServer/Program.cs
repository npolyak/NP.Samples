using Grpc.Core;
using Service;
using StreamingRelayServer;

Server server = new Server
{
    Services = { RelayService.BindService(new RelayServiceImplementations()) }
};

server.Ports.Add(new ServerPort("localhost", 5555, ServerCredentials.Insecure));

server.Start();

Console.ReadLine();