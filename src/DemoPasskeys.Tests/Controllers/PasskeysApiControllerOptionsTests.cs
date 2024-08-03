using FluentAssertions;

namespace DemoPasskeys.Tests.Controllers;

public class PasskeysApiControllerOptionsTests : IntegrationTestBase
{
    [Test]
    public async Task Get_PublicKeyCredentialCreationOptions_Should_Return_Json()
    {
        var response = await GetAsync("/api/passkeys/publickeycredentialcreationoptions", true);
        response.EnsureSuccessStatusCode();

        response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");

        var content = await response.Content.ReadAsStringAsync();
        content.Should().NotBeNullOrWhiteSpace();
    }

    [Test]
    public async Task Get_PublicKeyCredentialRequestOptions_Should_Return_Json()
    {
        var response = await GetAsync("/api/passkeys/PublicKeyCredentialRequestOptions", true);
        response.EnsureSuccessStatusCode();

        response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");

        var content = await response.Content.ReadAsStringAsync();
        content.Should().NotBeNullOrWhiteSpace();
    }
}