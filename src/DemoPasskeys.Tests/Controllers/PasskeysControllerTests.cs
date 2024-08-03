namespace DemoPasskeys.Tests.Controllers;

public class PasskeysControllerTests : IntegrationTestBase
{
    [Test]
    public async Task Should_Return_Ok()
    {
        var response = await GetAsync("/Passkeys");
        response.EnsureSuccessStatusCode();
    }
}