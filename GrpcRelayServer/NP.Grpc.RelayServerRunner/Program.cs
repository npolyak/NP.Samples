using NP.Grpc.CommonRelayInterfaces;
using NP.Grpc.RelayServerConfig;
using NP.IoCy;
using NP.Protobuf;

var containerBuilder = new ContainerBuilder<Enum>();

containerBuilder.RegisterType<IGrpcConfig, GrpcServerConfig>();

containerBuilder.RegisterMultiCell(typeof(Enum), IoCKeys.Topics);

containerBuilder.RegisterPluginsFromSubFolders("Plugins/Services");

var container = containerBuilder.Build();

IRelayServer relayServer = container.Resolve<IRelayServer>();

Console.ReadLine();