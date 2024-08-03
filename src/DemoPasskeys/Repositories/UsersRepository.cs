using System.Text.Json;

namespace DemoPasskeys.Repositories;

public static class UsersRepository
{
    private static string DataDirectory => Path.Combine(Configuration.DataDirectory, "Users");

    public static void Write(UserModel user)
    {
        Directory.CreateDirectory(DataDirectory);

        var filePath = GetFilePath(user.Id);
        var content = JsonSerializer.Serialize(user, RepositoryConfig.JsonOptions);
        File.WriteAllText(filePath, content);
    }

    public static UserModel? Read(string id)
    {
        var filePath = GetFilePath(id);
        return File.Exists(filePath) ? Deserialize(filePath) : null;
    }

    public static void Delete(string id)
    {
        var filePath = GetFilePath(id);
        File.Delete(filePath);
    }

    public static UserModel? FindByEmail(string email)
    {
        var files = Directory.GetFiles(DataDirectory);
        return files
            .Select(Deserialize)
            .FirstOrDefault(user => MatchEmail(user!, email));
    }

    private static bool MatchEmail(UserModel user, string email)
    {
        return string.Equals(user.EmailAddress, email, StringComparison.InvariantCultureIgnoreCase);
    }

    private static UserModel? Deserialize(string filePath)
    {
        var json = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<UserModel>(json, RepositoryConfig.JsonOptions);
    }

    private static string GetFilePath(string id)
    {
        return Path.Combine(DataDirectory, $"{id}.json");
    }
}