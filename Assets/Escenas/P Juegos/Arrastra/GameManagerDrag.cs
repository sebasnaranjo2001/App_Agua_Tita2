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

    [Header("Configuración Escena")]
    public string nombreEscenaDestino;
    public string nombrePanelDestino;
    public float tiempoEspera = 3f;

    private int faseActual = 0;

    void Start()
    {
        if (panelFinal != null) panelFinal.SetActive(false);

        // Cargamos la fase pero SIN resetear posiciones al puro inicio
        // para no chocar con el Start de los items
        CargarFase(false);
    }

    void CargarFase(bool resetearPosiciones = true)
    {
        if (fases == null || fases.Length == 0) return;
        Fase f = fases[faseActual];

        // Llenar textos con protección
        for (int i = 0; i < textosUI.Length; i++)
        {
            if (textosUI[i] != null && i < f.textos.Length)
                textosUI[i].text = f.textos[i];
        }

        // Llenar imágenes con protección
        for (int i = 0; i < imagenesUI.Length; i++)
        {
            if (imagenesUI[i] != null && i < f.imagenes.Length)
                imagenesUI[i].sprite = f.imagenes[i];
        }

        foreach (DropZone zona in zonas)
        {
            if (zona != null) zona.ResetZona();
        }

        // Solo reseteamos posiciones si no es el primer arranque
        if (resetearPosiciones)
        {
            foreach (DragItem item in items)
            {
                if (item != null) item.ResetPosition();
            }
        }
    }

    public void Comprobar()
    {
        bool todoCorrecto = true;

        foreach (DropZone zona in zonas)
        {
            if (zona == null) continue;

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

        if (todoCorrecto) Invoke("SiguienteFase", 1.5f);
        else Invoke("Resetear", 1.5f);
    }

    void Resetear()
    {
        foreach (DropZone zona in zonas) if (zona != null) zona.ResetZona();
        foreach (DragItem item in items) if (item != null) item.ResetPosition();
    }

    void SiguienteFase()
    {
        faseActual++;
        if (faseActual >= fases.Length)
        {
            if (panelFinal != null) panelFinal.SetActive(true);
            StartCoroutine(CargarPanelDespues());
        }
        else CargarFase(true);
    }

    IEnumerator CargarPanelDespues()
    {
        yield return new WaitForSeconds(tiempoEspera);
        SceneManager.LoadScene(nombreEscenaDestino);
        yield return new WaitForSeconds(0.1f);

        GameObject panel = GameObject.Find(nombrePanelDestino);
        if (panel != null) panel.SetActive(true);
    }

    public void Volver() => SceneManager.LoadScene("Menu");
}