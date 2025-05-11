using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System;
using UnityEngine;

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager Instance { get; private set; }
    public static string ConnectionString { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        EnvReader.Load();
        string server = EnvReader.Get("DB_SERVER", true);
        string db = EnvReader.Get("DB_DATABASE", true);
        string user = EnvReader.Get("DB_USER", true);
        string password = EnvReader.Get("DB_PASSWORD", true);
        string sslmode = EnvReader.Get("DB_SSLMODE", true);

        if (server == null || db == null || user == null || password == null || sslmode == null)
        {
            Debug.LogError("Falha ao carregar variáveis do .env. Algum valor nulo.");
            return;
        }

        ConnectionString = $"Server={server};Database={db};Uid={user};Pwd={password};SslMode={sslmode};Pooling=true;Max Pool Size=7;";
        Debug.Log("DatabaseManager inicializado com sucesso.");

    }

    // Método para executar consultas que não retornam dados (INSERT, UPDATE, DELETE, CREATE, ALTER, DROP)
    public bool ExecuteNonQuery(string query, Dictionary<string, object> parameters = null)
    {
        using (MySqlConnection conn = new MySqlConnection(ConnectionString))
        {
            try
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            cmd.Parameters.AddWithValue(param.Key, param.Value);
                        }
                    }
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Erro ao executar query: {e.Message}");
                return false;
            }
        }
    }

    // Método para consultas que retornam um valor único (SELECT, COUNT, SHOW, DESCRIBE, EXPLAIN)
    public object ExecuteScalar(string query, Dictionary<string, object> parameters = null)
    {
        using (MySqlConnection conn = new MySqlConnection(ConnectionString))
        {
            try
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            cmd.Parameters.AddWithValue(param.Key, param.Value);
                        }
                    }
                    return cmd.ExecuteScalar();
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Erro ao executar scalar: {e.Message}");
                return -1;
            }
        }
    }

    // Método para consultas que retornam mais de um valor
    public List<Dictionary<string, object>> ExecuteReader(string query, Dictionary<string, object> parameters = null)
    {
        List<Dictionary<string, object>> results = new List<Dictionary<string, object>>();

        using (MySqlConnection conn = new MySqlConnection(ConnectionString))
        {
            try
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            cmd.Parameters.AddWithValue(param.Key, param.Value);
                        }
                    }

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Dictionary<string, object> row = new Dictionary<string, object>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                row.Add(reader.GetName(i), reader.GetValue(i));
                            }
                            results.Add(row);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Erro ao executar reader: {e.Message}");
            }
        }

        return results;
    }
}