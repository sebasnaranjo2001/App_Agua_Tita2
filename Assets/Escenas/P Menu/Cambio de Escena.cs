using UnityEngine;
using UnityEngine.SceneManagement;

public class CambiarEscena : MonoBehaviour
{
    [Header("Configuración")]
    public string nombreEscena;

    // --- FUNCIÓN 1: La de siempre (Para botones de Inicio, Volver, etc.) ---
    // Esta NO revisa nada, solo cambia la escena directamente.
    public void IrAEscena()
    {
        if (!string.IsNullOrEmpty(nombreEscena))
        {
            SceneManager.LoadScene(nombreEscena);
        }
    }

    // --- FUNCIÓN 2: La especial (SOLO para el botón Empezar) ---
    // Esta REVISA si hay un nombre seleccionado antes de avanzar.
    public void IrAEscenaSiHayMiembro()
    {
        // 1. Verificamos que el Manejador de Registro exista
        if (ManejadorRegistro.instance != null)
        {
            // 2. ¿Hay alguien seleccionado?
            if (!string.IsNullOrEmpty(ManejadorRegistro.instance.nombreSeleccionado))
            {
                // Si hay alguien, cargamos la escena que pusiste en el Inspector
                SceneManager.LoadScene(nombreEscena);
            }
            else
            {
                // Si no hay nadie, no hace nada. 
                // Tu script de "Avisos" se encargará de mostrar el mensaje.
                Debug.Log("Bloqueado: No hay miembro seleccionado.");
            }
        }
    }
}