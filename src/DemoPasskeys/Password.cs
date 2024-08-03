using System.Security.Cryptography;
using System.Text;

namespace DemoPasskeys;

public static class Password
{
    public static string Hash(string plain)
    {
        var bytes = Encoding.UTF8.GetBytes(plain);
        var hash = SHA256.HashData(bytes);
        var base64 = Convert.ToBase64String(hash);

        return base64;
    }

    public static bool Verify(string plain, string hash)
    {
        var expected = Hash(plain);
        return string.Equals(expected, hash);
    }
}