using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SonsDeProvocacao : MonoBehaviour
{

    public AudioSource tauntSom;

    public AudioClip okSom;
    public AudioClip risoSom;
    public AudioClip choroSom;
    public AudioClip raivaSom;

    public float clipVolume = .5f;

    public void playOkSom()
    {
        tauntSom.PlayOneShot(okSom,PlayerPrefs.GetFloat("soundVolume"));
    }

    public void playRisoSom()
    {
         tauntSom.PlayOneShot(risoSom,PlayerPrefs.GetFloat("soundVolume"));
    }

    public void playChoroSom()
    {
         tauntSom.PlayOneShot(choroSom,PlayerPrefs.GetFloat("soundVolume"));
    }

    public void playRaivaSom()
    {
         tauntSom.PlayOneShot(raivaSom,PlayerPrefs.GetFloat("soundVolume"));
    }
}
