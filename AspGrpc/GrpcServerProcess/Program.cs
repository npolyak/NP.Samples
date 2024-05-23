using GrpcServerProcess;

// create builder
var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();

// add grpc service
builder.Services.AddGrpc();
builder.Services.AddSingleton(new GreeterImplementation());

// create the Kestrel application
var app = builder.Build();

// specifies GreeterImplementation as the gRPC service to run and 
// channel to it all the requests that specify 55003 port
app.MapGrpcService<GreeterImplementation>().RequireHost("*:55003");

// runs Kestrel server. 
await app.RunAsync();