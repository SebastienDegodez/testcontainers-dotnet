using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;
using System.Net;
using System;

using static RestAssured.Dsl;
using Testcontainers.Microcks.Model;

namespace Testcontainers.Microcks.Tests;

public sealed class MicrocksContractTestingFunctionalityTests : IAsyncLifetime
{
  private readonly INetwork _network = new NetworkBuilder().Build();
  private readonly MicrocksContainer _microcksContainer;
  private readonly IContainer _badImpl;
  private readonly IContainer _goodImpl;


  private static readonly string BAD_PASTRY_IMAGE = "quay.io/microcks/contract-testing-demo:01";
  private static readonly string GOOD_PASTRY_IMAGE = "quay.io/microcks/contract-testing-demo:02";

  public MicrocksContractTestingFunctionalityTests()
  {
    _microcksContainer = new MicrocksBuilder()
        .WithNetwork(_network)
        .Build();

    _badImpl = new ContainerBuilder()
      .WithImage(BAD_PASTRY_IMAGE)
      .WithNetwork(_network)
      .WithNetworkAliases("bad-impl")
      .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged(".*Example app listening on port 3001.*"))
      .Build();

    _goodImpl = new ContainerBuilder()
      .WithImage(GOOD_PASTRY_IMAGE)
      .WithNetwork(_network)
      .WithNetworkAliases("good-impl")
      .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged(".*Example app listening on port 3002.*"))
      .Build();
  }

  public Task DisposeAsync()
  {
    return Task.WhenAll(
      _microcksContainer.DisposeAsync().AsTask(),
      _badImpl.DisposeAsync().AsTask(),
      _goodImpl.DisposeAsync().AsTask(),
      _network.DisposeAsync().AsTask()
    );
  }

  public Task InitializeAsync()
  {
    _microcksContainer.Started +=
      (_, _) => _microcksContainer.ImportAsMainArtifact("apipastries-openapi.yaml");

    return Task.WhenAll(
      _microcksContainer.StartAsync(),
      _badImpl.StartAsync(),
      _goodImpl.StartAsync()
    );
  }

  [Fact]
  public void ShouldConfigRetrieval()
  {
    var uriBuilder = new UriBuilder(_microcksContainer.GetHttpEndpoint())
    {
      Path = "/api/keycloak/config"
    };

    Given()
      .When()
      .Get(uriBuilder.ToString())
      .Then()
      .StatusCode(HttpStatusCode.OK);
  }

  [Fact]
  public async Task Should()
  {
    var testRequest = new TestRequest
    {
      ServiceId = "API Pastries:0.0.1",
      RunnerType = TestRunnerType.OPEN_API_SCHEMA,
      TestEndpoint = "http://bad-impl:3001",
      Timeout = 2000
    };

    TestResult testResult = await _microcksContainer.TestEndpointAsync(testRequest);
  }
}
