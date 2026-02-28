using System;
using System.IO;
using System.Collections.Generic;

namespace LLTC.Utils;

public static class ConfigManager
{
    public const string CONFIG_FILE_NAME = "config.ini";

    private static string? configPath = null;

    public static string? ConfigPath { get { return configPath; } }

    public static void Init()
    {
        configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, CONFIG_FILE_NAME);

        if (!File.Exists(configPath))
            File.Create(configPath).Close();
    }

    public static Dictionary<string, string> Read(string? path = "default")
    {
        if (path != null && path.Equals("default"))
            path = configPath;

        if (path != null)
        {
            var output = new Dictionary<string, string>();
            string content = string.Empty;

            using (StreamReader sR = new (path))
                content = sR.ReadToEnd();

            foreach (string line in content.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                string[] parts = line.Split('=', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                if (parts.Length == 2)
                    output.Add(parts[0], parts[1]);
            }

            return output;
        }
        else
            throw new NotInitializedException("Config manager is not initiliazed!");
    }

    public static void Write(Dictionary<string, string> data, string? path = "default")
    {
        if (path != null && path.Equals("default"))
            path = configPath;

        if (path != null)
        {
            string content = string.Empty;

            foreach (KeyValuePair<string, string> pair in data)
                content += $"{pair.Key}={pair.Value}\n";

            using (StreamWriter sW = new (path))
                sW.Write(content);
        }
        else
            throw new NotInitializedException("Config manager is not initiliazed!");
    }
}