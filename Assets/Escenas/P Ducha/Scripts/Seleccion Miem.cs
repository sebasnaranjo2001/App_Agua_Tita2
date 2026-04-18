using UnityEngine;
using UnityEngine.UI;

public class SeleccionMiembros : MonoBehaviour
{
    [Header("Referencias")]
    public Image circuloConfirmacion; // El objeto "Confirmacion" (el círculo)

    [Header("Colores")]
    public Color colorVerde = Color.green;
    public Color colorBlanco = Color.white;

    public void SeleccionarEsteMiembro()
    {
        // 1. Le avisamos al sistema central (Avisos) que ESTE es el miembro elegido
        // Esto es lo que hará que el botón "Empezar" y "Eliminar" funcionen
        if (Avisos.instance != null)
        {
            Avisos.instance.RegistrarSeleccion(this);
        }

        // 2. Cambiamos visualmente el círculo a verde
        if (circuloConfirmacion != null)
        {
            circuloConfirmacion.color = colorVerde;
        }

        Debug.Log("Has seleccionado a: " + gameObject.name);
    }

    // Función que llamará el script de Avisos para limpiar la selección previa
    public void Deseleccionar()
    {
        if (circuloConfirmacion != null)
        {
            circuloConfirmacion.color = colorBlanco;
        }
    }
}