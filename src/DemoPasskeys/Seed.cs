using DemoPasskeys.Repositories;

namespace DemoPasskeys;

public static class Seed
{
    public static void SeedUsers()
    {
        var data = new[]
        {
            new[] { "B9C0D", "daniel.wagner@example.com", "SicherPW2024$" },
            ["A1B2C", "anna.schmidt@example.com", "SicheresPW123!"],
            ["D3E4F", "max.muster@example.com", "GeheimPW456$"],
            ["G5H6I", "julia.meyer@example.com", "Passwort!789"],
            ["J7K8L", "peter.klein@example.com", "StarkesPW#321"],
            ["M9N0O", "sandra.bauer@example.com", "NeuesPW2024!"],
            ["P1Q2R", "thomas.schulz@example.com", "ZufallPW$123"],
            ["S3T4U", "maria.fischer@example.com", "KomplexPW!456"],
            ["V5W6X", "michael.weber@example.com", "EinzigartigPW#789"],
            ["Y7Z8A", "laura.hofmann@example.com", "GeheimesPW@2024"]
        };

        foreach (var raw in data)
        {
            var user = new UserModel(raw[0], raw[1], Password.Hash(raw[2]));
            UsersRepository.Write(user);
        }
    }
}