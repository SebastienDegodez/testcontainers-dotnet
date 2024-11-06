using System.Text.Json.Serialization;

namespace Testcontainers.Microcks.Model;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TestRunnerType
{
    HTTP,
    SOAP_HTTP,
    SOAP_UI,
    POSTMAN,
    OPEN_API_SCHEMA,
    ASYNC_API_SCHEMA,
    GRPC_PROTOBUF,
    GRAPHQL_SCHEMA
}