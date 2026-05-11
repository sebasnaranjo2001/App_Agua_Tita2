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

    [Space(5)]
    public Sprite iconInicioNormal, iconInicioSelected;
    public Sprite iconJuegosNormal, iconJuegosSelected;
    public Sprite iconGuiaNormal, iconGuiaSelected;

    [Header("--- SISTEMA VISUAL (FONDOS) ---")]
    public Image fondoPrincipal;
    public Sprite fondoInicio, fondoJuegos, fondoGuia, fondoDuchometro;

    [Header("--- SECCIÓN INICIO (PUM) ---")]
    public GameObject logoApp;
    public GameObject seccionAvisos;
    public GameObject seccionBotonDuchometro; // El que está en el menú
    public GameObject seccionTarjetas;

    [Header("--- SECCIÓN GUÍA (PUM) ---")]
    public GameObject logoGuia;
    public GameObject tituloGuia, subtituloGuia;
    public GameObject zonaBano, zonaCocina, zonaLavanderia, zonaJardin;

    [Header("--- SECCIÓN JUEGOS (PUM) ---")]
    public GameObject logoJuegos;
    public GameObject tituloJuegos, subtituloJuegos;
    public GameObject itemJ1, itemJ2, itemJ3;

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
            logoApp, seccionAvisos, seccionBotonDuchometro, seccionTarjetas,
            logoGuia, tituloGuia, subtituloGuia, zonaBano, zonaCocina, zonaLavanderia, zonaJardin,
            logoJuegos, tituloJuegos, subtituloJuegos, itemJ1, itemJ2, itemJ3
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

    // --- MÉTODOS DE NAVEGACIÓN (CON ANIMACIONES RESTAURADAS) ---

    public void AbrirPanelDuchometro()
    {
        if (seccionActual == "duchometro") return;
        seccionActual = "duchometro";
        ActualizarEstadoBotones("duchometro");
        EjecutarTransicionFondo(fondoDuchometro, panelDuchometro);
        // Nota: Los elementos internos los animará tu nuevo script manejador
    }

    public void AbrirPanelJuegos()
    {
        if (seccionActual == "juegos") return;
        seccionActual = "juegos";
        ActualizarEstadoBotones("juegos");
        EjecutarTransicionFondo(fondoJuegos, panelJuegos);
        AnimarEntradaJuegos(); // RESTAURADO
    }

    public void AbrirPanelGuia()
    {
        if (seccionActual == "guia") return;
        seccionActual = "guia";
        ActualizarEstadoBotones("guia");
        EjecutarTransicionFondo(fondoGuia, panelGuia);
        AnimarEntradaGuia(); // RESTAURADO
    }

    public void MostrarInicio()
    {
        if (seccionActual == "inicio") return;
        seccionActual = "inicio";
        ActualizarEstadoBotones("inicio");
        EjecutarTransicionFondo(fondoInicio, panelInicio);
        AnimarEntradaInicio(); // RESTAURADO
    }

    // --- BLOQUE DE ANIMACIONES PUM ---

    private void AnimarEntradaInicio()
    {
        SetScaleZero(logoApp, seccionAvisos, seccionBotonDuchometro, seccionTarjetas);
        Pop(logoApp, 0.5f, 0.05f);
        Pop(seccionAvisos, 0.4f, 0.15f);
        Pop(seccionBotonDuchometro, 0.4f, 0.22f);
        Pop(seccionTarjetas, 0.4f, 0.30f);
    }

    private void AnimarEntradaGuia()
    {
        SetScaleZero(logoGuia, tituloGuia, subtituloGuia, zonaBano, zonaCocina, zonaLavanderia, zonaJardin);
        Pop(logoGuia, 0.6f, 0.0f);
        Pop(tituloGuia, 0.4f, 0.12f);
        Pop(subtituloGuia, 0.4f, 0.18f);
        Pop(zonaBano, 0.4f, 0.25f);
        Pop(zonaCocina, 0.4f, 0.31f);
        Pop(zonaLavanderia, 0.4f, 0.37f);
        Pop(zonaJardin, 0.4f, 0.43f);
    }

    private void AnimarEntradaJuegos()
    {
        SetScaleZero(logoJuegos, tituloJuegos, subtituloJuegos, itemJ1, itemJ2, itemJ3);
        Pop(logoJuegos, 0.6f, 0.0f);
        Pop(tituloJuegos, 0.4f, 0.12f);
        Pop(subtituloJuegos, 0.4f, 0.18f);
        Pop(itemJ1, 0.4f, 0.25f);
        Pop(itemJ2, 0.4f, 0.31f);
        Pop(itemJ3, 0.4f, 0.37f);
    }

    // --- MÉTODOS HERRAMIENTA ---

    private void ActualizarEstadoBotones(string seccionActiva)
    {
        if (btnInicio) btnInicio.image.sprite = (seccionActiva == "inicio") ? iconInicioSelected : iconInicioNormal;
        if (btnJuegos) btnJuegos.image.sprite = (seccionActiva == "juegos") ? iconJuegosSelected : iconJuegosNormal;
        if (btnGuia) btnGuia.image.sprite = (seccionActiva == "guia") ? iconGuiaSelected : iconGuiaNormal;

        GestionarEscalaBoton(btnInicio.transform, seccionActiva == "inicio");
        GestionarEscalaBoton(btnJuegos.transform, seccionActiva == "juegos");
        GestionarEscalaBoton(btnGuia.transform, seccionActiva == "guia");
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