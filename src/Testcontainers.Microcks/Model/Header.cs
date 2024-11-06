using System.Text.Json.Serialization;
using Testcontainers.Microcks.Converter;

namespace Testcontainers.Microcks.Model;

public class Header
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("values")]
    [JsonConverter(typeof(ArrayToStringConverter))]
    public string Values { get; set; }
}