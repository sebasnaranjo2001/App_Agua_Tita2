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
    public GameObject[] elementosGameplay;

    [Header("Panels Finales")]
    public GameObject panelVictoria;
    public GameObject panelIntermedio;
    public GameObject panelDerrota;

    [Header("Gotas Victoria")]
    public GameObject gotaVictoria1;
    public GameObject gotaVictoria2;
    public GameObject gotaVictoria3;

    [Header("Gotas Intermedio")]
    public GameObject gotaIntermedio1;
    public GameObject gotaIntermedio2;
    public GameObject gotaIntermedio3;

    [Header("Gotas Derrota")]
    public GameObject gotaDerrota1;
    public GameObject gotaDerrota2;
    public GameObject gotaDerrota3;


    [Header("Textos de Aciertos por Panel")]
    public TMP_Text textoAciertosVictoria;
    public TMP_Text textoAciertosIntermedio;
    public TMP_Text textoAciertosDerrota;

    [Header("Texto Aciertos Gameplay")]
    public TMP_Text textoAciertos;

    [Header("Elementos Panel Victoria")]
    public GameObject[] elementosVictoria;

    [Header("Elementos Panel Intermedio")]
    public GameObject[] elementosIntermedio;

    [Header("Elementos Panel Derrota")]
    public GameObject[] elementosDerrota;

    [Header("Animaciones Panel")]
    public float delayEntreElementos = 0.1f;

    [Header("Animaciones UI")]
    public TMP_Text categoriaJuego;
    public RectTransform fondoCategoria;
    public Button botonComprobar;
    [Header("Barra de Progreso")]
    public ProgressBarUI barraProgreso;
    [Header("Cronometro")]
    public float tiempoMaximo = 60f;
    private float tiempoActual;
    private bool tiempoActivo = true;

    public Image fillReloj;
    public TMP_Text textoTiempo;

    [Header("Tiempo en Paneles")]
    public TMP_Text textoTiempoVictoria;
    public TMP_Text textoTiempoIntermedio;
    public TMP_Text textoTiempoDerrota;

    [Header("Animacion Reloj")]
    public RectTransform relojTransform;

    private Vector3 posicionOriginalReloj;
    private int ultimoSegundoMostrado;


    private Vector2 posicionOriginalCategoria;
    private Vector2 posicionOriginalBoton;

    private int faseActual = 0;
    private int aciertos = 0;

    void Start()
    {
        
        MezclarFases();

        if (panelVictoria != null) panelVictoria.SetActive(false);
        if (panelIntermedio != null) panelIntermedio.SetActive(false);
        if (panelDerrota != null) panelDerrota.SetActive(false);

        foreach (GameObject obj in elementosGameplay)
        {
            if (obj != null) obj.SetActive(true);
        }

       

        // Animación categoría
        posicionOriginalCategoria = fondoCategoria.anchoredPosition;

        fondoCategoria.anchoredPosition =
            new Vector2(
                posicionOriginalCategoria.x,
                posicionOriginalCategoria.y + 200f
            );

        LeanTween.move(
            fondoCategoria,
            posicionOriginalCategoria,
            0.7f
        ).setDelay(0.15f).setEaseOutBack();

        // Animación botón
        RectTransform botonRect = botonComprobar.GetComponent<RectTransform>();
        posicionOriginalBoton = botonRect.anchoredPosition;
        botonRect.anchoredPosition = new Vector2(posicionOriginalBoton.x, posicionOriginalBoton.y - 250f);
        LeanTween.move(botonRect, posicionOriginalBoton, 0.6f).setDelay(0.3f).setEaseOutBack();

        if (barraProgreso != null)
        {
            barraProgreso.ReiniciarBarra();
        }

        tiempoActual = tiempoMaximo;
        ultimoSegundoMostrado = Mathf.CeilToInt(tiempoActual);

        if (fillReloj != null)
        {
            fillReloj.fillAmount = 0f;
        }

        if (textoTiempo != null)
        {
            int minutos = Mathf.FloorToInt(tiempoActual / 60);
            int segundos = Mathf.FloorToInt(tiempoActual % 60);

            textoTiempo.text =
                string.Format("{0:00}:{1:00}", minutos, segundos);
        }

        if (relojTransform != null)
        {
            posicionOriginalReloj = relojTransform.localPosition;

            relojTransform.localScale = Vector3.zero;

            LeanTween.scale(
                relojTransform.gameObject,
                Vector3.one,
                0.4f
            ).setEaseOutBack();
        }

        CargarFase(false);
    }

    void Update()
    {
        ActualizarCronometro();
    }

    void MezclarFases()
    {
        for (int i = 0; i < fases.Length; i++)
        {
            int randomIndex = Random.Range(i, fases.Length);
            Fase temp = fases[i];
            fases[i] = fases[randomIndex];
            fases[randomIndex] = temp;
        }
    }

    void CargarFase(bool resetearPosiciones = true)
    {
        if (barraProgreso != null)
        {
            barraProgreso.Actualizar(
                faseActual,
                fases.Length,
                "Fase"
            );

            barraProgreso.textoEstado.text =
                "Fase " + (faseActual + 1) +
                " de " + fases.Length;
        }
        if (fases == null || fases.Length == 0) return;

        Fase f = fases[faseActual];

        // Textos
        for (int i = 0; i < textosUI.Length; i++)
        {
            if (textosUI[i] != null && i < f.textos.Length)
            {
                textosUI[i].text = f.textos[i];
                
            }
        }

        // Imágenes
        for (int i = 0; i < imagenesUI.Length; i++)
        {
            if (imagenesUI[i] != null && i < f.imagenes.Length)
            {
                imagenesUI[i].sprite = f.imagenes[i];
                
                imagenesUI[i].transform.localScale = Vector3.zero;
                LeanTween.scale(imagenesUI[i].gameObject, Vector3.one, 0.35f).setDelay(i * 0.1f).setEaseOutBack();
            }
        }

        // Reset zonas
        foreach (DropZone zona in zonas)
        {
            if (zona != null) zona.ResetZona();
        }

        // Reset items
        if (resetearPosiciones)
        {
            foreach (DragItem item in items)
            {
                if (item != null) item.ResetPosition();
            }
        }

        // Mezclar posiciones
        for (int i = 0; i < items.Length; i++)
        {
            int randomIndex = Random.Range(i, items.Length);
            RectTransform rectA = items[i].GetComponent<RectTransform>();
            RectTransform rectB = items[randomIndex].GetComponent<RectTransform>();
            Vector2 tempPos = rectA.anchoredPosition;
            rectA.anchoredPosition = rectB.anchoredPosition;
            rectB.anchoredPosition = tempPos;
        }

        // Animar items
        for (int j = 0; j < items.Length; j++)
        {
            items[j].transform.localScale = Vector3.zero;
            LeanTween.scale(items[j].gameObject, Vector3.one, 0.35f).setDelay(0.25f + j * 0.1f).setEaseOutBack();
        }

        
    }

    void ActualizarCronometro()
    {
        if (!tiempoActivo)
            return;

        tiempoActual -= Time.deltaTime;

        if (tiempoActual < 0)
            tiempoActual = 0;

        int minutos = Mathf.FloorToInt(tiempoActual / 60);
        int segundos = Mathf.FloorToInt(tiempoActual % 60);

        if (textoTiempo != null)
        {
            textoTiempo.text =
                string.Format("{0:00}:{1:00}", minutos, segundos);
        }

        if (fillReloj != null)
        {
            fillReloj.fillAmount =
                1f - (tiempoActual / tiempoMaximo);
        }

        int segundoActual = Mathf.CeilToInt(tiempoActual);

        if (segundoActual != ultimoSegundoMostrado)
        {
            ultimoSegundoMostrado = segundoActual;

            if (relojTransform != null)
            {
                LeanTween.scale(
                    relojTransform.gameObject,
                    Vector3.one * 1.12f,
                    0.1f
                ).setOnComplete(() =>
                {
                    LeanTween.scale(
                        relojTransform.gameObject,
                        Vector3.one,
                        0.1f
                    );
                });
            }
        }

        if (tiempoActual <= 5f && relojTransform != null)
        {
            float shake =
                Mathf.Sin(Time.time * 40f) * 3f;

            relojTransform.localPosition =
                posicionOriginalReloj +
                new Vector3(shake, 0, 0);
        }
        else if (relojTransform != null)
        {
            relojTransform.localPosition = posicionOriginalReloj;
        }

        if (tiempoActual <= 0)
        {
            tiempoActivo = false;

            MostrarTiempoEnPaneles();

            foreach (GameObject obj in elementosGameplay)
            {
                if (obj != null)
                    obj.SetActive(false);
            }

            if (textoAciertosDerrota != null)
            {
                textoAciertosDerrota.text =
                    "Aciertos: " + aciertos + "/" + fases.Length;
            }

            AnimarPanelDerrota();
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

            if (zona.objetoActual != null)
                zona.objetoActual.MarcarIncorrecto();

            todoCorrecto = false;
        }
        else if (zona.EsCorrecto())
        {
            zona.MarcarCorrecto();

            if (zona.objetoActual != null)
                zona.objetoActual.MarcarCorrecto();
        }
        else
        {
            zona.MarcarIncorrecto();

            if (zona.objetoActual != null)
                zona.objetoActual.MarcarIncorrecto();

            todoCorrecto = false;
        }
    }

    if (todoCorrecto)
    {
        aciertos++;
        ActualizarTextoAciertos();
    }

    Invoke("SiguienteFase", 1.5f);
}

    void SiguienteFase()
    {
        faseActual++;

        Debug.Log("Siguiente fase -> " + faseActual);

        if (barraProgreso != null)
        {
            barraProgreso.Actualizar(
                faseActual,
                fases.Length,
                "Fase"
            );

            barraProgreso.textoEstado.text =
                "Fase " + (faseActual + 1) +
                " de " + fases.Length;
        }

        if (faseActual >= fases.Length)
        {
            if (barraProgreso != null)
            {
                barraProgreso.Actualizar(
                    fases.Length,
                    fases.Length,
                    "Fase"
                );
            }

            MostrarResultadoFinal();
        }
        else
        {
            CargarFase(true);
        }
    }

    void ActualizarTextoAciertos()
    {
        if (textoAciertos != null)
            textoAciertos.text = "Aciertos: " + aciertos + "/" + fases.Length;
    }
    void MostrarTiempoEnPaneles()
    {
        int tiempoUsado =
            Mathf.RoundToInt(tiempoMaximo - tiempoActual);

        string textoFinal =
            tiempoUsado + " segundos";

        if (textoTiempoVictoria != null)
            textoTiempoVictoria.text = textoFinal;

        if (textoTiempoIntermedio != null)
            textoTiempoIntermedio.text = textoFinal;

        if (textoTiempoDerrota != null)
            textoTiempoDerrota.text = textoFinal;
    }

    void MostrarResultadoFinal()
    {
        tiempoActivo = false;

        MostrarTiempoEnPaneles();

        string resultadoFinal =
            "Aciertos: " + aciertos + "/" + fases.Length;

        float porcentaje = (float)aciertos / fases.Length;

        if (porcentaje >= 0.8f)
        {
            if (textoAciertosVictoria != null)
                textoAciertosVictoria.text = resultadoFinal;

            AnimarPanelVictoria();
        }
        else if (porcentaje >= 0.5f)
        {
            if (textoAciertosIntermedio != null)
                textoAciertosIntermedio.text = resultadoFinal;

            AnimarPanelIntermedio();
        }
        else
        {
            if (textoAciertosDerrota != null)
                textoAciertosDerrota.text = resultadoFinal;

            AnimarPanelDerrota();
        }
    }

    void AnimarElemento(GameObject obj, float delay)
    {
        if (obj == null) return;

        obj.transform.localScale = Vector3.zero;

        LeanTween.scale(
            obj,
            Vector3.one,
            0.35f
        )
        .setDelay(delay)
        .setEaseOutBack();
    }

    void AnimarElementosPanel(GameObject[] elementos)
    {
        if (elementos == null) return;

        for (int i = 0; i < elementos.Length; i++)
        {
            if (elementos[i] == null)
                continue;

            elementos[i].SetActive(true);

            elementos[i].transform.localScale = Vector3.zero;

            LeanTween.scale(
                elementos[i],
                Vector3.one,
                0.3f
            )
            .setDelay(i * 0.1f)
            .setEaseOutBack();
        }
    }


    void AnimarPanelVictoria()
    {
        foreach (GameObject obj in elementosGameplay)
        {
            if (obj != null)
                obj.SetActive(false);
        }

        foreach (GameObject obj in elementosVictoria)
        {
            if (obj != null)
                obj.SetActive(false);
        }

        panelVictoria.SetActive(true);

        panelVictoria.transform.localScale = Vector3.zero;

        LeanTween.scale(
            panelVictoria,
            Vector3.one,
            0.5f
        ).setEaseOutBack();

        AnimarElemento(gotaVictoria1, 0.1f);
        AnimarElemento(gotaVictoria2, 0.5f);
        AnimarElemento(gotaVictoria3, 0.9f);

        LeanTween.delayedCall(
            1.4f,
            () =>
            {
                AnimarElementosPanel(elementosVictoria);
            });
    }

    void AnimarPanelIntermedio()
    {
        foreach (GameObject obj in elementosGameplay)
        {
            if (obj != null)
                obj.SetActive(false);
        }

        foreach (GameObject obj in elementosIntermedio)
        {
            if (obj != null)
                obj.SetActive(false);
        }

        panelIntermedio.SetActive(true);

        panelIntermedio.transform.localScale = Vector3.zero;

        LeanTween.scale(
            panelIntermedio,
            Vector3.one,
            0.5f
        ).setEaseOutBack();

        if (gotaIntermedio1 != null)
        {
            gotaIntermedio1.SetActive(true);
            AnimarElemento(gotaIntermedio1, 0f);
        }

        if (gotaIntermedio2 != null)
        {
            gotaIntermedio2.SetActive(true);
            AnimarElemento(gotaIntermedio2, 0.4f);
        }

        if (gotaIntermedio3 != null)
        {
            gotaIntermedio3.SetActive(true);
            AnimarElemento(gotaIntermedio3, 0.8f);
        }

        LeanTween.delayedCall(
            1.3f,
            () =>
            {
                AnimarElementosPanel(elementosIntermedio);
            });
    }

    void AnimarPanelDerrota()
    {
        foreach (GameObject obj in elementosGameplay)
        {
            if (obj != null)
                obj.SetActive(false);
        }

        foreach (GameObject obj in elementosDerrota)
        {
            if (obj != null)
                obj.SetActive(false);
        }

        panelDerrota.SetActive(true);

        panelDerrota.transform.localScale = Vector3.zero;

        LeanTween.scale(
            panelDerrota,
            Vector3.one,
            0.5f
        ).setEaseOutBack();

        AnimarElemento(gotaDerrota1, 0.1f);

        LeanTween.delayedCall(
            0.6f,
            () =>
            {
                AnimarElementosPanel(elementosDerrota);
            });
    }

}


