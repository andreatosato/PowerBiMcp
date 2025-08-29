using Microsoft.Extensions.AI;
using Microsoft.PowerBI.Api;
using Microsoft.Rest;
using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Text.Json;

namespace PowerBiMcp.Tools;

internal class DatasetTools
{
    private readonly IPowerBIClient _powerBIClient;

    public DatasetTools(IPowerBIClient powerBIClient)
    {
        _powerBIClient = powerBIClient;
    }

    [McpServerTool]
    [Description("Get datasets from a specific workspace.")]
    public TextContent GetDatasetsByWorkspaceId(
        [Description("The ID of the workspace")] string workspace_id
    )
    {
        try
        {
            var datasets = _powerBIClient.Datasets.GetDatasetsInGroup(Guid.Parse(workspace_id));
            return new TextContent(JsonSerializer.Serialize(datasets));
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

    [McpServerTool]
    [Description("Get tables from a specific dataset.")]
    public TextContent GetDatasetTables(
        [Description("The ID of the workspace")] string workspace_id,
        [Description("The ID of the dataset")] string dataset_id
    )
    {
        try
        {
            var tables = _powerBIClient.Datasets.GetTables(Guid.Parse(workspace_id), dataset_id);
            return new TextContent(JsonSerializer.Serialize(tables));
        }
        catch (HttpOperationException ex)
        {
            return new TextContent($"Error getting dataset tables: {ex.Message}\nResponse: {ex.Response?.Content}");
        }
        catch (Exception ex)
        {
            return new TextContent($"Error getting dataset tables: {ex.Message}");
        }
    }

}
