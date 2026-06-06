using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ManejadorNavegacion : MonoBehaviour
{
    [Header("--- PANELES PRINCIPALES ---")]
    public GameObject panelRegistro;
    public GameObject panelCronometro;
    public GameObject panelRanking;

    [Header("--- BARRA DE SELECCIÓN ---")]
    public RectTransform indicadorSeleccion;
    public float tiempoAnimacion = 0.35f;
    public LeanTweenType tipoCurva = LeanTweenType.easeOutBack;

    [Header("Configuración Barra")]
    public float posRegX; public float anchoReg;
    public float posCronX; public float anchoCron;
    public float posRankX; public float anchoRank;

    [Header("--- TEXTOS BARRA NAVEGACIÓN ---")]
    public TextMeshProUGUI txtRegistro;
    public TextMeshProUGUI txtCronometro;
    public TextMeshProUGUI txtRanking;
    public Color colorTextoInactivo = new Color(26f / 255f, 58f / 255f, 95f / 255f); // #1A3A5F (Azul Corporativo)
    public Color colorTextoActivo = Color.white;

    [Header("--- COLORES INDICADOR DE SELECCIÓN ---")]
    public Color colorBarraRegistro = new Color(34f / 255f, 72f / 255f, 112f / 255f); // #224870 (Actualizado a oscuro)
    public Color colorBarraCronometro = new Color(24f / 255f, 90f / 255f, 78f / 255f); // #185A4E (Actualizado a oscuro)
    public Color colorBarraRanking = new Color(117f / 255f, 71f / 255f, 34f / 255f); // #754722 (Actualizado a oscuro)

    [Header("--- ENCABEZADO DUCHOMETRO (APARICIÓN SUAVE) ---")]
    public RectTransform encabezadoDuchometro;

    [Header("--- ELEMENTOS UI REGISTRO ---")]
    public GameObject textoNumeroMiembros;
    public GameObject avisos;
    public GameObject botonEmpezar;
    public GameObject botonAnadirGrande;
    public GameObject panelBotonesPequenos;
    public GameObject panelDeslizable;
    public GameObject panelTarjetaRegistro;

    [Header("--- ANIMACIÓN BOTTOM SHEET Y BLUR ---")]
    public RectTransform rectTarjetaRegistro;
    public GameObject fondoBlur;
    public float posAbiertaY = 0f;
    public float posCerradaY = -2000f;

    [Header("--- ELEMENTOS UI CRONOMETRO ---")]
    public GameObject panelPrincipalCrono;
    public GameObject panelSecundarioCrono;
    public GameObject panelBotonesCrono;

    private Dictionary<GameObject, Vector3> escalasOriginales = new Dictionary<GameObject, Vector3>();
    private Dictionary<RectTransform, Vector2> posicionesOriginales = new Dictionary<RectTransform, Vector2>();
    private Vector2 posDisenoReg;
    private string panelActualNombre = "";

    void Awake()
    {
        RegistrarEscalasYPosiciones();
        if (panelRegistro) posDisenoReg = panelRegistro.GetComponent<RectTransform>().anchoredPosition;
    }

    void OnEnable() { ConfiguracionInicial(); }

    void RegistrarEscalasYPosiciones()
    {
        GameObject[] todos = { textoNumeroMiembros, avisos, botonEmpezar, botonAnadirGrande,
                              panelBotonesPequenos, panelDeslizable, panelTarjetaRegistro,
                              panelPrincipalCrono, panelSecundarioCrono, panelBotonesCrono };
        foreach (GameObject obj in todos)
        {
            if (obj != null && !escalasOriginales.ContainsKey(obj))
                escalasOriginales.Add(obj, obj.transform.localScale);
        }

        if (encabezadoDuchometro != null && !posicionesOriginales.ContainsKey(encabezadoDuchometro))
            posicionesOriginales.Add(encabezadoDuchometro, encabezadoDuchometro.anchoredPosition);
    }

    public void ApagarTodo()
    {
        if (panelRegistro) panelRegistro.SetActive(false);
        if (panelCronometro) panelCronometro.SetActive(false);
        if (panelRanking) panelRanking.SetActive(false);
        if (panelTarjetaRegistro) panelTarjetaRegistro.SetActive(false);
        if (Avisos.instance != null) Avisos.instance.ForzarOcultarAvisos();
    }

    public void ConfiguracionInicial()
    {
        panelActualNombre = "";

        AnimarEncabezadoNatural(encabezadoDuchometro, 0.6f, 0.05f);

        Cronometro crono = UnityEngine.Object.FindFirstObjectByType<Cronometro>();
        if (crono != null && crono.estaContando)
        {
            IrACronometro();
        }
        else
        {
            IrARegistro();
        }
    }

    public void IrARegistro()
    {
        if (panelActualNombre == "registro") return;

        // --- NUEVA VALIDACIÓN: Bloquear si el cronómetro está en marcha ---
        Cronometro crono = UnityEngine.Object.FindFirstObjectByType<Cronometro>();
        if (crono != null && crono.estaContando) return;

        ApagarTodo();
        panelRegistro.SetActive(true);
        panelRegistro.GetComponent<RectTransform>().anchoredPosition = posDisenoReg;

        if (Avisos.instance != null) Avisos.instance.ActualizarInterfazSegunContador(false);

        AnimarPum();
        ActualizarBarraVisual(posRegX, anchoReg, colorBarraRegistro);
        ActualizarTextosTab("registro");
        panelActualNombre = "registro";
    }

    public void IrACronometro()
    {
        if (panelActualNombre == "cronometro") return;
        ApagarTodo();
        panelCronometro.SetActive(true);
        AnimarPumCrono();
        ActualizarBarraVisual(posCronX, anchoCron, colorBarraCronometro);
        ActualizarTextosTab("cronometro");
        panelActualNombre = "cronometro";
    }

    public void IrARanking()
    {
        if (panelActualNombre == "ranking") return;

        // --- NUEVA VALIDACIÓN: Bloquear si el cronómetro está en marcha ---
        Cronometro crono = UnityEngine.Object.FindFirstObjectByType<Cronometro>();
        if (crono != null && crono.estaContando) return;

        ApagarTodo();

        if (ManejadorRegistro.instance != null) ManejadorRegistro.instance.ActualizarRanking();

        panelRanking.SetActive(true);
        ActualizarBarraVisual(posRankX, anchoRank, colorBarraRanking);
        ActualizarTextosTab("ranking");
        panelActualNombre = "ranking";
    }

    // --- MÉTODOS VISUALES Y ANIMACIONES ---

    public void ActualizarBarraVisual(float x, float w, Color colorDestino)
    {
        if (indicadorSeleccion == null) return;

        LeanTween.cancel(indicadorSeleccion.gameObject);
        LeanTween.move(indicadorSeleccion, new Vector2(x, indicadorSeleccion.anchoredPosition.y), tiempoAnimacion).setEase(tipoCurva);
        LeanTween.size(indicadorSeleccion, new Vector2(w, indicadorSeleccion.sizeDelta.y), tiempoAnimacion).setEase(tipoCurva);

        Image imgBarra = indicadorSeleccion.GetComponent<Image>();
        if (imgBarra != null)
        {
            LeanTween.color(indicadorSeleccion, colorDestino, tiempoAnimacion);
        }
    }

    private void ActualizarTextosTab(string tabActiva)
    {
        CambiarColorTexto(txtRegistro, tabActiva == "registro" ? colorTextoActivo : colorTextoInactivo);
        CambiarColorTexto(txtCronometro, tabActiva == "cronometro" ? colorTextoActivo : colorTextoInactivo);
        CambiarColorTexto(txtRanking, tabActiva == "ranking" ? colorTextoActivo : colorTextoInactivo);
    }

    private void CambiarColorTexto(TextMeshProUGUI txt, Color colorDestino)
    {
        if (txt == null) return;
        LeanTween.cancel(txt.gameObject);
        LeanTween.value(txt.gameObject, txt.color, colorDestino, tiempoAnimacion)
            .setOnUpdate((Color val) => { txt.color = val; });
    }

    private void AnimarEncabezadoNatural(RectTransform rect, float tiempo, float delay)
    {
        if (rect == null) return;

        Vector2 posFinal = posicionesOriginales.ContainsKey(rect) ? posicionesOriginales[rect] : rect.anchoredPosition;

        CanvasGroup cg = rect.GetComponent<CanvasGroup>();
        if (cg == null) cg = rect.gameObject.AddComponent<CanvasGroup>();

        LeanTween.cancel(rect.gameObject);

        rect.anchoredPosition = new Vector2(posFinal.x, posFinal.y + 30f);
        cg.alpha = 0f;

        LeanTween.moveY(rect, posFinal.y, tiempo).setEase(LeanTweenType.easeOutExpo).setDelay(delay);
        LeanTween.alphaCanvas(cg, 1f, tiempo * 0.8f).setEase(LeanTweenType.easeOutQuad).setDelay(delay);
    }

    public void AnimarPum()
    {
        List<GameObject> aAnimar = new List<GameObject>();
        GameObject[] posibles = { textoNumeroMiembros, avisos, botonEmpezar, botonAnadirGrande, panelBotonesPequenos, panelDeslizable };
        foreach (GameObject obj in posibles)
        {
            if (obj != null && obj.activeSelf) { aAnimar.Add(obj); obj.transform.localScale = Vector3.zero; }
        }
        for (int i = 0; i < aAnimar.Count; i++)
        {
            Vector3 ef = escalasOriginales.ContainsKey(aAnimar[i]) ? escalasOriginales[aAnimar[i]] : Vector3.one;
            LeanTween.scale(aAnimar[i], ef, 0.45f).setEase(LeanTweenType.easeOutBack).setDelay(0.05f * i);
        }
    }

    public void AnimarPumCrono()
    {
        List<GameObject> aAnimar = new List<GameObject>();
        GameObject[] posibles = { panelPrincipalCrono, panelSecundarioCrono, panelBotonesCrono };
        foreach (GameObject obj in posibles)
        {
            if (obj != null) { aAnimar.Add(obj); obj.transform.localScale = Vector3.zero; }
        }
        for (int i = 0; i < aAnimar.Count; i++)
        {
            Vector3 ef = escalasOriginales.ContainsKey(aAnimar[i]) ? escalasOriginales[aAnimar[i]] : Vector3.one;
            LeanTween.scale(aAnimar[i], ef, 0.45f).setEase(LeanTweenType.easeOutBack).setDelay(0.08f * i);
        }
    }

    public void ActualizarElementosRegistro(bool hay, bool conPum)
    {
        if (botonAnadirGrande) botonAnadirGrande.SetActive(!hay);
        if (panelBotonesPequenos) panelBotonesPequenos.SetActive(hay);
        if (botonEmpezar) botonEmpezar.SetActive(hay);
        if (conPum) AnimarPum();
    }

    public void AbrirTarjetaRegistro()
    {
        if (panelTarjetaRegistro == null || rectTarjetaRegistro == null) return;

        panelTarjetaRegistro.SetActive(true);

        rectTarjetaRegistro.anchoredPosition = new Vector2(rectTarjetaRegistro.anchoredPosition.x, posCerradaY);
        LeanTween.moveY(rectTarjetaRegistro, posAbiertaY, 0.4f).setEase(LeanTweenType.easeOutBack);

        if (fondoBlur != null)
        {
            fondoBlur.SetActive(true);
            CanvasGroup cg = fondoBlur.GetComponent<CanvasGroup>();
            if (cg == null) cg = fondoBlur.AddComponent<CanvasGroup>();

            cg.alpha = 0f;
            LeanTween.alphaCanvas(cg, 1f, 0.4f);
        }
    }

    public void CerrarTarjetaRegistro()
    {
        if (panelTarjetaRegistro == null || rectTarjetaRegistro == null) return;

        LeanTween.moveY(rectTarjetaRegistro, posCerradaY, 0.3f).setEase(LeanTweenType.easeInBack).setOnComplete(() => {
            panelTarjetaRegistro.SetActive(false);
            if (Avisos.instance != null) Avisos.instance.ActualizarInterfazSegunContador(false);
        });

        if (fondoBlur != null)
        {
            CanvasGroup cg = fondoBlur.GetComponent<CanvasGroup>();
            if (cg != null)
            {
                LeanTween.alphaCanvas(cg, 0f, 0.3f).setOnComplete(() => fondoBlur.SetActive(false));
            }
            else fondoBlur.SetActive(false);
        }
    }
}