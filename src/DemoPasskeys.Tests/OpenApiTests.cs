using FluentAssertions;
using Snapshooter.NUnit;

namespace DemoPasskeys.Tests;

public class OpenApiTests : IntegrationTestBase
{
    [TestCase("/swagger/index.html")]
    [TestCase("/swagger/v1/swagger.json")]
    public async Task Should_Return_Ok(string path)
    {
        var response = await GetAsync(path);
        response.EnsureSuccessStatusCode();
    }

    [Test]
    public async Task Swagger_Should_Match_Snapshot()
    {
        var content = await GetStringAsync("/swagger/v1/swagger.json");
        content.Should().MatchSnapshot();
    }
}