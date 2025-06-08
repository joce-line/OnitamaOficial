using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Credito : MonoBehaviour
{
    public void Update()
	{
		if (Input.GetMouseButtonDown(0)) SceneManager.LoadScene("MenuPrincipal");
	}
}
