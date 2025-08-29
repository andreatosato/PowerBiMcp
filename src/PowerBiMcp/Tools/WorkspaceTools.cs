using Microsoft.Extensions.AI;
using Microsoft.PowerBI.Api;
using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Text.Json;

namespace PowerBiMcp.Tools;

internal class WorkspaceTools
{
    private readonly IPowerBIClient _powerBIClient;
    public WorkspaceTools(IPowerBIClient powerBIClient)
    {
        _powerBIClient = powerBIClient;
    }

    [McpServerTool]
    [Description("Get all Power BI workspaces accessible to the user.")]
    public async Task<TextContent> GetWorkspacesAsync()
    {
        try
        {
            var data = await _powerBIClient.Groups.GetGroupsAsync();
            return new TextContent(JsonSerializer.Serialize(data));
        }
        catch (Exception e)
        {
            return new TextContent($"Error getting workspaces: {e.Message}");
        }
    }
}


internal class RandomNumberTools
{
    [McpServerTool]
    [Description("Generates a random number between the specified minimum and maximum values.")]
    public int GetRandomNumber(
        [Description("Minimum value (inclusive)")] int min = 0,
        [Description("Maximum value (exclusive)")] int max = 100)
    {
        return Random.Shared.Next(min, max);
    }
}
