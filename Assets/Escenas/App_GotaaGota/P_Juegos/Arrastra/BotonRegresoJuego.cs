using UnityEngine;
using UnityEngine.SceneManagement;

public class BotonRegresoJuego : MonoBehaviour
{
    // Nombre exacto de tu escena de menú
    public string nombreEscenaMenu = "Menu";

    public void VolverAlMenuJuegos()
    {
        // 1. Le avisamos al script del Menú que queremos abrir la pestaña de juegos
        // Usamos la variable estática que acabamos de crear
        NavegacionMenuPrincipal.panelAbridor = "juegos";

        // 2. Cargamos la escena
        SceneManager.LoadScene(nombreEscenaMenu);
    }
}