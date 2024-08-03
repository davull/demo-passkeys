namespace DemoPasskeys.Tests;

public static class Dummies
{
    public static UserModel UserModel(
        string? id = null, string? email = null,
        string? plaintextPassword = null)
    {
        id ??= Guid.NewGuid().ToString();
        email ??= "john.doe@example.com";
        plaintextPassword ??= "password";

        var passwordHash = Password.Hash(plaintextPassword);

        return new UserModel(id, email, passwordHash);
    }

    public static StorePasskeyModel StorePasskeyModel(
        string publicKey, string? id = null, string? userId = null,
        int publicKeyAlgorithm = -7)
    {
        id ??= Guid.NewGuid().ToString();
        userId ??= "test-001";

        return new StorePasskeyModel(id, userId, publicKey, publicKeyAlgorithm);
    }

    public static VerifyPasskeyModel VerifyPasskeyModel(
        string id = "", string authenticatorData = "",
        string clientDataJson = "{ }", string signature = "",
        string userHandle = "")
    {
        return new VerifyPasskeyModel(id, authenticatorData, clientDataJson,
            signature, userHandle);
    }
}