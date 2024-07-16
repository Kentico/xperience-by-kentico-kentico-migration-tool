using System.Text;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

namespace Migration.Toolkit.Common.Services.Ipc;

public class IpcService(ToolkitConfiguration toolkitConfiguration, ILogger<IpcService> logger)
{
    private const string IPC_PING_PATH = "/ToolkitApi/Test";
    private const string IPC_DISCOVERED_INFO_PATH = "/ToolkitApi/GetAllDefinitions";

    public async Task<bool> IsConfiguredAsync()
    {
        var advancedFeatures = toolkitConfiguration.OptInFeatures?.QuerySourceInstanceApi;
        var connections = advancedFeatures?.Connections ?? [];

        if (!(advancedFeatures?.Enabled ?? false))
        {
            logger.LogInformation("Advanced features are disabled");
            return false;
        }

        logger.LogInformation("Advanced features are enabled");

        var hc = new HttpClient();
        var results = new List<HttpResponseMessage>();
        foreach (var connectionInfo in connections)
        {
            if (connectionInfo.SourceInstanceUri != null)
            {
                var pingUri = new Uri(connectionInfo.SourceInstanceUri, IPC_PING_PATH);
                var request = new HttpRequestMessage(
                    HttpMethod.Post, pingUri
                );

                string json = JsonConvert.SerializeObject(new { secret = connectionInfo.Secret });
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                request.Content = content;

                results.Add(await hc.SendAsync(request));
            }
            else
            {
                logger.LogError("SourceInstanceUri is required");
                return false;
            }
        }

        bool ok = true;
        foreach (var httpResponseMessage in results)
        {
            httpResponseMessage.EnsureSuccessStatusCode();

            try
            {
                string response = await httpResponseMessage.Content.ReadAsStringAsync();
                if (JsonConvert.DeserializeObject(response) is { } deserializeObject)
                {
                    ok &= ((dynamic)deserializeObject).pong == true;
                }
                else
                {
                    ok = false;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while connecting to source instance");
                ok = false;
            }
        }

        return ok;
    }

    public async Task<Dictionary<string, SourceInstanceDiscoveredInfo>> GetSourceInstanceDiscoveredInfos()
    {
        var advancedFeatures = toolkitConfiguration.OptInFeatures?.QuerySourceInstanceApi;
        var connections = advancedFeatures?.Connections ?? [];

        var discoveredInfoList = new Dictionary<string, SourceInstanceDiscoveredInfo>(StringComparer.InvariantCultureIgnoreCase);

        foreach (var connectionInfo in connections)
        {
            if (connectionInfo.SourceInstanceUri != null)
            {
                var pingUri = new Uri(connectionInfo.SourceInstanceUri, IPC_DISCOVERED_INFO_PATH);
                var hc = new HttpClient();
                var request = new HttpRequestMessage(
                    HttpMethod.Post, pingUri
                );

                string json = JsonConvert.SerializeObject(new { secret = connectionInfo.Secret });
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                request.Content = content;

                var response = await hc.SendAsync(request);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                if (JsonConvert.DeserializeObject<SourceInstanceDiscoveredInfo>(responseBody) is { } deserializedResponse)
                {
                    discoveredInfoList.Add(deserializedResponse.SiteName, deserializedResponse);
                }
                else
                {
                    throw new Exception("Invalid response to IPC from source instance");
                }
            }
        }

        return discoveredInfoList;
    }
}
