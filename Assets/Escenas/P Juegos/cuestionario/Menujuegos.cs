using UnityEngine;
using UnityEngine.SceneManagement;

public class BotonRegresarAJuegos : MonoBehaviour
{
    // Este método lo asignas al botón en la escena "Cuestionario"
    public void IrAJuegos()
    {
        // Cargar la escena "Menu"
        SceneManager.LoadScene("Menu");

        // IMPORTANTE: como los objetos de la jerarquía solo existen
        // después de cargar la escena, necesitamos un "callback"
        // para activar el panel "Juegos" cuando la escena termine de cargar.
        SceneManager.sceneLoaded += ActivarJuegos;
    }

    private void ActivarJuegos(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Menu")
        {
            // Buscar el objeto "Juegos" en la jerarquía
            GameObject juegosPanel = GameObject.Find("Juegos");
            if (juegosPanel != null)
            {
                // Activar el panel
                juegosPanel.SetActive(true);
            }

            // Muy importante: quitar el callback para que no se ejecute cada vez
            SceneManager.sceneLoaded -= ActivarJuegos;
        }
    }
}
