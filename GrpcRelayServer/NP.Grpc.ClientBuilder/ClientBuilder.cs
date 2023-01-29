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
                // create container builder with keys limited to Enum (enumeration values)
                var containerBuilder = new ContainerBuilder<System.Enum>();

                // Register GrpcServerConfig containing server Name as "localhost"
                // and server port - 5555 to be retuned by the container 
                // for the IGrpcConfig type.
                containerBuilder.RegisterType<IGrpcConfig, GrpcServerConfig>();

                // register multicell of cell type Enum and resolution key IoCKeys.Topics
                containerBuilder.RegisterMultiCell(typeof(System.Enum), IoCKeys.Topics);

                // get the plugins from Plugins/Services folder under
                // the folder containing client executable
                containerBuilder.RegisterPluginsFromSubFolders("Plugins/Services");
                var container = containerBuilder.Build();

                // create the relay client
                _relayClient = container.Resolve<IRelayClient>();
            }

            // return relay client
            return _relayClient;
        }
    }
}