using NP.Grpc.CommonRelayInterfaces;

namespace NP.Grpc.RelayServerConfig
{
    public class GrpcServerConfig : IGrpcConfig
    {
        public string ServerName => "localhost";

        public int Port => 5555;
    }
}