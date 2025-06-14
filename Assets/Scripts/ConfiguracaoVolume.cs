using UnityEngine;
using UnityEngine.UI;

public class ConfiguracaoVolume : MonoBehaviour
{
    //configuração de volume
    public AudioSource backGroundMusic;
    public AudioSource Sound;
    public AudioClip testSound;

    public Slider barMusic;
    public Slider barSound;

    void Start()
    {        
        loadMusicVolume();
        loadSoundVolume();
    }

    //volume da musica
    public void changeVolumeMusic()
    {
        backGroundMusic = GameObject.Find("MusicBackGround").GetComponent<AudioSource>();
        backGroundMusic.volume = barMusic.value;
        saveMusicVolume();
    }

    public void saveMusicVolume()
    {
         PlayerPrefs.SetFloat("musicVolume", barMusic.value);
    }

    public void loadMusicVolume()
    {
         barMusic.value = PlayerPrefs.GetFloat("musicVolume");
    }

    //volume dos sons de partida
    public void changeVolumeSound()    
    {        
        saveSoundVolume();
    }

    public void soundTest()
    {
        Sound.PlayOneShot(testSound, barSound.value);
    }

    public void saveSoundVolume()
    {
         PlayerPrefs.SetFloat("soundVolume", barSound.value);
    }

    public void loadSoundVolume()
    {
         barSound.value = PlayerPrefs.GetFloat("soundVolume");
    }
}
