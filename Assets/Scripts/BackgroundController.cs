using UnityEngine;
using UnityEngine.SceneManagement;

public class BackgroundController : MonoBehaviour
{
    public static BackgroundController Instance { get; private set; }

    public GameObject backGround; //background reinderizado

    public string id = "1";//TODO: id do background mudar posterior mente para um sistema BD
    private static GameObject instantiatedBackground = null;

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
        if (scene.name != "Game")
        {
            //verifica se o background ainda não foi instanciado, instancia
            if (instantiatedBackground == null)
            {
                instantiatedBackground = Instantiate(backGround, new Vector3(0, 0, 0), Quaternion.identity);
                instantiatedBackground.name = id;
                BackGround bgComponent = instantiatedBackground.GetComponent<BackGround>();
                bgComponent.Activate();

                Debug.Log("Background instanciado na cena: " + scene.name);
            }
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

}
