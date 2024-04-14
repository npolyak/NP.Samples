using GrpcServerProcess;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Services.AddGrpc();

var app = builder.Build();

app.MapGrpcService<GreeterImplementation>().RequireHost("*:55003");

app.Run();
