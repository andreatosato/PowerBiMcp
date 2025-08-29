using Microsoft.Extensions.AI;
using Microsoft.PowerBI.Api;
using Microsoft.Rest;
using System.Text.Json;

namespace PowerBiMcp.Tools;

internal class ReportTools
{
    private readonly IPowerBIClient _powerBIClient;

    public ReportTools(IPowerBIClient powerBIClient)
    {
        _powerBIClient = powerBIClient;
    }

    public async Task<TextContent> GetReportsAsync(string workspaceId)
    {
        try
        {
            var data = await _powerBIClient.Reports.GetReportAsync(Guid.Parse(workspaceId));
            return new TextContent(JsonSerializer.Serialize(data));
        }
        catch (HttpOperationException ex)
        {
            return new TextContent($"Error retrieving datasets: {ex.Message}\nResponse: {ex.Response?.Content}");
        }
        catch (Exception ex)
        {
            return new TextContent($"Error: {ex.Message}");
        }
    }
}
