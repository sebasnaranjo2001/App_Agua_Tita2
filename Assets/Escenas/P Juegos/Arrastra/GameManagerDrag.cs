using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManagerDrag : MonoBehaviour
{
    [System.Serializable]
    public class Fase
    {
        public string[] textos;          // Textos de A, B, C
        public Sprite[] imagenes;       // Imágenes de Zona1, Zona2, Zona3
    }

    public Fase[] fases;

    public DropZone[] zonas;           // Zona1, Zona2, Zona3
    public DragItem[] items;           // A, B, C

    public TMP_Text[] textosUI;        // Textos visuales de A, B, C
    public Image[] imagenesUI;         // Imágenes visuales de Zona1,2,3

    public GameObject panelFinal;

    private int faseActual = 0;

    void Start()
    {
        panelFinal.SetActive(false);
        CargarFase();
    }

    void CargarFase()
    {
        Fase f = fases[faseActual];

        // Cambiar textos
        for (int i = 0; i < textosUI.Length; i++)
        {
            textosUI[i].text = f.textos[i];
        }

        // Cambiar imágenes
        for (int i = 0; i < imagenesUI.Length; i++)
        {
            imagenesUI[i].sprite = f.imagenes[i];
        }

        // Resetear zonas
        foreach (DropZone zona in zonas)
        {
            zona.ResetZona();
        }

        // Resetear posiciones de A, B, C
        foreach (DragItem item in items)
        {
            item.ResetPosition();
        }
    }

    public void Comprobar()
    {
        bool todoCorrecto = true;

        foreach (DropZone zona in zonas)
        {
            // Si no hay objeto soltado
            if (zona.objetoActual == null)
            {
                zona.MarcarIncorrecto();
                todoCorrecto = false;
            }
            else if (zona.EsCorrecto())
            {
                zona.MarcarCorrecto();
            }
            else
            {
                zona.MarcarIncorrecto();
                todoCorrecto = false;
            }
        }

        if (todoCorrecto)
        {
            Invoke("SiguienteFase", 1.5f);
        }
        else
        {
            Invoke("Resetear", 1.5f);
        }
    }

    void Resetear()
    {
        foreach (DropZone zona in zonas)
        {
            zona.ResetZona();
        }

        foreach (DragItem item in items)
        {
            item.ResetPosition();
        }
    }

    void SiguienteFase()
    {
        faseActual++;

        if (faseActual >= fases.Length)
        {
            panelFinal.SetActive(true);
        }
        else
        {
            CargarFase();
        }
    }

    public void Volver()
    {
        SceneManager.LoadScene("z_juegos");
    }
}