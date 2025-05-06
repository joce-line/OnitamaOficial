using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScriptTelaTitulo : MonoBehaviour
{
	public void Update()
	{
		if (Input.GetMouseButtonDown(0)) SceneManager.LoadScene("Login");
	}
}

