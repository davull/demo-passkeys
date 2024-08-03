using System.Security.Cryptography;
using System.Text;

namespace DemoPasskeys;

public static class Passkeys
{
    private const string RpId = "passkeys.demo";

    private const int Es256 = -7;
    private const int Rs256 = -257;

    public static object CreatePublicKeyCredentialCreationOptions(UserModel user)
    {
        // https://developer.mozilla.org/en-US/docs/Web/API/PublicKeyCredentialCreationOptions

        var userId = Encoding.UTF8.GetBytes(user.Id);
        var challenge = Challenge();

        var options = new
        {
            // An ArrayBuffer, TypedArray, or DataView provided by the relying  party's server and used as a
            // cryptographic challenge. This value will be signed by the authenticator and the signature will be
            // sent back as part of AuthenticatorAttestationResponse.attestationObject.
            challenge = Convert.ToBase64String(challenge),
            // An object describing the relying party that requested the credential creation.
            rp = new
            {
                id = RpId,
                name = "Passkeys Demo Corp"
            },
            // An object describing the user account for which the credential is generated.
            user = new
            {
                // An ArrayBuffer, TypedArray, or DataView representing a unique ID for the user account. This value
                // has a maximum length of 64 bytes,  and is not intended to be displayed to the user.
                id = Convert.ToBase64String(userId),
                name = user.EmailAddress,
                displayName = user.EmailAddress
            },
            // An Array of objects which specify the key types and signature algorithms the Relying Party supports,
            // ordered from most preferred to least preferred. The client and authenticator will make a best-effort
            // to create a credential of the most preferred type possible.
            pubKeyCredParams = new object[]
            {
                new { type = "public-key", alg = Es256 },
                new { type = "public-key", alg = Rs256 }
            },
            excludeCredentials = Array.Empty<object>(),
            authenticatorSelection = new
            {
                requireResidentKey = true,
                userVerification = "preferred"
            }
        };

        return options;
    }

    public static object CreatePublicKeyCredentialRequestOptions()
    {
        // https://developer.mozilla.org/en-US/docs/Web/API/CredentialsContainer/get#web_authentication_api

        var challenge = Challenge();
        var options = new
        {
            challenge = Convert.ToBase64String(challenge),
            rpId = RpId
        };
        return options;
    }

    public static bool Verify(VerifyPasskeyModel data, StorePasskeyModel storedPasskey)
    {
        // https://w3c.github.io/webauthn/#sctn-public-key-easy
        // https://w3c.github.io/webauthn/#authenticator-data
        // https://w3c.github.io/webauthn/#sctn-verifying-assertion

        // https://w3c.github.io/webauthn/#sctn-authenticator-data
        //  The authenticator data structure encodes contextual bindings made by the authenticator.
        var authenticatorData = Convert.FromBase64String(data.AuthenticatorData);
        var (rpIdHash, flags, signCount) = AuthenticatorData(authenticatorData);
        // Using credentialRecord.publicKey, verify that sig is a valid signature
        // over the binary concatenation of authData and hash.
        var signature = Convert.FromBase64String(data.Signature);

        try
        {
            AssertRpIdHash(rpIdHash);
            AssertFlags(flags);
            AssertCounter(signCount);
            AssertSignature(storedPasskey, signature, authenticatorData, data.ClientDataJson);

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return false;
        }
    }

    private static void AssertRpIdHash(byte[] rpIdHash)
    {
        var expected = SHA256.HashData(Encoding.UTF8.GetBytes(RpId));

        if (rpIdHash.SequenceEqual(expected) is false)
            throw new Exception("Invalid RP ID hash");
    }

    private static void AssertFlags(byte flags)
    {
        // https://w3c.github.io/webauthn/#authdata-flags

        // dec    29
        // bin    0 0 0 1   1 1 0 1
        //        | |   |   | |   |
        //        | |   |   | |   --- [0] User present (UP)
        //        | |   |   | ------- [2] User verified (UV)
        //        | |   |   --------- [3] Backup Eligibility (BE)
        //        | |   ------------- [4] Backup State (BS)
        //        | ----------------- [6] Attested credential data included (AT)
        //        ------------------- [7] Extension data included (ED)
        var up = (flags & 0b0000_0001) == 0b0000_0001;
        var uv = (flags & 0b0000_0100) == 0b0000_0100;
        var be = (flags & 0b0000_1000) == 0b0000_1000;
        var bs = (flags & 0b0001_0000) == 0b0001_0000;
        var at = (flags & 0b0100_0000) == 0b0100_0000;
        var ed = (flags & 0b1000_0000) == 0b1000_0000;

        // ...
    }

    private static void AssertCounter(uint signCount)
    {
        // https://w3c.github.io/webauthn/#authdata-signcount

        // ...
    }

    private static void AssertSignature(StorePasskeyModel stored, byte[] signature,
        byte[] authenticatorData, string clientDataJson)
    {
        var bytes = Encoding.UTF8.GetBytes(clientDataJson);
        var hash = SHA256.HashData(bytes);
        byte[] data = [..authenticatorData, ..hash];

        var valid = AssertSignature(stored, data, signature);
        if (valid is false)
            throw new Exception("Invalid signature");
    }

    private static bool AssertSignature(StorePasskeyModel stored, byte[] data, byte[] signature)
    {
        var publicKeyInfo = Convert.FromBase64String(stored.PublicKey);

        switch (stored.PublicKeyAlgorithm)
        {
            case Es256:
            {
                using var ecdsa = CreateEcdsa(publicKeyInfo);
                var valid = ecdsa.VerifyData(data, signature, HashAlgorithmName.SHA256,
                    DSASignatureFormat.Rfc3279DerSequence);
                return valid;
            }
            case Rs256:
            {
                using var rsa = CreateRsa(publicKeyInfo);
                var valid = rsa.VerifyData(data, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                return valid;
            }
            default: throw new Exception($"Invalid public key algorithm {stored.PublicKeyAlgorithm}");
        }
    }

    private static ECDsa CreateEcdsa(byte[] publicKeyInfo)
    {
        var ecd = ECDsa.Create();
        ecd.ImportSubjectPublicKeyInfo(publicKeyInfo, out var l);

        if (l != publicKeyInfo.Length)
            throw new Exception("Invalid public key");

        return ecd;
    }

    private static RSA CreateRsa(byte[] publicKeyInfo)
    {
        var rsa = RSA.Create();
        rsa.ImportSubjectPublicKeyInfo(publicKeyInfo, out var l);

        if (l != publicKeyInfo.Length)
            throw new Exception("Invalid public key");

        return rsa;
    }

    private static (byte[] rpIdHash, byte flags, uint signCount) AuthenticatorData(byte[] authenticatorData)
    {
        // https://w3c.github.io/webauthn/#sctn-authenticator-data

        // SHA-256 hash of the RP ID the credential is scoped to.
        var rpIdHash = authenticatorData[..32];
        var flags = authenticatorData[32];
        // Signature counter, 32-bit unsigned big-endian integer.
        var signCount = SignCount(authenticatorData[33..37]);

        return (rpIdHash, flags, signCount);
    }

    private static uint SignCount(byte[] signCountBytes)
    {
        // Signature counter, 32-bit unsigned big-endian integer.
        if (BitConverter.IsLittleEndian)
            Array.Reverse(signCountBytes);

        var signCount = BitConverter.ToUInt32(signCountBytes);
        return signCount;
    }

    private static byte[] Challenge()
    {
        var bytes = new byte[32];
        Random.Shared.NextBytes(bytes);
        return bytes;
    }
}