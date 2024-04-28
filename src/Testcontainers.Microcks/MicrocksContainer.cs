
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
namespace Testcontainers.Microcks;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class MicrocksContainer : DockerContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MicrocksContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    public MicrocksContainer(MicrocksConfiguration configuration)
        : base(configuration)
    {
        Starting += (_, _) => Logger.LogInformation("MicrocksContainer container is starting, performing configuration.");
        Started += (_, _) => Logger.LogInformation("MicrocksContainer container is ready! UI available at {Url}.", GetHttpEndpoint());
    }

    public Uri GetGraphQLMockEndpoint(string name, string version)
    {
        return new UriBuilder(GetHttpEndpoint())
        {
            Path = $"graphql/{name}/{version}"
        }.Uri;
    }

    public Uri GetGrpcMockEndpoint()
    {
        return new UriBuilder("grpc", Hostname, GetMappedPublicPort(MicrocksBuilder.MicrocksGrpcPort)).Uri;
    }

    /// <summary>
    /// Obtains the HTTP endpoint for the Microcks container.
    /// </summary>
    /// <returns>HTTP endpoint address</returns>
    public Uri GetHttpEndpoint()
    {
        return new UriBuilder( 
            Uri.UriSchemeHttp, 
            Hostname, 
            GetMappedPublicPort(MicrocksBuilder.MicrocksHttpPort)
        ).Uri;
    }

    public Uri GetRestMockEndpoint(string name, string version)
    {
        return new UriBuilder(this.GetHttpEndpoint())
        {
            Path = $"rest/{name}/{version}"
        }.Uri;
    }

    public Uri GetSoapMockEndpoint(string name, string version)
    {
        return new UriBuilder(GetHttpEndpoint())
        {
            Path = $"soap/{name}/{version}"
        }.Uri;
    }

    public void ImportAsMainArtifact(string artifact)
    {
        this.ImportArtifactAsync(artifact, true).GetAwaiter().GetResult();
    }

    protected override ValueTask DisposeAsyncCore()
    {
        return base.DisposeAsyncCore();
    }
}