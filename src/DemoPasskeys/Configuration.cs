namespace DemoPasskeys;

public static class Configuration
{
    public static string DataDirectory => GetEnv("DATA_DIRECTORY");

    private static string GetEnv(string dataDirectory)
    {
        var value = Environment.GetEnvironmentVariable(dataDirectory);
        if (string.IsNullOrWhiteSpace(value))
            throw new Exception($"Environment variable {dataDirectory} is not set");

        return value;
    }
}