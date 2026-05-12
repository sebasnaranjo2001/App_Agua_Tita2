using UnityEngine;
using TMPro; // Para usar TextMeshPro

public class ManejadorRetos : MonoBehaviour
{
    [Header("Referencias UI")]
    public GameObject panelRetos;
    public TMP_Text textoDelReto;

    [Header("Configuración de Retos")]
    [TextArea]
    public string[] listaDeRetos = new string[] {
        "¡Dúchate en menos de 5 minutos hoy!",
        "Cierra el grifo mientras te lavas los dientes.",
        "Usa un balde en lugar de manguera para lavar algo."
    };

    private int ultimoIndice = -1; // Para no repetir el mismo reto al refrescar

    void Start()
    {
        if (panelRetos != null) panelRetos.SetActive(false);
    }

    public void AbrirRetos()
    {
        if (panelRetos == null || listaDeRetos.Length == 0) return;

        MostrarNuevoReto();

        // Animación de aparecer
        panelRetos.SetActive(true);
        panelRetos.transform.localScale = Vector3.zero;
        LeanTween.scale(panelRetos, Vector3.one, 0.4f).setEaseOutBack();
    }

    // Esta es la función para el botón de Refrescar
    public void RefrescarReto()
    {
        // Pequeña animación de "pulsación" para que se note el cambio
        LeanTween.scale(textoDelReto.gameObject, Vector3.one * 1.1f, 0.1f).setLoopPingPong(1).setOnComplete(() => {
            MostrarNuevoReto();
        });
    }

    private void MostrarNuevoReto()
    {
        int nuevoIndice;

        // Si hay más de un reto, buscamos uno que no sea el mismo de antes
        if (listaDeRetos.Length > 1)
        {
            do
            {
                nuevoIndice = Random.Range(0, listaDeRetos.Length);
            } while (nuevoIndice == ultimoIndice);
        }
        else
        {
            nuevoIndice = 0;
        }

        ultimoIndice = nuevoIndice;
        textoDelReto.text = listaDeRetos[nuevoIndice];
    }

    public void CerrarRetos()
    {
        LeanTween.scale(panelRetos, Vector3.zero, 0.3f).setEaseInBack().setOnComplete(() => {
            panelRetos.SetActive(false);
        });
    }
}