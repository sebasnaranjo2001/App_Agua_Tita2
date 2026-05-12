using UnityEngine;
using TMPro;

public class BuscadorDeMiembros : MonoBehaviour
{
    public TMP_Text textoNombreBañandose;

    // Se ejecuta cada vez que el panel se activa
    void OnEnable()
    {
        ActualizarInterfaz();
    }

    public void ActualizarInterfaz()
    {
        if (ManejadorRegistro.instance != null)
        {
            string nombre = ManejadorRegistro.instance.nombreSeleccionado;
            if (!string.IsNullOrEmpty(nombre))
            {
                textoNombreBañandose.text = nombre;
            }
            else
            {
                textoNombreBañandose.text = "---";
            }
        }
    }
}