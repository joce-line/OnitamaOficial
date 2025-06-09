using Assets.scripts.InfoPlayer;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.Networking;

public class BackgroundController : MonoBehaviour
{
    public static BackgroundController Instance { get; private set; }

    public GameObject backGround;

    private static GameObject instantiatedBackground = null;
    private string caminhoAtual = "";
    private Sprite spriteAtual = null;

    private void Awake()
    {
        if (SceneManager.GetActiveScene().name == "Game")
        {
            Destroy(gameObject);
            return;
        }

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

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Game")
        {
            if (instantiatedBackground != null)
            {
                Destroy(instantiatedBackground);
                instantiatedBackground = null;
            }
        }
        else
        {
            if (instantiatedBackground == null)
            {
                instantiatedBackground = Instantiate(backGround, new Vector3(0, 0, 1), Quaternion.identity);
                instantiatedBackground.name = "Background";
                DontDestroyOnLoad(instantiatedBackground);
            }
            AplicarBackground(); // <- só chama depois da instância criada
        }
    }

    private void AplicarBackground()
    {
        string caminho = PlayerInfo.caminho_Background;

        if (string.IsNullOrEmpty(caminho))
        {
            caminho = "Backgrounds/defaultBG";
        }

        StartCoroutine(LoadBackground(caminho));
    }

    public void AtualizarBackground(int idBackground, string caminho)
    {
        if (instantiatedBackground == null)
        {
            Debug.LogWarning("Instância do background ainda não existe.");
            return;
        }

        StartCoroutine(LoadBackground(caminho));
    }

    private IEnumerator LoadBackground(string url)
    {
        if (instantiatedBackground == null)
        {
            Debug.LogError("instantiatedBackground está nulo! Não é possível aplicar o background.");
            yield break;
        }

        if (string.IsNullOrEmpty(url))
        {
            //Debug.LogWarning("Caminho inválido ou vazio, usando defaultBG.");
            Sprite defaultSprite = Resources.Load<Sprite>("Backgrounds/defaultBG");
            instantiatedBackground.GetComponent<SpriteRenderer>().sprite = defaultSprite;
            yield break;
        }

        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
            instantiatedBackground.GetComponent<SpriteRenderer>().sprite = sprite;
            Debug.Log("Background carregado com sucesso!");
        }
        else
        {
            Debug.LogWarning("Erro ao carregar background remoto. Usando defaultBG.");
            Sprite defaultSprite = Resources.Load<Sprite>("Backgrounds/defaultBG");
            instantiatedBackground.GetComponent<SpriteRenderer>().sprite = defaultSprite;
        }

    }
}
