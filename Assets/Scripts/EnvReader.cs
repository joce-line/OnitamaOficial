using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public static class EnvReader
{
    private static Dictionary<string, string> envVars;
    private static bool isLoaded = false;

    // Caminho padrão do arquivo .env
    private static string GetEnvPath()
    {
        return Path.Combine(Application.streamingAssetsPath, ".env");
    }

    public static void Load()
    {
        if (isLoaded) return;

        string envPath = GetEnvPath();
        if (!File.Exists(envPath))
        {
            Debug.LogError("Arquivo .env não encontrado em: " + envPath);
            return;
        }

        var lines = File.ReadAllLines(envPath);
        envVars = new Dictionary<string, string>();

        foreach (var line in lines)
        {
            if (line.StartsWith("#") || string.IsNullOrWhiteSpace(line)) continue;

            var parts = line.Split('=', 2);
            if (parts.Length == 2)
            {
                string key = parts[0].Trim();
                string value = parts[1].Trim();
                if (value.StartsWith("\"") && value.EndsWith("\""))
                    value = value.Substring(1, value.Length - 2);
                envVars[key] = value;
            }
        }

        isLoaded = true;
    }

    public static string Get(string key, bool required = false)
    {
        if (!isLoaded) Load();

        if (envVars != null && envVars.ContainsKey(key))
        {
            return envVars[key];
        }

        if (required)
        {
            Debug.LogError($"Variável obrigatória '{key}' não encontrada.");
            return null;
        }

        Debug.LogWarning($"Variável de ambiente '{key}' não encontrada.");
        return null;
    }
}
