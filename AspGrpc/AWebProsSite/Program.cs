using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.StaticFiles;
using System.IO.Compression;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddResponseCompression
(
    options =>
    {
        options.EnableForHttps = true;
        options.Providers.Add<BrotliCompressionProvider>();
        options.Providers.Add<GzipCompressionProvider>();
    }
);

// Add services to the container.
builder.Services.AddRazorPages();


builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Fastest;
});

builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.SmallestSize;
});

builder.Services.AddWebEncoders();
//builder.Services.AddHealthChecks();

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
app.UseResponseCompression();

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
        {".symbols", "application/octet-stream" },
        {".ts", "application/octet-stream" }
    };
foreach (var kvp in dict)
{
    contentTypeProvider.Mappings[kvp.Key] = kvp.Value;
}

app.UseDefaultFiles();

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles
    (
        new StaticFileOptions 
        { 
            ContentTypeProvider = contentTypeProvider, 
            HttpsCompression = HttpsCompressionMode.Compress 
        }
    );

app.UseRouting();

app.UseCors(corsPolicyName);

//app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
