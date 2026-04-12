using UnityEngine;
using UnityEngine.SceneManagement;

public class CambiarEscena : MonoBehaviour
{
    // Nombre de la escena a la que quieres ir
    public string nombreEscena;

    // Método que se llamará desde el botón
    public void IrAEscena()
    {
        SceneManager.LoadScene(nombreEscena);
    }
}