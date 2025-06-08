using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class MusicMananger : MonoBehaviour
{
    public static MusicMananger instance;

    public AudioSource MusicBackground;
    public AudioSource MusicBattle;
    public AudioSource MusicWin;
    public AudioSource MusicLose;

    private Scene currentScene;

    void Awake()
    {
        if(instance == null)
        {
            instance = (MusicMananger)FindObjectOfType(typeof(MusicMananger));
            if(instance == null)
            {
                instance = this;
            }
        }
        DontDestroyOnLoad(this.gameObject);
        currentScene = SceneManager.GetActiveScene();
        instance.playMusicGeral();
    }

    public void playMusicGeral()
    {
        if(MusicBattle.isPlaying)
        {
            MusicBattle.Stop();
        }
        
        if(MusicWin.isPlaying)
        {
            MusicWin.Stop();
        }

        if(MusicLose.isPlaying)
        {
            MusicLose.Stop();
        }
        
        if(!MusicBackground.isPlaying)
        {
            MusicBackground.Play();
        }        
    }

    public void playMusicBattle()
    {
        if(!MusicBattle.isPlaying)
        {
            MusicBattle.Play();
        }
        
        if(MusicWin.isPlaying)
        {
            MusicWin.Stop();
        }

        if(MusicLose.isPlaying)
        {
            MusicLose.Stop();
        }
        
        if(MusicBackground.isPlaying)
        {
            MusicBackground.Stop();
        }        
    }

    public void playMusicWin()
    {
        if(MusicBattle.isPlaying)
        {
            MusicBattle.Stop();
        }
        
        if(!MusicWin.isPlaying)
        {
            MusicWin.Play();
        }

        if(MusicLose.isPlaying)
        {
            MusicLose.Stop();
        }
        
        if(MusicBackground.isPlaying)
        {
            MusicBackground.Stop();
        }        
    }

    public void playMusicLose()
    {
        if(MusicBattle.isPlaying)
        {
            MusicBattle.Stop();
        }
        
        if(MusicWin.isPlaying)
        {
            MusicWin.Stop();
        }

        if(!MusicLose.isPlaying)
        {
            MusicLose.Play();
        }
        
        if(MusicBackground.isPlaying)
        {
            MusicBackground.Stop();
        }        
    }

    public void musicStop()
    {
        MusicBattle.Stop();
        MusicBackground.Stop();       
    }
}
