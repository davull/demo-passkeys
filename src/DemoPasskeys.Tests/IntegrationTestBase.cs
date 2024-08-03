using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace DemoPasskeys.Tests;

public abstract class IntegrationTestBase
{
    private TestWebFactory _factory = null!;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _factory = new TestWebFactory();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _factory.Dispose();
    }

    private HttpClient CreateClient()
    {
        return _factory.CreateClient(
            new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
    }

    protected async Task<HttpResponseMessage> GetAsync(string path, bool authorize = false)
    {
        var client = CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Get, path);

        if (authorize)
        {
            var cookie = await GetAuthCookie();
            request.Headers.Add("Cookie", cookie);
        }

        return await client.SendAsync(request);
    }

    protected async Task<string> GetStringAsync(string path)
    {
        var response = await GetAsync(path);
        return await response.Content.ReadAsStringAsync();
    }

    protected async Task<HttpResponseMessage> PostJsonAsync(string path, object payload)
    {
        var client = CreateClient();
        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        return await client.PostAsync(path, content);
    }

    protected async Task<HttpResponseMessage> PostFormAsync(string path,
        KeyValuePair<string, string>[] formValues)
    {
        var client = CreateClient();
        var content = new FormUrlEncodedContent(formValues);
        return await client.PostAsync(path, content);
    }

    protected async Task<string> GetAuthCookie(string email = "peter.pan@test.de",
        string password = "peter")
    {
        var response = await PostFormAsync("/User/Login",
        [
            new KeyValuePair<string, string>("email", email),
            new KeyValuePair<string, string>("password", password)
        ]);

        var raw = response.Headers
            .GetValues("Set-Cookie")
            .SingleOrDefault(s => s.StartsWith(".auth"));

        return string.IsNullOrEmpty(raw)
            ? string.Empty
            : raw.Split(";").First();
    }
}