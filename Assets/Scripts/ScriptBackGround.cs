using UnityEngine;

public class ScriptBackGround : MonoBehaviour
{
    public GameObject backGround; //background reinderizado

    public string id = "1";//id do background mudar posterior mente para um sistema BD
 
    void Start()
    {
        backGround.GetComponent<BackGround>().name = id;
        backGround.GetComponent<BackGround>().Activate();
        Instantiate(backGround,new Vector3(0,0,-1), Quaternion.identity);
    }
    
}
