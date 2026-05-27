using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class NavegacionMenuPrincipal : MonoBehaviour
{
    public static string panelAbridor = "";
    private string seccionActual = "";

    private Dictionary<GameObject, Vector3> escalasOriginales = new Dictionary<GameObject, Vector3>();

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
    public RectTransform burbujaSeleccion; // Arrastra aquí la imagen de tu burbuja
    public float tiempoMovimiento = 0.35f;

    [Space(5)]
    public float posXInicio;   // Posición X para Inicio
    public float posXJuegos;   // Posición X para Juegos
    public float posXGuia;     // Posición X para Guía

    [Space(5)]
    // CORRECCIÓN: Colores asignados matemáticamente para evitar el error de compilación
    public Color colorBurbujaInicio = new Color(78f / 255f, 168f / 255f, 222f / 255f); // #4EA8DE
    public Color colorBurbujaJuegos = new Color(160f / 255f, 132f / 255f, 232f / 255f); // #A084E8
    public Color colorBurbujaGuia = new Color(244f / 255f, 162f / 255f, 97f / 255f);   // #F4A261

    [Header("--- SISTEMA VISUAL (FONDOS) ---")]
    public Image fondoPrincipal;
    public Sprite fondoInicio, fondoJuegos, fondoGuia, fondoDuchometro;

    [Header("--- ELEMENTOS INICIO (PUM) ---")]
    public GameObject seccionAvisos;
    public GameObject seccionBotonDuchometro;
    public GameObject seccionTarjetas;

    [Header("--- ELEMENTOS GUÍA (PUM) ---")]
    public GameObject tituloGuia, subtituloGuia;
    public GameObject zonaBano, zonaCocina, zonaLavanderia, zonaJardin;

    [Header("--- ELEMENTOS JUEGOS (PUM) ---")]
    public GameObject tituloJuegos, subtituloJuegos;
    public GameObject itemJ1, itemJ2, itemJ3;

    [Header("--- ELEMENTOS DUCHOMETRO (PUM) ---")]
    public GameObject tituloDuchometro, subtituloDuchometro, barraInternaDuchometro;

    void Start()
    {
        RegistrarEscalas();
        ConfigurarPanelInicial();

        if (btnDuchometroMenu != null)
            btnDuchometroMenu.onClick.AddListener(AbrirPanelDuchometro);
    }

    void RegistrarEscalas()
    {
        GameObject[] todosLosObjetos = {
            seccionAvisos, seccionBotonDuchometro, seccionTarjetas,
            tituloGuia, subtituloGuia, zonaBano, zonaCocina, zonaLavanderia, zonaJardin,
            tituloJuegos, subtituloJuegos, itemJ1, itemJ2, itemJ3,
            tituloDuchometro, subtituloDuchometro, barraInternaDuchometro
        };

        foreach (GameObject obj in todosLosObjetos)
        {
            if (obj != null && !escalasOriginales.ContainsKey(obj))
            {
                escalasOriginales.Add(obj, obj.transform.localScale);
            }
        }
    }

    void ConfigurarPanelInicial()
    {
        if (string.IsNullOrEmpty(panelAbridor)) { MostrarInicio(); return; }
        if (panelAbridor == "juegos") AbrirPanelJuegos();
        else if (panelAbridor == "guia") AbrirPanelGuia();
        else if (panelAbridor == "duchometro") AbrirPanelDuchometro();
        else MostrarInicio();
    }

    // --- MÉTODOS DE NAVEGACIÓN ---

    public void AbrirPanelDuchometro()
    {
        if (seccionActual == "duchometro") return;
        seccionActual = "duchometro";
        ActualizarEstadoBotones("duchometro");
        EjecutarTransicionFondo(fondoDuchometro, panelDuchometro);

        SetScaleZero(tituloDuchometro, subtituloDuchometro, barraInternaDuchometro);
        Pop(tituloDuchometro, 0.4f, 0.12f);
        Pop(subtituloDuchometro, 0.4f, 0.18f);
        Pop(barraInternaDuchometro, 0.4f, 0.25f);
    }

    public void AbrirPanelJuegos()
    {
        if (seccionActual == "juegos") return;
        seccionActual = "juegos";
        ActualizarEstadoBotones("juegos");
        EjecutarTransicionFondo(fondoJuegos, panelJuegos);

        SetScaleZero(tituloJuegos, subtituloJuegos, itemJ1, itemJ2, itemJ3);
        Pop(tituloJuegos, 0.4f, 0.12f);
        Pop(subtituloJuegos, 0.4f, 0.18f);
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

        SetScaleZero(tituloGuia, subtituloGuia, zonaBano, zonaCocina, zonaLavanderia, zonaJardin);
        Pop(tituloGuia, 0.4f, 0.12f);
        Pop(subtituloGuia, 0.4f, 0.18f);
        Pop(zonaBano, 0.4f, 0.25f);
        Pop(zonaCocina, 0.4f, 0.31f);
        Pop(zonaLavanderia, 0.4f, 0.37f);
        Pop(zonaJardin, 0.4f, 0.43f);
    }

    public void MostrarInicio()
    {
        if (seccionActual == "inicio") return;
        seccionActual = "inicio";
        ActualizarEstadoBotones("inicio");
        EjecutarTransicionFondo(fondoInicio, panelInicio);

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
}