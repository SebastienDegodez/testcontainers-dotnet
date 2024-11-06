using System.Text.Json.Serialization;

namespace Testcontainers.Microcks.Model;

public class TestResult
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("version")]
    public int Version { get; set; }

    [JsonPropertyName("testNumber")]
    public int TestNumber { get; set; }

    [JsonPropertyName("testDate")]
    public long TestDate { get; set; }

    [JsonPropertyName("testedEndpoint")]
    public string TestedEndpoint { get; set; }

    [JsonPropertyName("serviceId")]
    public string ServiceId { get; set; }

    [JsonPropertyName("timeout")]
    public int Timeout { get; set; }

    [JsonPropertyName("elapsedTime")]
    public int ElapsedTime { get; set; }

    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("inProgress")]
    public bool InProgress { get; set; }

    [JsonPropertyName("runnerType")]
    public TestRunnerType RunnerType { get; set; }

    [JsonPropertyName("testCaseResults")]
    public List<TestCaseResult> TestCaseResults { get; set; }
}
