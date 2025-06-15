using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    public AudioSource MusicBackground;

    public AudioClip MusicGeral;
    public AudioClip MusicBattle;
    public AudioClip MusicWin;
    public AudioClip MusicLose;

    private Scene currentScene;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
       if(!PlayerPrefs.HasKey("musicVolume"))
       {
           PlayerPrefs.SetFloat("musicVolume", 0.05f);
           MusicBackground.volume = PlayerPrefs.GetFloat("musicVolume");
       }
       else
       {
           MusicBackground.volume = PlayerPrefs.GetFloat("musicVolume");
       }
       instance.playMusicGeral();
       
       if(!PlayerPrefs.HasKey("soundVolume"))
       {
           PlayerPrefs.SetFloat("soundVolume", 0.05f);
       }
    }

    public void playMusicGeral()
    {        
        MusicBackground.resource = MusicGeral;
        MusicBackground.Play();        
    }

    public void playMusicBattle()
    {
        MusicBackground.resource = MusicBattle;
        MusicBackground.Play();
    }

    public void playMusicWin()
    {
        MusicBackground.resource = MusicWin;
        MusicBackground.Play();
    }

    public void playMusicLose()
    {
        MusicBackground.resource = MusicLose;
        MusicBackground.Play();
    }

    public void musicStop()
    {
        MusicBackground.Stop();
    }
}
