
using Greet;
using Microsoft.AspNetCore.Builder;
using SimpleGrpcServerTest;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

// create GreeterImplementation object providing the 
// RPC SayHello implementation
GreeterImplementation greeterImplementation = new GreeterImplementation();

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Services.AddGrpc();

var app = builder.Build();

app.MapGrpcService<GreeterImplementation>();

app.Run();