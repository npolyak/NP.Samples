using NP.Grpc.CommonRelayInterfaces;
using NP.Grpc.RelayServerConfig;
using NP.IoCy;

// create container builder with Enum keys
var containerBuilder = new ContainerBuilder<Enum>();

// Register IGrpcConfig type to be resolved to GrpcServerConfig objectp 
containerBuilder.RegisterSingletonType<IGrpcConfig, GrpcServerConfig>();

// Dynamically load and inject all the plugins from the subfolders of
// Plugins/Services folder under TargetFolder of the project
// TargetFolder is where the executable of the project is located
// e.g. folder bin/Debug/net6.0 under the projects directory. 
containerBuilder.RegisterPluginsFromSubFolders("Plugins/Services");

// build the IoC container from container builder
var container = containerBuilder.Build();

// get the reference to the relay server from the plugin
// The server will start running the moment it is created. 
IRelayServer relayServer = container.Resolve<IRelayServer>();

// prevent the program from exiting
Console.ReadLine();