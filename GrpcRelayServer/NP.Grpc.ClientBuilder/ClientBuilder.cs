using NP.Grpc.CommonRelayInterfaces;
using NP.Grpc.RelayServerConfig;
using NP.IoCy;
using NP.Protobuf;

namespace NP.Grpc.ClientBuilder
{
    public static class ClientBuilder
    {
        private static IRelayClient? _relayClient;

        public static IRelayClient GetClient()
        {
            if (_relayClient == null)
            {
                var containerBuilder = new ContainerBuilder<System.Enum>();

                containerBuilder.RegisterType<IGrpcConfig, GrpcServerConfig>();

                containerBuilder.RegisterMultiCell(typeof(System.Enum), IoCKeys.Topics);

                containerBuilder.RegisterPluginsFromSubFolders("Plugins/Services");
                var container = containerBuilder.Build();

                _relayClient = container.Resolve<IRelayClient>();
            }

            return _relayClient;
        }
    }
}