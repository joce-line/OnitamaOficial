using Assets.scripts.InfoPlayer;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackgroundController : MonoBehaviour
{
    public static BackgroundController Instance { get; private set; }

    public GameObject backGround;

    public Sprite[] backgroundSprites;

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

        //LoadUserBackground();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "Game")
        {
            if (instantiatedBackground == null)
            {
                string set_backgroung = string.IsNullOrEmpty(PlayerInfo.id_Background) ? "0" : PlayerInfo.id_Background;

                instantiatedBackground = Instantiate(backGround, Vector3.zero, Quaternion.identity);
                instantiatedBackground.name = set_backgroung;

                BackGround bgComponent = instantiatedBackground.GetComponent<BackGround>();

                if (int.TryParse(set_backgroung, out int selectedIndex) &&
                    selectedIndex >= 0 && selectedIndex < backgroundSprites.Length)
                {
                    bgComponent.SetSprite(backgroundSprites[selectedIndex]);
                }
                else
                {
                    Debug.LogWarning("ID inválido para background: " + set_backgroung);
                }

                bgComponent.Activate();

                Debug.Log("Background instanciado na cena: " + scene.name);
            }
        }
    }
    public void AtualizarBackground(int novoId)
    {
        if (instantiatedBackground == null)
            return;

        BackGround bgComponent = instantiatedBackground.GetComponent<BackGround>();

        if (novoId >= 0 && novoId < backgroundSprites.Length)
        {
            bgComponent.SetSprite(backgroundSprites[novoId]);
            Debug.Log("Background atualizado em tempo real.");
        }
        else
        {
            Debug.LogWarning("ID de background inválido para atualização.");
        }
    }


    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
