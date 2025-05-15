using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScriptLoja : MonoBehaviour
{
	//variáveis para referenciar os painéis existentes na tela
	public GameObject lojaSkins;
	public GameObject lojaBackGround;
	public GameObject lojaPoder;

	//variavél para criação instantanea deskins na tela
	public GameObject itemSkin;
	public Dictionary<int,string> nomeSkin = new Dictionary<int,string>()
	{
		{1,"Amarelo"},
		{2,"Azul"},
		{3,"Laranja"},
		{4,"Verde"},
	};

	public void mostraSkin()
	{
		/*
		IDictionaryEnumerator dictEnumaretor = nomeSkin.GetEnumerator();
		while (dictEnumaretor.MoveNext())
		{
			GameObject ljobj = Instantiate(itemSkin, lojaSkins);
			ljobj.GetComponent<PeaoSprite>().GetComponent<SpriteRenderer>().sprite = Sprites.Load()
		}
		*/
	}

	//funcões de botões
	public void onClickSkin()
	{
		lojaSkins.SetActive(true);
		lojaBackGround.SetActive(false);
		lojaPoder.SetActive(false);
	}

	public void onClickBG()
	{
		lojaSkins.SetActive(false);
		lojaBackGround.SetActive(true);
		lojaPoder.SetActive(false);
	}

	public void onClickPoder()
	{
		lojaSkins.SetActive(false);
		lojaBackGround.SetActive(false);
		lojaPoder.SetActive(true);
	}

	public void OnClickSair()
	{
		SceneManager.LoadScene("MenuPrincipal");
	}
}
