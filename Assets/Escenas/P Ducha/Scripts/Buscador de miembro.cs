using UnityEngine;
using TMPro;

public class BuscadorDeMiembros : MonoBehaviour
{
    [Header("Referencias de UI")]
    public TMP_Text textoNombreBañandose; // Arrastra aquí el texto que dice el nombre
    public GameObject panelSeleccionRapida; // Arrastra aquí el panel con la lista (opcional por ahora)

    void Start()
    {
        // Apenas cargue la escena del cronómetro, actualizamos el nombre
        ActualizarInterfaz();
    }

    public void ActualizarInterfaz()
    {
        // Verificamos que el "cerebro" exista
        if (ManejadorRegistro.instance != null)
        {
            string nombre = ManejadorRegistro.instance.nombreSeleccionado;

            // Si hay un nombre guardado, lo ponemos. Si no, avisamos.
            if (!string.IsNullOrEmpty(nombre))
            {
                textoNombreBañandose.text = nombre;
            }
            else
            {
                textoNombreBañandose.text = "---";
                Debug.LogWarning("¡Sebas, no hay ningún miembro seleccionado!");
            }
        }
    }

    // Esta función sirve para el botón que quieres que despliegue la lista
    public void AlternarListaDeMiembros()
    {
        if (panelSeleccionRapida != null)
        {
            bool estadoActual = panelSeleccionRapida.activeSelf;
            panelSeleccionRapida.SetActive(!estadoActual);
        }
    }
}