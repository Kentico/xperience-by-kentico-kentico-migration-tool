namespace Migration.Toolkit.Common.Services.Ipc;

using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

public class IpcService
{
    private const string IPC_PING_PATH = "/ToolkitApi/Test";
    private const string IPC_DISCOVERED_INFO_PATH = "/ToolkitApi/GetAllDefinitions";

    private readonly ToolkitConfiguration _toolkitConfiguration;
    private readonly ILogger<IpcService> _logger;

    public IpcService(ToolkitConfiguration toolkitConfiguration, ILogger<IpcService> logger)
    {
        _toolkitConfiguration = toolkitConfiguration;
        _logger = logger;
    }

    public async Task<bool> IsConfiguredAsync()
    {
        var advancedFeatures = _toolkitConfiguration.OptInFeatures?.QuerySourceInstanceApi;
        var connections = advancedFeatures?.Connections ?? new List<SourceInstanceInfo>();

        if (!(advancedFeatures?.Enabled ?? false))
        {
            _logger.LogInformation("Advanced features are disabled");
            return false;
        }
        else
        {
            _logger.LogInformation("Advanced features are enabled");
        }
        
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
                
                var json = JsonConvert.SerializeObject(new { secret = connectionInfo.Secret });
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                request.Content = content;
                
                results.Add(await hc.SendAsync(request));
            }
            else
            {
                _logger.LogError("SourceInstanceUri is required");
                return false;
            }
        }

        var ok = true;
        foreach (var httpResponseMessage in results)
        {
            httpResponseMessage.EnsureSuccessStatusCode();

            try
            {
                var response = await httpResponseMessage.Content.ReadAsStringAsync();
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
                _logger.LogError(ex, "Error while connecting to source instance");
                ok = false;
            }
        }

        return ok;
    }

    public async Task<Dictionary<string, SourceInstanceDiscoveredInfo>> GetSourceInstanceDiscoveredInfos()
    {
        var advancedFeatures = _toolkitConfiguration.OptInFeatures?.QuerySourceInstanceApi;
        var connections = advancedFeatures?.Connections ?? new List<SourceInstanceInfo>();

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
                
                var json = JsonConvert.SerializeObject(new { secret = connectionInfo.Secret });
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                request.Content = content;
                
                var response = await hc.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();
                if (JsonConvert.DeserializeObject<SourceInstanceDiscoveredInfo>(responseBody) is {} deserializedResponse)
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