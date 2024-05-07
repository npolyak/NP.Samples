using GrpcServerProcess;

// create ASP.NET application builder.
var builder = WebApplication.CreateBuilder(args);

// Add a service generating razor pages
builder.Services.AddRazorPages();

// add a service for grpc
builder.Services.AddGrpc();

// build the application
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// use default file (Index.cshtml) when no path is specified after 
// server:port combination
app.UseDefaultFiles();

// allow using static files (e.g. .js, html, etc)
app.UseStaticFiles();

// use grpc-web 
app.UseGrpcWeb();

// allow razor pages generation
app.MapRazorPages();

// create the GreeterImplementation service and allow it to be accessed from grpc-web
app.MapGrpcService<GreeterImplementation>().EnableGrpcWeb();

// start the ASP.NET server
app.Run();
