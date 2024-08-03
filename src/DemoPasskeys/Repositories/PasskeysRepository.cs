using System.Text.Json;

namespace DemoPasskeys.Repositories;

public static class PasskeysRepository
{
    private static string DataDirectory => Path.Combine(Configuration.DataDirectory, "Passkeys");

    public static void Write(StorePasskeyModel data)
    {
        Directory.CreateDirectory(DataDirectory);

        var filePath = GetFilePath(data.Id);
        var json = JsonSerializer.Serialize(data, RepositoryConfig.JsonOptions);
        File.WriteAllText(filePath, json);
    }

    public static StorePasskeyModel? Read(string id)
    {
        var filePath = GetFilePath(id);
        if (!File.Exists(filePath)) return null;

        var json = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<StorePasskeyModel>(json, RepositoryConfig.JsonOptions);
    }

    private static string GetFilePath(string id)
    {
        return Path.Combine(DataDirectory, $"{id}.json");
    }
}