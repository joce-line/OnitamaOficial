using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;

public class RankingManager : MonoBehaviour
{
    public TextMeshProUGUI ListaRankingPosicao;
    public TextMeshProUGUI ListaRankingNome;
    public TextMeshProUGUI ListaRankingVitoria;

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
                ListaRankingNome.text = "Nenhum dado de ranking encontrado.";
                return;
            }

            string rankingFinalPosicao = "";
            string rankingFinalNome = "";
            string rankingFinalVitoria = "";

            int posicao = 1;
            foreach (var jogador in resultados)
            {
                if (posicao > 5)
                {
                    break;
                }
                string nome = jogador["nickname"].ToString();
                int vitorias = int.Parse(jogador["vitorias"].ToString());

                rankingFinalPosicao += $"{posicao,3}º\n";
                rankingFinalNome += $"{nome,-15}\n";
                rankingFinalVitoria += $"{vitorias}\n";
                posicao++;
            }

            ListaRankingPosicao.text = rankingFinalPosicao;
            ListaRankingNome.text = rankingFinalNome;
            ListaRankingVitoria.text = rankingFinalVitoria;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Erro ao buscar ranking: {e.Message}");
            ListaRankingNome.text = "Erro ao carregar ranking.";
        }
    }
}
