using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManagerDrag : MonoBehaviour
{
    [System.Serializable]
    public class Fase
    {
        public string[] textos;
        public Sprite[] imagenes;
    }

    [Header("Fases")]
    public Fase[] fases;

    [Header("Gameplay")]
    public DropZone[] zonas;
    public DragItem[] items;
    public TMP_Text[] textosUI;
    public Image[] imagenesUI;

    [Header("Elementos Gameplay")]
    // PON TODO EL GAMEPLAY MENOS:
    // fondo y panels finales
    public GameObject[] elementosGameplay;

    [Header("Panels Finales")]
    public GameObject panelVictoria;
    public GameObject panelIntermedio;
    public GameObject panelDerrota;

    [Header("Textos de Aciertos por Panel")]
    public TMP_Text textoAciertosVictoria;
    public TMP_Text textoAciertosIntermedio;
    public TMP_Text textoAciertosDerrota;

    [Header("Texto Aciertos Gameplay")]
    public TMP_Text textoAciertos;

    private int faseActual = 0;
    private int aciertos = 0;

    void Start()
    {
        // =========================
        // MEZCLAR FASES
        // =========================

        MezclarFases();

        // =========================
        // DESACTIVAR PANELES
        // =========================

        if (panelVictoria != null)
            panelVictoria.SetActive(false);

        if (panelIntermedio != null)
            panelIntermedio.SetActive(false);

        if (panelDerrota != null)
            panelDerrota.SetActive(false);

        // =========================
        // MOSTRAR GAMEPLAY
        // =========================

        foreach (GameObject obj in elementosGameplay)
        {
            if (obj != null)
                obj.SetActive(true);
        }

        // =========================
        // ACTUALIZAR TEXTO GAMEPLAY
        // =========================

        ActualizarTextoAciertos();

        // =========================
        // PRIMERA FASE
        // =========================

        CargarFase(false);
    }

    // =========================
    // MEZCLAR FASES
    // =========================
    void MezclarFases()
    {
        for (int i = 0; i < fases.Length; i++)
        {
            int randomIndex =
                Random.Range(i, fases.Length);

            Fase temp = fases[i];
            fases[i] = fases[randomIndex];
            fases[randomIndex] = temp;
        }
    }

    // =========================
    // CARGAR FASE
    // =========================
    void CargarFase(bool resetearPosiciones = true)
    {
        if (fases == null || fases.Length == 0)
            return;

        Fase f = fases[faseActual];

        // =========================
        // APLICAR TEXTOS
        // =========================

        for (int i = 0; i < textosUI.Length; i++)
        {
            if (textosUI[i] != null &&
                i < f.textos.Length)
            {
                textosUI[i].text = f.textos[i];
            }
        }

        // =========================
        // APLICAR IMÁGENES
        // =========================

        for (int i = 0; i < imagenesUI.Length; i++)
        {
            if (imagenesUI[i] != null &&
                i < f.imagenes.Length)
            {
                imagenesUI[i].sprite =
                    f.imagenes[i];
            }
        }

        // =========================
        // RESET ZONAS
        // =========================

        foreach (DropZone zona in zonas)
        {
            if (zona != null)
                zona.ResetZona();
        }

        // =========================
        // RESET ITEMS
        // =========================

        if (resetearPosiciones)
        {
            foreach (DragItem item in items)
            {
                if (item != null)
                    item.ResetPosition();
            }
        }

        // =========================
        // MEZCLAR POSICIONES
        // DRAG ITEMS
        // =========================

        for (int i = 0; i < items.Length; i++)
        {
            int randomIndex =
                Random.Range(i, items.Length);

            Vector3 tempPos =
                items[i].transform.position;

            items[i].transform.position =
                items[randomIndex].transform.position;

            items[randomIndex].transform.position =
                tempPos;
        }
    }

    // =========================
    // COMPROBAR
    // =========================
    public void Comprobar()
    {
        bool todoCorrecto = true;

        foreach (DropZone zona in zonas)
        {
            if (zona == null)
                continue;

            // VACÍO
            if (zona.objetoActual == null)
            {
                zona.MarcarIncorrecto();
                todoCorrecto = false;
            }
            // CORRECTO
            else if (zona.EsCorrecto())
            {
                zona.MarcarCorrecto();
            }
            // INCORRECTO
            else
            {
                zona.MarcarIncorrecto();
                todoCorrecto = false;
            }
        }

        // =========================
        // SUMAR ACIERTO
        // =========================

        // SOLO SI TODA LA FASE ESTÁ PERFECTA
        if (todoCorrecto)
        {
            aciertos++;

            ActualizarTextoAciertos();
        }

        // =========================
        // PASAR SIEMPRE
        // =========================

        Invoke("SiguienteFase", 1.5f);
    }

    // =========================
    // SIGUIENTE FASE
    // =========================
    void SiguienteFase()
    {
        faseActual++;

        // FINAL DEL JUEGO
        if (faseActual >= fases.Length)
        {
            MostrarResultadoFinal();
        }
        else
        {
            CargarFase(true);
        }
    }

    // =========================
    // ACTUALIZAR TEXTO GAMEPLAY
    // =========================
    void ActualizarTextoAciertos()
    {
        if (textoAciertos != null)
        {
            textoAciertos.text =
                "Aciertos: " +
                aciertos +
                "/" +
                fases.Length;
        }
    }

    // =========================
    // RESULTADO FINAL
    // =========================
    void MostrarResultadoFinal()
    {
        // =========================
        // OCULTAR GAMEPLAY
        // =========================

        foreach (GameObject obj in elementosGameplay)
        {
            if (obj != null)
                obj.SetActive(false);
        }

        // =========================
        // DESACTIVAR PANELES
        // =========================

        if (panelVictoria != null)
            panelVictoria.SetActive(false);

        if (panelIntermedio != null)
            panelIntermedio.SetActive(false);

        if (panelDerrota != null)
            panelDerrota.SetActive(false);

        // =========================
        // TEXTO FINAL
        // =========================

        string resultadoFinal =
            "Aciertos: " +
            aciertos +
            "/" +
            fases.Length;

        // =========================
        // VICTORIA
        // =========================

        if (aciertos == fases.Length)
        {
            if (panelVictoria != null)
                panelVictoria.SetActive(true);

            if (textoAciertosVictoria != null)
                textoAciertosVictoria.text = resultadoFinal;
        }

        // =========================
        // DERROTA
        // 0 o 1
        // =========================

        else if (aciertos <= 1)
        {
            if (panelDerrota != null)
                panelDerrota.SetActive(true);

            if (textoAciertosDerrota != null)
                textoAciertosDerrota.text = resultadoFinal;
        }

        // =========================
        // INTERMEDIO
        // 2,3,4
        // =========================

        else
        {
            if (panelIntermedio != null)
                panelIntermedio.SetActive(true);

            if (textoAciertosIntermedio != null)
                textoAciertosIntermedio.text = resultadoFinal;
        }
    }

    // =========================
    // VOLVER
    // =========================
    public void Volver()
    {
        SceneManager.LoadScene("Menu");
    }

    // =========================
    // REINICIAR
    // =========================
    public void Reiniciar()
    {
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }
}