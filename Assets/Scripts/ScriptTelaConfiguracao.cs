using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Assets.scripts.InfoPlayer;
using System.Collections.Generic;

public class ScriptTelaConfiguracao : MonoBehaviour
{
    public TMP_Dropdown dropdownTemas;

    public void voltarButton()
    {
        SceneManager.LoadScene("MenuPrincipal");
    }

    public void confirmarButton()
    {
        int valorSelecionado = dropdownTemas.value + 1;
        Debug.Log("ID do tema selecionado: " + valorSelecionado);

        PlayerInfo.id_Background = valorSelecionado.ToString();

        BackgroundController.Instance?.AtualizarBackground(valorSelecionado);

        // Verifica se a conexão está ativa
        if (DatabaseManager.Instance == null || string.IsNullOrEmpty(DatabaseManager.ConnectionString))
        {
            Debug.LogError("DatabaseManager não inicializado.");
            return;
        }

        // Verifica se a conexão está ativa
        if (DatabaseManager.Instance == null || string.IsNullOrEmpty(DatabaseManager.ConnectionString))
        {
            Debug.LogError("DatabaseManager não inicializado.");
            return;
        }

        // Atualiza no banco de dados
        string updateQuery = "UPDATE usuarios SET id_Background = @bg WHERE idUsuario = @id";
        var parametros = new Dictionary<string, object>
        {
            { "@bg", valorSelecionado },
            { "@id", PlayerInfo.idPlayer }
        };

        if (DatabaseManager.Instance.ExecuteNonQuery(updateQuery, parametros))
        {
            Debug.Log("Backgroung do usuario salvo com sucesso.");
        }
        else
        {
            Debug.LogError("Erro ao salvar background do usuario.");
        }

    }
}
