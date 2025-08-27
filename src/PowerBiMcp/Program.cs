using Azure.Core;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.PowerBI.Api;
using Microsoft.Rest;

var builder = Host.CreateApplicationBuilder(args);

// Configure all logs to go to stderr (stdout is used for the MCP protocol messages).
builder.Logging.AddConsole(o => o.LogToStandardErrorThreshold = LogLevel.Trace);

builder.Services.AddScoped<IPowerBIClient, PowerBIClient>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();

    // Read Azure AD configuration from appsettings.json
    var tenantId = configuration["AzureAd:TenantId"];
    var clientId = configuration["AzureAd:ClientId"];
    var clientSecret = configuration["AzureAd:ClientSecret"];

    // Create credential for authentication
    var credential = new ClientSecretCredential(tenantId, clientId, clientSecret);

    // Define Power BI scope
    var scopes = new[] { "https://analysis.windows.net/powerbi/api/.default" };

    // Get access token
    var tokenRequestContext = new TokenRequestContext(scopes);
    var accessToken = credential.GetToken(tokenRequestContext, CancellationToken.None);

    // Create TokenCredentials with the bearer token
    var tokenCredentials = new TokenCredentials(accessToken.Token, "Bearer");

    return new PowerBIClient(new Uri(configuration["PowerBi:ServiceRootUrl"]!), tokenCredentials);
});

// Add the MCP services: the transport to use (stdio) and the tools to register.
builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly();

await builder.Build().RunAsync();
