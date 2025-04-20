// using UnityEngine;
// using System;
// using System.IO;
// using System.Collections.Generic;


// public static class EnvReader
// {
//     public static Dictionary<string, string> LoadEnv(string path)
//     {
//         var lines = File.ReadAllLines(path);
//         var dict = new Dictionary<string, string>();

//         foreach (var line in lines)
//         {
//             if (line.StartsWith("#") || string.IsNullOrWhiteSpace(line)) continue;

//             var parts = line.Split('=', 2);
//             if (parts.Length == 2)
//                 dict[parts[0].Trim()] = parts[1].Trim();
//         }

//         return dict;
//     }
// }

//CODIGO ANTIGO ACIMA

using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;

public static class EnvReader
{
    private static Dictionary<string, string> envVars;
    private static bool isLoaded = false;

    // Caminho padrão do arquivo .env
    private static string envPath = Path.Combine(Application.dataPath, "config", "env.env");

    public static void Load()
    {
        if (isLoaded) return;

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
                envVars[parts[0].Trim()] = parts[1].Trim();
        }

        isLoaded = true;
    }

    public static string Get(string key)
    {
        if (!isLoaded) Load();

        if (envVars != null && envVars.ContainsKey(key))
        {
            return envVars[key];
        }

        Debug.LogWarning($"Variável de ambiente '{key}' não encontrada.");
        return null;
    }
}
