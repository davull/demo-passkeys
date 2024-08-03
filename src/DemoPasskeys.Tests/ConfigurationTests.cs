using FluentAssertions;

namespace DemoPasskeys.Tests;

public class ConfigurationTests
{
    [Test]
    public void DataDirectory_Should_ReturnDataDirectory()
    {
        var dataDirectory = Configuration.DataDirectory;
        dataDirectory.Should().NotBeNullOrWhiteSpace();
    }
}