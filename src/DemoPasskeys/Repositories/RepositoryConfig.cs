using System.Text.Json;

namespace DemoPasskeys.Repositories;

internal static class RepositoryConfig
{
    public static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };
}