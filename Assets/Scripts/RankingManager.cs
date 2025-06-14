using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;

public class Ranking : MonoBehaviour
{
    public TextMeshProUGUI ListaRanking;
    public void sairRanking()
    {
        SceneManager.LoadScene("MenuPrincipal");
    }
    void Start()
    {
        consultaRanking();
    }

    public void consultaRanking()
    {
        try
        {
            string selectVitoriaNome = "SELECT nickname, vitorias FROM usuarios ORDER BY vitorias DESC";
            List<Dictionary<string, object>> resultados = DatabaseManager.Instance.ExecuteReader(selectVitoriaNome);

            if (resultados == null || resultados.Count == 0)
            {
                ListaRanking.text = "Nenhum dado de ranking encontrado.";
                return;
            }

            string rankingFinal = "";

            int posicao = 1;
            foreach (var jogador in resultados)
            {
                if (posicao > 5)
                {
                    break;
                }
                string nome = jogador["nickname"].ToString();
                int vitorias = int.Parse(jogador["vitorias"].ToString());

                rankingFinal += $"{posicao}º - {vitorias} vitória(s) - {nome}\n";
                posicao++;
            }

            ListaRanking.text = rankingFinal;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Erro ao buscar ranking: {e.Message}");
            ListaRanking.text = "Erro ao carregar ranking.";
        }
    }
}
