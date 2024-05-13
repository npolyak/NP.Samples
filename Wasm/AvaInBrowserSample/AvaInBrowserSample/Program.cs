using Microsoft.AspNetCore.StaticFiles;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

var provider = new FileExtensionContentTypeProvider();

// create a dictionary of mime types to add
var dict = new Dictionary<string, string>
    {
        {".pdb" , "application/octet-stream" },
        {".blat", "application/octet-stream" },
        {".dll" , "application/octet-stream" },
        {".dat" , "application/octet-stream" },
        {".json", "application/json" },
        {".wasm", "application/wasm" },
        {".symbols", "application/octet-stream" }
    };

// add the dictionary entries to the provider
foreach (var kvp in dict)
{
    provider.Mappings[kvp.Key] = kvp.Value;
}

// set the provider to contain the added
// mime types
app.UseStaticFiles(new StaticFileOptions
{
    ContentTypeProvider = provider
});

app.MapRazorPages();

app.Run();
