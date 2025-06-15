using UnityEngine;

public class AbrirPDFDrive : MonoBehaviour
{
    public string googleDrivePDF = "https://drive.google.com/file/d/1SSgkmA9jHpCUZCqvXkuROUrmITU_qAyU/view?usp=sharing";

    public void AbrirPDF()
    {
        Application.OpenURL(googleDrivePDF);
    }
}
