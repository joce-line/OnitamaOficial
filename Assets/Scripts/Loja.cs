using UnityEngine;
using Assets.scripts.InfoPlayer;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;

public class Loja : MonoBehaviour
{
    [Header("Referencias na cena")]
    public Transform contentParent;
    public GameObject skinPrefab;
    public GameObject modalConfirmacao;
    //paineis de compras na loja
    public GameObject LojaItem, LojaBackground, LojaPoderes, LojaCoin;

    private int idItemSelecionado;

    public static Loja instance;

    public static Loja GetInstance()
    {
        if (instance == null)
        {
            instance = (Loja)FindFirstObjectByType(typeof(Loja));
        }
        return instance;
    }

    public void Awake()
    {
        if (instance == null)
        {
            instance = (Loja)FindFirstObjectByType(typeof(Loja));
            if (instance == null)
            {
                instance = this;
            }
        }
    }

    void Start()
    {
        if (DatabaseManager.Instance == null || string.IsNullOrEmpty(DatabaseManager.ConnectionString))
        {
            Debug.LogError("DatabaseManager n�o inicializado.");
            return;
        }

        CreateItems();
    }

    public static void CreateItems()
    {
        string query = "SELECT id_Conjunto, nome_Conjunto, preco, caminho_Pawn, caminho_King FROM conjuntos_skins WHERE active = 1";
        List<Dictionary<string, object>> results = DatabaseManager.Instance.ExecuteReader(query);

        int idUsuario = PlayerInfo.idPlayer;

        foreach (var linha in results)
        {
            int id = Convert.ToInt32(linha["id_Conjunto"]);
            string nome = linha["nome_Conjunto"].ToString();
            int preco = Convert.ToInt32(linha["preco"]);
            string caminhoPawn = linha["caminho_Pawn"].ToString();
            string caminhoKing = linha["caminho_King"].ToString();

            // Verifica se o usu�rio j� comprou esse item
            string queryCheck = $"SELECT COUNT(*) FROM skins_usuario WHERE id_usuario = {idUsuario} AND id_conjunto = {id}";
            int count = Convert.ToInt32(DatabaseManager.Instance.ExecuteScalar(queryCheck));
            bool jaComprado = count > 0;

            instance.CreateItem(id, nome, preco, caminhoPawn, caminhoKing, jaComprado);

            //Debug.Log($"ID: {id}, Nome: {nome}, Pre�o: {preco}, J� Comprado: {jaComprado}");
        }
    }


    public void CreateItem(int id, string nome, int preco, string caminhoPawn, string caminhoKing, bool jaComprado)
    {
        GameObject temp = Instantiate(skinPrefab, contentParent);
        SkinItem skinItemScript = temp.GetComponent<SkinItem>();

        if (skinItemScript != null)
        {
            skinItemScript.ConfigurarItem(id, nome, preco, caminhoPawn, caminhoKing, jaComprado);
        }
        else
        {
            Debug.LogError("Prefab n�o cont�m o script SkinItem.");
        }
    }

    public void ConfirmarCompra(int id)
    {
        idItemSelecionado = id;
        modalConfirmacao.SetActive(true);
    }

    public void OnConfirmarCompra()
    {
        ComprarItem(idItemSelecionado);
        modalConfirmacao.SetActive(false);
    }

    public void OnCancelarCompra()
    {
        modalConfirmacao.SetActive(false);
    }

    public void ComprarItem(int idItem)
    {
        int idUsuario = PlayerInfo.idPlayer;
        string insertQuery = $"INSERT INTO skins_usuario (id_usuario, id_conjunto) VALUES ({idUsuario}, {idItem})";
        DatabaseManager.Instance.ExecuteNonQuery(insertQuery);

        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }
        CreateItems();
    }

    public void VoltarButton()
    {
        SceneManager.LoadScene("MenuPrincipal");
    }


    //função de lojas
    public void AtvLojaItem()
    {
        LojaItem.SetActive(true);
        LojaBackground.SetActive(false);
        LojaPoderes.SetActive(false);
        LojaCoin.SetActive(false);
    }

    public void AtvLojaBackGround()
    {
        LojaItem.SetActive(false);
        LojaBackground.SetActive(true);
        LojaPoderes.SetActive(false);
        LojaCoin.SetActive(false);
    }

    public void AtvLojaPoderes()
    {
        LojaItem.SetActive(false);
        LojaBackground.SetActive(false);
        LojaPoderes.SetActive(true);
        LojaCoin.SetActive(false);
    }

    public void AtvLojaCoins()
    {
        LojaItem.SetActive(false);
        LojaBackground.SetActive(false);
        LojaPoderes.SetActive(false);
        LojaCoin.SetActive(true);
    }
}
