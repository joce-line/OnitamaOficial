using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SonsDeProvocacao : MonoBehaviourPun
{

    public AudioSource tauntSom;

    public AudioClip okSom;
    public AudioClip risoSom;
    public AudioClip choroSom;
    public AudioClip raivaSom;

    public float clipVolume = .5f;
    //RPCs para cada som de provocação
    [PunRPC]
    public void RpcplayOkSom()
    {
        float volume = PlayerPrefs.GetFloat("soundVolume", clipVolume);
        tauntSom.PlayOneShot(okSom,volume);
    }

    [PunRPC]
    public void RpcplayRisoSom()
    {
        float volume = PlayerPrefs.GetFloat("soundVolume", clipVolume);
        tauntSom.PlayOneShot(risoSom,volume);
    }

    [PunRPC]
    public void RpcplayChoroSom()
    {
        float volume = PlayerPrefs.GetFloat("soundVolume", clipVolume);
        tauntSom.PlayOneShot(choroSom,volume);
    }

    [PunRPC]
    public void RpcplayRaivaSom()
    {
        float volume = PlayerPrefs.GetFloat("soundVolume", clipVolume);
        tauntSom.PlayOneShot(raivaSom,volume);
    }

    //Métodos públicos para acionar os RPCs
    public void PlayOkSom()
    {
        photonView.RPC("RpcplayOkSom", RpcTarget.All);
    }

    public void PlayRisoSom()
    {
        photonView.RPC("RpcplayRisoSom", RpcTarget.All);
    }

    public void PlayChoroSom()
    {
        photonView.RPC("RpcplayChoroSom", RpcTarget.All);
    }

    public void PlayRaivaSom()
    {
        photonView.RPC("RpcplayRaivaSom", RpcTarget.All);
    }
}
