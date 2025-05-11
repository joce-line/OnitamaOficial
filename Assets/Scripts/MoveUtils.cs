using System.Collections;
using UnityEngine;

public class MoveUtils
{
    //Função para mover suavemente o go do ponto Start ao ponto End durante um tempo
    //IEnumerator = leitura de dados um a um (executa ao longo de varios frames)
    public static IEnumerator SmoothLerp(float time, Vector3 start, Vector3 end, GameObject go, bool OnitamaCard = false)
    {
        float elapsedTime = 0;

        start.z = 0;
        end.z = OnitamaCard ? -5 : -1;

        //Calcula a posição intermediária entre start e end
        while (elapsedTime < time)
        {
            Vector3 newPos = Vector3.Lerp(start, end, (elapsedTime / time));
            newPos.z = OnitamaCard ? -5 : -1; ;
            go.transform.position = newPos;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Garante que chegue exatamente no final
        go.transform.position = end;
    }

    // TODO: Rotação suave **talvez mudar apos testes com cartas**
    public static IEnumerator SmoothRotate(float time, Quaternion target, GameObject go)
    {
        float elapsedTime = 0;
        Quaternion startRotation = go.transform.rotation;

        while (elapsedTime < time)
        {
            go.transform.rotation = Quaternion.Slerp(startRotation, target, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        go.transform.rotation = target;
    }
}
