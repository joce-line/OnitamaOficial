using UnityEngine;
using Assets.scripts.InfoPlayer;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;

[System.Serializable]
public class PacoteMoeda
{
    public int quantidade;
    public float preco;
    public Sprite imagem;
}

public class Loja : MonoBehaviour
{
    [Header("Referências na Cena")]
    public Transform contentParent; // Para skins
    public GameObject skinPrefab;
    public GameObject modalConfirmacao;
    public GameObject modalConfirmacaoCompra;
    public GameObject modalConfirmacaoMoeda;

    public GameObject LojaItem;
    public GameObject LojaCoin;

    [Header("Moedas")]
    public Transform contentMoedasParent; // Scroll View Moedas > Content
    public GameObject moedaPrefab;
    public List<PacoteMoeda> pacotesMoedas;

    private int idItemSelecionado;
    private int quantidadeMoedaSelecionada;

    private static bool itemsCreated = false;
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

        AtvLojaItem();
    }

    void Start()
    {
        if (DatabaseManager.Instance == null || string.IsNullOrEmpty(DatabaseManager.ConnectionString))
        {
            Debug.LogError("DatabaseManager não inicializado.");
            return;
        }
    }

    // ------------------- SKINS -------------------

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

            string queryCheck = $"SELECT COUNT(*) FROM skins_usuario WHERE id_usuario = {idUsuario} AND id_conjunto = {id}";
            int count = Convert.ToInt32(DatabaseManager.Instance.ExecuteScalar(queryCheck));
            bool jaComprado = count > 0;

            instance.CreateItem(id, nome, preco, caminhoPawn, caminhoKing, jaComprado);
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
            Debug.LogError("Prefab de skin não contém o script SkinItem.");
        }
    }

    public void ConfirmarCompra(int id)
    {
        idItemSelecionado = id;
        modalConfirmacao.SetActive(true);
    }

    public void OnConfirmarCompra()
    {
        Debug.Log("on confirmar compra");
        ComprarItem(idItemSelecionado);
        modalConfirmacao.SetActive(false);
    }

    public void OnCancelarCompra()
    {
        modalConfirmacao.SetActive(false);
    }

    public void OnCancelarCompraMoedas()
    {
        modalConfirmacaoMoeda.SetActive(false);
    }

    public void OnMoedasInsuficientes()
    {
        modalConfirmacaoCompra.SetActive(false);
    }

    public void ComprarItem(int idItem)
    {
        int idUsuario = PlayerInfo.idPlayer;

        string queryPreco = $"SELECT preco FROM conjuntos_skins WHERE id_Conjunto = {idItem}";
        int preco = Convert.ToInt32(DatabaseManager.Instance.ExecuteScalar(queryPreco));

        string queryMoedas = $"SELECT moedas FROM usuarios WHERE idUsuario = {idUsuario}";
        int moedasAtuais = Convert.ToInt32(DatabaseManager.Instance.ExecuteScalar(queryMoedas));

        if (moedasAtuais >= preco)
        {
            int novasMoedas = moedasAtuais - preco;

            string updateMoedasQuery = $"UPDATE usuarios SET moedas = {novasMoedas} WHERE idUsuario = {idUsuario}";
            DatabaseManager.Instance.ExecuteNonQuery(updateMoedasQuery);

            string insertQuery = $"INSERT INTO skins_usuario (id_Usuario, id_Conjunto) VALUES ({idUsuario}, {idItem})";
            DatabaseManager.Instance.ExecuteNonQuery(insertQuery);

            // Atualiza PlayerInfo.moeda e UI
            string queryAtualizaMoeda = $"SELECT moedas FROM usuarios WHERE idUsuario = {idUsuario}";
            PlayerInfo.moeda = Convert.ToInt32(DatabaseManager.Instance.ExecuteScalar(queryAtualizaMoeda));
            FindFirstObjectByType<InformacoesMoedas>().AtualizaMoedas();

            foreach (Transform child in contentParent)
            {
                Destroy(child.gameObject);
            }
            CreateItems();
        }
        else
        {
            Debug.Log("Moedas insuficientes!");
            modalConfirmacaoCompra.SetActive(true);
        }
    }

    // ------------------- MOEDAS -------------------

    public void CriarPacotesMoedas()
    {
        foreach (Transform child in contentMoedasParent)
        {
            Destroy(child.gameObject);
        }

        foreach (PacoteMoeda pacote in pacotesMoedas)
        {
            GameObject temp = Instantiate(moedaPrefab, contentMoedasParent);
            MoedaPacoteItem script = temp.GetComponent<MoedaPacoteItem>();

            if (script != null)
            {
                script.ConfigurarMoeda(pacote.quantidade, $"{pacote.quantidade} moedas", pacote.preco, pacote.imagem);
            }
            else
            {
                Debug.LogWarning("Prefab moeda está sem o script MoedaPacoteItem.");
            }
        }
    }

    public void ConfirmarCompraMoeda(int quantidade)
    {
        quantidadeMoedaSelecionada = quantidade;
        modalConfirmacaoMoeda.SetActive(true);
    }

    public void OnConfirmarCompraMoeda()
    {
        ComprarPacoteMoeda(quantidadeMoedaSelecionada);
        modalConfirmacaoMoeda.SetActive(false);
    }

    public void ComprarPacoteMoeda(int quantidade)
    {
        int idUsuario = PlayerInfo.idPlayer;

        string updateQuery = $"UPDATE usuarios SET moedas = moedas + {quantidade} WHERE idUsuario = {idUsuario}";
        DatabaseManager.Instance.ExecuteNonQuery(updateQuery);

        // Atualiza PlayerInfo.moeda e UI
        string queryAtualizaMoeda = $"SELECT moedas FROM usuarios WHERE idUsuario = {idUsuario}";
        PlayerInfo.moeda = Convert.ToInt32(DatabaseManager.Instance.ExecuteScalar(queryAtualizaMoeda));
        FindFirstObjectByType<InformacoesMoedas>().AtualizaMoedas();

        Debug.Log($"Comprado pacote de {quantidade} moedas.");
    }

    // ------------------- UI Navegação -------------------

    public void VoltarButton()
    {
        SceneManager.LoadScene("MenuPrincipal");
    }

    public void AtvLojaItem()
    {
        LojaItem.SetActive(true);
        LojaCoin.SetActive(false);

        if (!itemsCreated)
        {
            CreateItems();
            itemsCreated = true;
        }
    }

    public void AtvLojaCoins()
    {
        LojaItem.SetActive(false);
        LojaCoin.SetActive(true);

        CriarPacotesMoedas();
    }

    void OnDestroy()
    {
        itemsCreated = false;
    }
}
