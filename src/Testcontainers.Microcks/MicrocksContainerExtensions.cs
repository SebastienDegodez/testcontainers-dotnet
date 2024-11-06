using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Testcontainers.Microcks.Model;

namespace Testcontainers.Microcks;

public static class MicrocksContainerExtensions
{
    public static HttpClient Client => _lazyClient.Value;
    private static Lazy<HttpClient> _lazyClient = new Lazy<HttpClient>(() => new HttpClient()
    {
        DefaultRequestHeaders =
        {
            Accept = { MediaTypeWithQualityHeaderValue.Parse("application/json") },
            CacheControl = CacheControlHeaderValue.Parse("no-cache")
        }
    });


    public static async Task<TestResult> TestEndpointAsync(
        this MicrocksContainer container, TestRequest testRequest)
    {
        string httpEndpoint = container.GetHttpEndpoint() + "api/tests";
        var content = new StringContent(JsonSerializer.Serialize(testRequest), Encoding.UTF8, "application/json");
        var responseMessage = await Client.PostAsync(httpEndpoint, content);

        if (responseMessage.StatusCode == HttpStatusCode.Created)
        {
            var responseContent = await responseMessage.Content.ReadAsStringAsync();

            var testResult = JsonSerializer.Deserialize<TestResult>(responseContent);
            var testResultId = testResult.Id;

        }

        throw new Exception("Couldn't launch on new test on Microcks. Please check Microcks container logs");
    }

    internal static async Task ImportArtifactAsync(this MicrocksContainer container, string artifact, bool mainArtifact)
    {
        string url = $"{container.GetHttpEndpoint()}api/artifact/upload" + (mainArtifact ? "" : "?mainArtifact=false");
        var result = await container.UploadFileToMicrocksAsync(artifact, url);
        if (result.StatusCode != HttpStatusCode.Created)
        {
            throw new Exception($"Artifact has not been correctly imported: {result.StatusCode}");
        }
        container.Logger.LogInformation($"Artifact {artifact} has been imported");
    }

    internal static async Task<HttpResponseMessage> UploadFileToMicrocksAsync(this MicrocksContainer container, string filepath, string url)
    {
        using (var form = new MultipartFormDataContent())
        {
            using var snapContent = new ByteArrayContent(File.ReadAllBytes(filepath));
            snapContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
            form.Add(snapContent, "file", Path.GetFileName(filepath));

            return await Client.PostAsync(url, form);
        }
    }

    internal static async Task ImportSnapshotAsync(this MicrocksContainer container, string snapshot)
    {
        string url = $"{container.GetHttpEndpoint()}api/import";
        var result = await container.UploadFileToMicrocksAsync(snapshot, url);

        if (result.StatusCode != HttpStatusCode.Created)
        {
            throw new Exception($"Snapshot has not been correctly imported: {result.StatusCode}");
        }
        container.Logger.LogInformation($"Snapshot {snapshot} has been imported");
    }

    internal static async Task CreateSecretAsync(this MicrocksContainer container, Model.Secret secret)
    {
        string url = $"{container.GetHttpEndpoint()}api/secrets";
        var content = new StringContent(JsonSerializer.Serialize(secret), Encoding.UTF8, "application/json");

        var result = await Client.PostAsync(url, content);

        if (result.StatusCode != HttpStatusCode.Created)
        {
            throw new Exception("Secret has not been correctly created");
        }
        container.Logger.LogInformation($"Secret {secret.Name} has been created");
    }

    internal static async Task DownloadArtifactAsync(this MicrocksContainer container, string remoteArtifactUrl, bool main)
    {
        var content = new StringContent("mainArtifact=" + main + "&url=" + remoteArtifactUrl, System.Text.Encoding.UTF8, "application/x-www-form-urlencoded");
        var result = await Client
            .PostAsync($"{container.GetHttpEndpoint()}api/artifact/download", content);

        if (result.StatusCode != HttpStatusCode.Created)
        {
            throw new Exception("Artifact has not been correctly downloaded");
        }
        container.Logger.LogInformation($"Artifact {remoteArtifactUrl} has been downloaded");
    }
}
