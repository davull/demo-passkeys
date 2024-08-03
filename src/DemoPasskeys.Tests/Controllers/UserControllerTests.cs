using System.Net;
using FluentAssertions;

namespace DemoPasskeys.Tests.Controllers;

public class UserControllerTests : IntegrationTestBase
{
    [Test]
    public async Task GetLogin_Should_ReturnOk()
    {
        var response = await GetAsync("/User/Login");
        response.EnsureSuccessStatusCode();
    }

    [TestCase("", "")]
    [TestCase("test1@example.com", "invalid-password")]
    [TestCase("invalid-user@example.com", "myPasswordIsSecure123!")]
    public async Task PostLogin_WithInvalidCredentials_Should_Return(
        string email, string password)
    {
        var response = await PostFormAsync("/User/Login",
        [
            new KeyValuePair<string, string>("email", email),
            new KeyValuePair<string, string>("password", password)
        ]);

        var content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        content.Should().Contain("Invalid email address or password");

        var cookies = response.Headers.GetValues("Set-Cookie");
        cookies.Should().NotContain(s => s.StartsWith(".auth"));
    }

    [Test]
    public async Task PostLogin_WithValidCredentials_Should_Return_Cookie()
    {
        const string email = "peter.pan@test.de";
        const string password = "peter";

        var response = await PostFormAsync("/User/Login",
        [
            new KeyValuePair<string, string>("email", email),
            new KeyValuePair<string, string>("password", password)
        ]);

        response.StatusCode.Should().Be(HttpStatusCode.Found); // 302 Found
        response.Headers.Location.Should().Be("/User/Profile");

        var cookies = response.Headers.GetValues("Set-Cookie");
        cookies.Should().Contain(s => s.StartsWith(".auth"));
    }

    [Test]
    public async Task Logout_Should_DeleteCookie()
    {
        var response = await GetAsync("/User/Logout");

        response.StatusCode.Should().Be(HttpStatusCode.Found); // 302 Found
        response.Headers.Location.Should().Be("/");

        var cookies = response.Headers.GetValues("Set-Cookie");
        cookies.Should().Contain(".auth=; expires=Thu, 01 Jan 1970 00:00:00 GMT; path=/");
    }

    [Test]
    public async Task Profile_WithoutCookie_Should_RedirectToLogin()
    {
        var response = await GetAsync("/User/Profile");

        response.StatusCode.Should().Be(HttpStatusCode.Found); // 302 Found
        response.Headers.Location.Should().Be("/User/Login");
    }

    [Test]
    public async Task Profile_WithCookie_Should_ReturnOk()
    {
        var response = await GetAsync("/User/Profile", true);

        response.EnsureSuccessStatusCode();
    }
}