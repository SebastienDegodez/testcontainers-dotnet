using System.Text.Json.Serialization;

namespace Testcontainers.Microcks.Model;

public class TestRequest
{
    [JsonPropertyName("serviceId")]
    public string ServiceId { get; set; }

    [JsonPropertyName("runnerType")]
    public TestRunnerType RunnerType { get; set; }

    [JsonPropertyName("testEndpoint")]
    public string TestEndpoint { get; set; }
   
    [JsonPropertyName("timeout")]
    public int Timeout { get; set; }

    [JsonPropertyName("filteredOperations")]
    public List<string> FilteredOperations { get; set; }

    [JsonPropertyName("operationsHeaders")]
    public Dictionary<string, List<Header>> OperationsHeaders { get; set; }

    [JsonPropertyName("oAuth2Context")]
    public OAuth2ClientContext oAuth2Context { get; set; }
}
