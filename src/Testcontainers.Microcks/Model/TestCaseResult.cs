using System.Text.Json.Serialization;

namespace Testcontainers.Microcks.Model;

public class TestCaseResult
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("elapsedTime")]
    public int ElapsedTime { get; set; }

    [JsonPropertyName("operationName")]
    public string OperationName { get; set; }

    [JsonPropertyName("testStepResults")]
    public List<object> TestStepResults { get; set; }
}
