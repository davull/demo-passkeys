using System.Net;
using System.Text.Json;
using FluentAssertions;

namespace DemoPasskeys.Tests.Controllers;

public class PasskeysApiControllerTests : IntegrationTestBase
{
    [TestCaseSource(nameof(UserPasskeyTestCases))]
    public async Task UserPasskey_Should_Store_Passkey(StorePasskeyModel model)
    {
        var response = await PostJsonAsync("/api/passkeys/userpasskey", model);
        response.EnsureSuccessStatusCode();
    }

    [Test]
    public async Task Verify_With_Invalid_Id_Should_Return_Unauthorized()
    {
        var model = Dummies.VerifyPasskeyModel("invalid");
        var response = await PostJsonAsync("/api/passkeys/verify", model);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Test]
    public async Task Verify_With_Invalid_Signature_Should_Return_Unauthorized()
    {
        var model = Dummies.VerifyPasskeyModel(
            "test-101",
            "uzLHnR90LeW8TN7lyKDreFnTIik5+rK0UAv28GHbsWcFAAAAAQ==",
            "{\"type\":\"webauthn.get\",\"challenge\":\"kpDSmxMRU2pYueQlWzk_dTdSYCMpB5-BLpB0uuql4IA\",\"origin\":\"https://passkeys.demo:7011\",\"crossOrigin\":false}",
            "aW52YWxpZA=="); // base64 "invalid"

        var response = await PostJsonAsync("/api/passkeys/verify", model);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [TestCaseSource(nameof(VerifyPasskeyTestCases))]
    public async Task Verify_With_Valid_Credentials_Should_Authorize(VerifyPasskeyModel model)
    {
        var response = await PostJsonAsync("/api/passkeys/verify", model);
        response.EnsureSuccessStatusCode();
    }

    private static IEnumerable<TestCaseData> UserPasskeyTestCases() => TestCases().Select(tuple =>
        new TestCaseData(tuple.request) { TestName = tuple.name });

    private static IEnumerable<TestCaseData> VerifyPasskeyTestCases() => TestCases().Select(tuple =>
        new TestCaseData(tuple.verify) { TestName = tuple.name });

    private static IEnumerable<(string name, StorePasskeyModel request, VerifyPasskeyModel verify)> TestCases()
    {
        var content = File.ReadAllText("Controllers/PasskeysTestData.json");
        var testCases = JsonDocument.Parse(content).RootElement.EnumerateArray();

        foreach (var testCase in testCases)
        {
            var name = testCase.GetProperty("Name").GetString()!;
            var request = testCase.GetProperty("CreatePasskeyRequest").Deserialize<StorePasskeyModel>()!;
            var verify = testCase.GetProperty("VerifyPasskeyRequest").Deserialize<VerifyPasskeyModel>()!;
            yield return (name, request, verify);
        }
    }
}