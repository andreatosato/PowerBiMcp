using Azure.Core;
using Azure.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.PowerBI.Api;
using Microsoft.Rest;
using PowerBiMcp.Tools;

var builder = WebApplication.CreateBuilder(args);

// Configure all logs to go to stderr (stdout is used for the MCP protocol messages).
builder.Logging.AddConsole(o => o.LogToStandardErrorThreshold = LogLevel.Trace);

builder.Services.AddScoped<IPowerBIClient, PowerBIClient>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();

    // Read Azure AD configuration from appsettings.json
    var tenantId = configuration["AzureAd:TenantId"];
    var clientId = configuration["AzureAd:ClientId"];
    var clientSecret = configuration["AzureAd:ClientSecret"];

    //var tenantId = "214cc121-4ce0-4752-953b-40fd8801626b";
    //var clientId = "5bf580c6-201a-4aa5-a65e-335b3e898a11";
    //var clientSecret = "S7U8Q~aAFl~fLJ45V~rT8Wm-Cmmkccvq7VSqLbrj";

    // Create credential for authentication
    var credential = new ClientSecretCredential(tenantId, clientId, clientSecret);

    // Define Power BI scope
    var scopes = new[] { "https://analysis.windows.net/powerbi/api/.default" };

    // Get access token
    var tokenRequestContext = new TokenRequestContext(scopes);
    var accessToken = credential.GetToken(tokenRequestContext, CancellationToken.None);

    // Create TokenCredentials with the bearer token
    var tokenCredentials = new TokenCredentials(accessToken.Token, "Bearer");

    return new PowerBIClient(new Uri("https://api.powerbi.com/"/*"configuration["PowerBi:ServiceRootUrl"]*/!), tokenCredentials);
});

// Add the MCP services: the transport to use (stdio) and the tools to register.
builder.Services
    .AddMcpServer()
    .WithHttpTransport()
    //.WithStdioServerTransport()
    .WithTools<PowerBiMcp.Tools.WorkspaceTools>()
    .WithTools<DatasetTools>();

var app = builder.Build();

app.MapMcp();
//await app.RunAsync();
await app.RunAsync("http://localhost:3001");