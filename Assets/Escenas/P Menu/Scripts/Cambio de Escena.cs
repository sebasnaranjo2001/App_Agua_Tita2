using UnityEngine;
using UnityEngine.SceneManagement;

public class CambiarEscena : MonoBehaviour
{
    [Header("Configuración")]
    public string nombreEscena;

    // --- FUNCIÓN 1: La de siempre (Inicio, Volver, etc.) ---
    public void IrAEscena()
    {
        if (!string.IsNullOrEmpty(nombreEscena))
        {
            SceneManager.LoadScene(nombreEscena);
        }
    }

    // --- FUNCIÓN PARA EL CRONÓMETRO: Guarda y cambia de escena ---
    public void IrAEscenaGuardando()
    {
        // Buscamos tu script llamado 'Cronometro'
        Cronometro scriptDucha = Object.FindFirstObjectByType<Cronometro>();

        if (scriptDucha != null)
        {
            // Intentamos llamar a la función de guardado
            scriptDucha.GuardarTiempoFinal();
            Debug.Log("Datos del cronómetro enviados al registro.");
        }

        // Cambiamos de escena
        IrAEscena();
    }

    // --- FUNCIÓN ESPECIAL: Para el botón Empezar (Duchómetro) ---
    public void IrAEscenaSiHayMiembro()
    {
        if (ManejadorRegistro.instance != null)
        {
            if (!string.IsNullOrEmpty(ManejadorRegistro.instance.nombreSeleccionado))
            {
                SceneManager.LoadScene(nombreEscena);
            }
            else
            {
                Debug.Log("Bloqueado: No hay miembro seleccionado.");
            }
        }
    }
}