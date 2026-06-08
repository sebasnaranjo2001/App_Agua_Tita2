using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class NavegacionMenuPrincipal : MonoBehaviour
{
    public static string panelAbridor = "";
    private string seccionActual = "";

    private Dictionary<GameObject, Vector3> escalasOriginales = new Dictionary<GameObject, Vector3>();
    private Dictionary<RectTransform, Vector2> posicionesOriginales = new Dictionary<RectTransform, Vector2>();

    [Header("--- CONFIGURACIÓN DE PANELES ---")]
    public GameObject panelInicio;
    public GameObject panelJuegos;
    public GameObject panelGuia;
    public GameObject panelDuchometro;

    [Header("--- BARRA DE NAVEGACIÓN (BOTONES) ---")]
    public Button btnInicio;
    public Button btnJuegos;
    public Button btnGuia;
    public Button btnDuchometroMenu;

    [Header("--- BURBUJA FLUIDA ---")]
    public RectTransform burbujaSeleccion;
    public float tiempoMovimiento = 0.35f;

    [Space(5)]
    public float posXInicio;
    public float posXJuegos;
    public float posXGuia;

    [Space(5)]
    public Color colorBurbujaInicio = new Color(78f / 255f, 168f / 255f, 222f / 255f);
    public Color colorBurbujaJuegos = new Color(160f / 255f, 132f / 255f, 232f / 255f);
    public Color colorBurbujaGuia = new Color(244f / 255f, 162f / 255f, 97f / 255f);

    [Header("--- SISTEMA VISUAL (FONDOS) ---")]
    public Image fondoPrincipal;
    public Sprite fondoInicio, fondoJuegos, fondoGuia, fondoDuchometro;

    [Header("--- ENCABEZADOS NUEVOS (APARICIÓN SUAVE) ---")]
    public RectTransform encabezadoInicio;
    public RectTransform encabezadoJuegos;
    public RectTransform encabezadoGuia;
    public RectTransform encabezadoDuchometro;

    [Header("--- ELEMENTOS INICIO (PUM) ---")]
    public GameObject seccionAvisos;
    public GameObject seccionBotonDuchometro;
    public GameObject seccionTarjetas;

    [Header("--- ELEMENTOS GUÍA NUEVOS (PUM) ---")]
    public GameObject descripcionGeneralGuia;
    public GameObject btnBano;
    public GameObject btnCocina;
    public GameObject btnLavanderia;
    public GameObject btnJardin;

    [Header("--- ELEMENTOS JUEGOS (PUM) ---")]
    public GameObject itemJ1, itemJ2, itemJ3;

    [Header("--- ELEMENTOS DUCHOMETRO (PUM) ---")]
    public GameObject barraInternaDuchometro;

    [Header("--- CONEXIÓN CON VIDEO 1 ---")]
    public GameObject panelVideoGota;
    public ControladorVideoGota scriptVideoGota;

    // 👇 AQUÍ ESTÁ LO NUEVO QUE SE AGREGÓ 👇
    [Header("--- CONEXIÓN CON VIDEO 2 ---")]
    public GameObject panelVideo2; // Arrastra tu SEGUNDO Panel de Video aquí
    public ControladorVideoGota scriptVideo2; // Arrastra el script del segundo panel aquí

    void Start()
    {
        RegistrarEscalasYPosiciones();
        ConfigurarPanelInicial();

        if (btnDuchometroMenu != null)
            btnDuchometroMenu.onClick.AddListener(AbrirPanelDuchometro);
    }

    void RegistrarEscalasYPosiciones()
    {
        GameObject[] todosLosObjetos = {
            seccionAvisos, seccionBotonDuchometro, seccionTarjetas,
            descripcionGeneralGuia, btnBano, btnCocina, btnLavanderia, btnJardin,
            itemJ1, itemJ2, itemJ3,
            barraInternaDuchometro
        };

        foreach (GameObject obj in todosLosObjetos)
        {
            if (obj != null && !escalasOriginales.ContainsKey(obj))
            {
                escalasOriginales.Add(obj, obj.transform.localScale);
            }
        }

        if (encabezadoInicio != null && !posicionesOriginales.ContainsKey(encabezadoInicio))
            posicionesOriginales.Add(encabezadoInicio, encabezadoInicio.anchoredPosition);

        if (encabezadoJuegos != null && !posicionesOriginales.ContainsKey(encabezadoJuegos))
            posicionesOriginales.Add(encabezadoJuegos, encabezadoJuegos.anchoredPosition);

        if (encabezadoGuia != null && !posicionesOriginales.ContainsKey(encabezadoGuia))
            posicionesOriginales.Add(encabezadoGuia, encabezadoGuia.anchoredPosition);

        if (encabezadoDuchometro != null && !posicionesOriginales.ContainsKey(encabezadoDuchometro))
            posicionesOriginales.Add(encabezadoDuchometro, encabezadoDuchometro.anchoredPosition);
    }

    void ConfigurarPanelInicial()
    {
        if (string.IsNullOrEmpty(panelAbridor)) { MostrarInicio(); return; }
        if (panelAbridor == "juegos") AbrirPanelJuegos();
        else if (panelAbridor == "guia") AbrirPanelGuia();
        else if (panelAbridor == "duchometro") AbrirPanelDuchometro();
        else MostrarInicio();
    }

    // --- FUNCIÓN PARA ABRIR EL VIDEO 1 DESDE EL MENÚ ---
    public void BotonAbrirElVideoDesdeMenu()
    {
        if (panelVideoGota != null)
        {
            panelVideoGota.SetActive(true);
        }

        if (scriptVideoGota != null)
        {
            scriptVideoGota.AbrirVideo();
        }
    }

    // 👇 NUEVA FUNCIÓN PARA ABRIR EL VIDEO 2 DESDE EL MENÚ 👇
    public void BotonAbrirVideo2DesdeMenu()
    {
        if (panelVideo2 != null)
        {
            panelVideo2.SetActive(true); // Despierta el segundo panel
        }

        if (scriptVideo2 != null)
        {
            scriptVideo2.AbrirVideo(); // Ejecuta la animación y el play del segundo video
        }
    }

    // --- MÉTODOS DE NAVEGACIÓN ---
    public void AbrirPanelDuchometro()
    {
        if (seccionActual == "duchometro") return;
        seccionActual = "duchometro";
        ActualizarEstadoBotones("duchometro");
        EjecutarTransicionFondo(fondoDuchometro, panelDuchometro);

        AnimarEncabezadoNatural(encabezadoDuchometro, 0.6f, 0.05f);

        SetScaleZero(barraInternaDuchometro);
        Pop(barraInternaDuchometro, 0.4f, 0.22f);
    }

    public void AbrirPanelJuegos()
    {
        if (seccionActual == "juegos") return;
        seccionActual = "juegos";
        ActualizarEstadoBotones("juegos");
        EjecutarTransicionFondo(fondoJuegos, panelJuegos);

        AnimarEncabezadoNatural(encabezadoJuegos, 0.6f, 0.05f);

        SetScaleZero(itemJ1, itemJ2, itemJ3);
        Pop(itemJ1, 0.4f, 0.25f);
        Pop(itemJ2, 0.4f, 0.31f);
        Pop(itemJ3, 0.4f, 0.37f);
    }

    public void AbrirPanelGuia()
    {
        if (seccionActual == "guia") return;
        seccionActual = "guia";
        ActualizarEstadoBotones("guia");
        EjecutarTransicionFondo(fondoGuia, panelGuia);

        AnimarEncabezadoNatural(encabezadoGuia, 0.6f, 0.05f);

        SetScaleZero(descripcionGeneralGuia, btnBano, btnCocina, btnLavanderia, btnJardin);

        Pop(descripcionGeneralGuia, 0.4f, 0.18f);
        Pop(btnBano, 0.4f, 0.25f);
        Pop(btnCocina, 0.4f, 0.31f);
        Pop(btnLavanderia, 0.4f, 0.37f);
        Pop(btnJardin, 0.4f, 0.43f);
    }

    public void MostrarInicio()
    {
        if (seccionActual == "inicio") return;
        seccionActual = "inicio";
        ActualizarEstadoBotones("inicio");
        EjecutarTransicionFondo(fondoInicio, panelInicio);

        AnimarEncabezadoNatural(encabezadoInicio, 0.6f, 0.05f);

        SetScaleZero(seccionAvisos, seccionBotonDuchometro, seccionTarjetas);
        Pop(seccionAvisos, 0.4f, 0.15f);
        Pop(seccionBotonDuchometro, 0.4f, 0.22f);
        Pop(seccionTarjetas, 0.4f, 0.30f);
    }

    // --- MÉTODOS HERRAMIENTA ---
    private void ActualizarEstadoBotones(string seccionActiva)
    {
        GestionarEscalaBoton(btnInicio.transform, seccionActiva == "inicio");
        GestionarEscalaBoton(btnJuegos.transform, seccionActiva == "juegos");
        GestionarEscalaBoton(btnGuia.transform, seccionActiva == "guia");

        if (burbujaSeleccion != null && seccionActiva != "duchometro")
        {
            float posDestinoX = burbujaSeleccion.anchoredPosition.x;
            Color colorDestino = Color.white;

            if (seccionActiva == "inicio") { posDestinoX = posXInicio; colorDestino = colorBurbujaInicio; }
            else if (seccionActiva == "juegos") { posDestinoX = posXJuegos; colorDestino = colorBurbujaJuegos; }
            else if (seccionActiva == "guia") { posDestinoX = posXGuia; colorDestino = colorBurbujaGuia; }

            LeanTween.cancel(burbujaSeleccion.gameObject);
            LeanTween.moveX(burbujaSeleccion, posDestinoX, tiempoMovimiento).setEase(LeanTweenType.easeOutBack);

            Image imgBurbuja = burbujaSeleccion.GetComponent<Image>();
            if (imgBurbuja != null)
            {
                LeanTween.color(burbujaSeleccion, colorDestino, tiempoMovimiento);
            }
        }
        else if (burbujaSeleccion != null && seccionActiva == "duchometro")
        {
            LeanTween.scale(burbujaSeleccion.gameObject, Vector3.zero, 0.2f);
        }

        if (burbujaSeleccion != null && seccionActiva != "duchometro")
        {
            LeanTween.scale(burbujaSeleccion.gameObject, Vector3.one, 0.2f);
        }
    }

    private void EjecutarTransicionFondo(Sprite nuevoFondo, GameObject panelDestino)
    {
        if (fondoPrincipal != null && nuevoFondo != null)
        {
            LeanTween.cancel(fondoPrincipal.gameObject);
            LeanTween.alpha(fondoPrincipal.rectTransform, 0.3f, 0.12f).setEase(LeanTweenType.easeInOutQuad).setOnComplete(() => {
                fondoPrincipal.sprite = nuevoFondo;
                LeanTween.alpha(fondoPrincipal.rectTransform, 1f, 0.12f).setEase(LeanTweenType.easeInOutQuad);
            });
        }
        DesactivarTodosLosPaneles();
        if (panelDestino != null) panelDestino.SetActive(true);
    }

    private void DesactivarTodosLosPaneles()
    {
        if (panelInicio) panelInicio.SetActive(false);
        if (panelJuegos) panelJuegos.SetActive(false);
        if (panelGuia) panelGuia.SetActive(false);
        if (panelDuchometro) panelDuchometro.SetActive(false);
    }

    private void GestionarEscalaBoton(Transform t, bool estaActivo)
    {
        if (t == null) return;
        float escalaObjetivo = estaActivo ? 1.15f : 1.0f;
        LeanTween.cancel(t.gameObject);
        LeanTween.scale(t.gameObject, Vector3.one * escalaObjetivo, 0.3f).setEase(LeanTweenType.easeOutBack);
    }

    private void Pop(GameObject obj, float tiempo, float delay)
    {
        if (obj == null) return;
        Vector3 escalaFinal = escalasOriginales.ContainsKey(obj) ? escalasOriginales[obj] : Vector3.one;
        LeanTween.cancel(obj);
        LeanTween.scale(obj, escalaFinal, tiempo).setEase(LeanTweenType.easeOutBack).setDelay(delay);
    }

    private void SetScaleZero(params GameObject[] objetos)
    {
        foreach (GameObject obj in objetos) if (obj != null) obj.transform.localScale = Vector3.zero;
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
}