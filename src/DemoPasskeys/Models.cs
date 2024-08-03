namespace DemoPasskeys;

public record StorePasskeyModel(
    string Id,
    string UserId,
    string PublicKey,
    int PublicKeyAlgorithm);

public record VerifyPasskeyModel(
    string Id,
    string AuthenticatorData,
    string ClientDataJson,
    string Signature,
    string UserHandle);

public record UserModel(
    string Id,
    string EmailAddress,
    string PasswordHash);