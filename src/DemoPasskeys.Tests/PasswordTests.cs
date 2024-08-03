using FluentAssertions;

namespace DemoPasskeys.Tests;

public class PasswordTests
{
    [Test]
    public void Should_Hash_Password()
    {
        const string plain = "peter";
        const string expected = "AmrZsUp0U7dIjaoMasvCWLFQb1LEQcfEZUdMGlZDlP8=";

        var hash = Password.Hash(plain);

        hash.Should().BeEquivalentTo(expected);
    }
}