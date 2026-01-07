// See https://aka.ms/new-console-template for more information

using HtmlCssToImage;
using HtmlCssToImage.Models.Requests;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateEmptyApplicationBuilder(new HostApplicationBuilderSettings
{
    Args = args,
    ContentRootPath = AppContext.BaseDirectory
});

builder.Configuration
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false);

//Configuring with just the path
//builder.Services.AddHtmlCssToImage("HCTI");
// Configuring with the section
 builder.Services.AddHtmlCssToImage(builder.Configuration.GetSection("HCTI"));

var app = builder.Build();
var client = app.Services.GetRequiredService<IHtmlCssToImageClient>();

Console.WriteLine("🚀 HCTI AOT Test Starting...");

var urlRequest = new CreateUrlImageRequest { Url = "https://google.com" };
var signedUrl = client.CreateAndRenderUrl(urlRequest);
Console.WriteLine($"✅ Signed URL Generated: {signedUrl}...");

