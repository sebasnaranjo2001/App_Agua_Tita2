using UnityEngine;
using TMPro;

public class DetalleBotonRanking : MonoBehaviour
{
    public string nombreMiembro;

    [Header("Referencias de Textos")]
    public TMP_Text txtPuesto;
    public TMP_Text txtNombre;
    public TMP_Text txtTiempo;
    public TMP_Text txtMotivacion;

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