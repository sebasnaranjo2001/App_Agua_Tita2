using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManagerDrag : MonoBehaviour
{
    [System.Serializable]
    public class Fase
    {
        public string[] textos;
        public Sprite[] imagenes;
    }

    public Fase[] fases;

    public DropZone[] zonas;
    public DragItem[] items;

    public TMP_Text[] textosUI;
    public Image[] imagenesUI;

    [Header("Panel Final")]
    public GameObject panelFinal;

    [Header("Ir a otra escena y panel")]
    public string nombreEscenaDestino;   // Escena donde está el panel
    public string nombrePanelDestino;    // Nombre exacto del panel
    public float tiempoEspera = 3f;      // Segundos de espera

    private int faseActual = 0;

    void Start()
    {
        panelFinal.SetActive(false);
        CargarFase();
    }

    void CargarFase()
    {
        Fase f = fases[faseActual];

        for (int i = 0; i < textosUI.Length; i++)
        {
            textosUI[i].text = f.textos[i];
        }

        for (int i = 0; i < imagenesUI.Length; i++)
        {
            imagenesUI[i].sprite = f.imagenes[i];
        }

        foreach (DropZone zona in zonas)
        {
            zona.ResetZona();
        }

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
            StartCoroutine(CargarPanelDespues());
        }
        else
        {
            CargarFase();
        }
    }

    IEnumerator CargarPanelDespues()
    {
        yield return new WaitForSeconds(tiempoEspera);

        SceneManager.LoadScene(nombreEscenaDestino);

        yield return null; // Espera 1 frame

        GameObject panel = GameObject.Find(nombrePanelDestino);

        if (panel != null)
        {
            panel.SetActive(true);
        }
    }

    public void Volver()
    {
        SceneManager.LoadScene("Menu");
    }
}