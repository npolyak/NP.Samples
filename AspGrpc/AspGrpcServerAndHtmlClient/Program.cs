using GrpcServerProcess;
using Microsoft.AspNetCore.StaticFiles;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddGrpc();

builder.Services.AddWebEncoders();
builder.Services.AddHealthChecks();

string corsPolicyName = "CorsPolicy";

builder.Services.AddCors
(
    options =>
    {
        options.AddPolicy
        (
            corsPolicyName,
            builder =>
            {
                builder
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            }
        );
    }
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

var contentTypeProvider = new FileExtensionContentTypeProvider();
var dict = new Dictionary<string, string>
    {
        {".pdb" , "application/octet-stream" },
        {".blat", "application/octet-stream" },
        {".bin", "application/octet-stream" },
        {".dll" , "application/octet-stream" },
        {".dat" , "application/octet-stream" },
        {".json", "application/json" },
        {".wasm", "application/wasm" },
        {".symbols", "application/octet-stream" }
    };
foreach (var kvp in dict)
{
    contentTypeProvider.Mappings[kvp.Key] = kvp.Value;
}

app.UseDefaultFiles();
app.UseStaticFiles(/*new StaticFileOptions { ContentTypeProvider = contentTypeProvider }*/);
app.UseRouting();
app.UseGrpcWeb();


app.UseCors(corsPolicyName);

//app.UseAuthorization();

app.MapRazorPages();

app.MapGrpcService<GreeterImplementation>().EnableGrpcWeb().RequireHost("*:55003");

app.Run();
