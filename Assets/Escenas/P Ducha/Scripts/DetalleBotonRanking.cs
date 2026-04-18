using UnityEngine;

public class DetalleBotonRanking : MonoBehaviour
{
    // Este es el cuadrito que le falta a Unity encontrar:
    public string nombreMiembro;

    public void AlHacerClic()
    {
        // Buscamos el manejador en la escena
        ManejadorRanking manejador = Object.FindFirstObjectByType<ManejadorRanking>();

        if (manejador != null)
        {
            // Le pedimos que abra el historial de este nombre
            manejador.AbrirHistorial(nombreMiembro);
        }
    }
}