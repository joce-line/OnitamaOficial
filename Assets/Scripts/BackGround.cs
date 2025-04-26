using UnityEngine;

public class BackGround : MonoBehaviour
{
    public Sprite bg00, bg01;//variaveis de Sprite background

    public void Activate()
    {
        switch(this.name)
        {
            case "0": this.GetComponent<SpriteRenderer>().sprite = bg00;break;
            case "1": this.GetComponent<SpriteRenderer>().sprite = bg01;break;
        }
        this.transform.position = new Vector3(0,0,1);
    }

}
