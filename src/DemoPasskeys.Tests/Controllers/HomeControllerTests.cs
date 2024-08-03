namespace DemoPasskeys.Tests.Controllers;

public class HomeControllerTests : IntegrationTestBase
{
    [TestCase("/")]
    [TestCase("/Home")]
    [TestCase("/Home/Index")]
    public async Task Should_Return_Ok(string path)
    {
        var response = await GetAsync(path);
        response.EnsureSuccessStatusCode();
    }
}