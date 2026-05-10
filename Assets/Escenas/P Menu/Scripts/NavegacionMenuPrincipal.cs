using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NavegacionMenuPrincipal : MonoBehaviour
{
    public static string panelAbridor = "";
    private static bool yaSeAnimoAlEntrar = false;
    private string seccionActual = "";

    [Header("Referencias de Paneles")]
    public GameObject panelInicio;
    public GameObject panelJuegos;
    public GameObject panelGuia;

    [Header("Referencias de Botones")]
    public Button btnInicio;
    public Button btnJuegos;
    public Button btnGuia;

    [Header("Sprites de Iconos")]
    public Sprite iconInicioNormal, iconInicioSelected;
    public Sprite iconJuegosNormal, iconJuegosSelected;
    public Sprite iconGuiaNormal, iconGuiaSelected;

    [Header("Referencias de Textos y Fondos")]
    public TextMeshProUGUI txtGotaAGota;
    public TextMeshProUGUI txtGuiaFamiliar;
    public Image fondoPrincipal;
    public Sprite fondoInicio, fondoJuegos, fondoGuia;

    [Header("Colores de Texto")]
    public Color colorTextoInicio = new Color(0.1f, 0.22f, 0.37f);
    public Color colorTextoJuegos = new Color(0.18f, 0.1f, 0.28f);
    public Color colorTextoGuia = new Color(0.29f, 0.17f, 0.04f);

    [Header("Objetos PUM - Inicio")]
    public GameObject seccionAvisos;
    public GameObject seccionDuchometro;
    public GameObject seccionTarjetas;

    [Header("Objetos PUM - Juegos")]
    public GameObject tituloJuegos, itemJ1, itemJ2, itemJ3;

    [Header("Objetos PUM - Guía")]
    public GameObject tituloGuia;
    public GameObject subtituloGuia;
    public GameObject zonaBano, zonaCocina, zonaLavanderia, zonaJardin;

    void Start()
    {
        ConfigurarPanelInicial();
    }

    void ConfigurarPanelInicial()
    {
        if (string.IsNullOrEmpty(panelAbridor)) { MostrarInicio(); return; }
        if (panelAbridor == "juegos") AbrirPanelJuegos();
        else if (panelAbridor == "guia") AbrirPanelGuia();
        else MostrarInicio();
    }

    public void AbrirPanelJuegos()
    {
        if (seccionActual == "juegos") return;
        seccionActual = "juegos";
        ActualizarEstadoBotones("juegos");
        EjecutarTransicionCompleta(fondoJuegos, colorTextoJuegos, panelJuegos);
        AnimarEntradaJuegos();
    }

    public void AbrirPanelGuia()
    {
        if (seccionActual == "guia") return;
        seccionActual = "guia";
        ActualizarEstadoBotones("guia");
        EjecutarTransicionCompleta(fondoGuia, colorTextoGuia, panelGuia);
        AnimarEntradaGuia(); // <--- Nueva animación
    }

    public void MostrarInicio()
    {
        if (seccionActual == "inicio") return;
        seccionActual = "inicio";
        ActualizarEstadoBotones("inicio");
        EjecutarTransicionCompleta(fondoInicio, colorTextoInicio, panelInicio);
        AnimarEntradaInicio();
    }

    private void ActualizarEstadoBotones(string seccionActiva)
    {
        if (btnInicio) btnInicio.image.sprite = (seccionActiva == "inicio") ? iconInicioSelected : iconInicioNormal;
        if (btnJuegos) btnJuegos.image.sprite = (seccionActiva == "juegos") ? iconJuegosSelected : iconJuegosNormal;
        if (btnGuia) btnGuia.image.sprite = (seccionActiva == "guia") ? iconGuiaSelected : iconGuiaNormal;

        GestionarEscalaBoton(btnInicio.transform, seccionActiva == "inicio");
        GestionarEscalaBoton(btnJuegos.transform, seccionActiva == "juegos");
        GestionarEscalaBoton(btnGuia.transform, seccionActiva == "guia");
    }

    private void GestionarEscalaBoton(Transform t, bool estaActivo)
    {
        if (t == null) return;
        float escalaObjetivo = estaActivo ? 1.15f : 1.0f;
        LeanTween.cancel(t.gameObject);
        LeanTween.scale(t.gameObject, Vector3.one * escalaObjetivo, 0.3f).setEase(LeanTweenType.easeOutBack);
    }

    // --- ANIMACIONES DE ENTRADA (PUM) ---

    private void AnimarEntradaInicio()
    {
        SetScaleZero(seccionAvisos, seccionDuchometro, seccionTarjetas);
        Pop(seccionAvisos, 0.4f, 0.1f);
        Pop(seccionDuchometro, 0.4f, 0.18f);
        Pop(seccionTarjetas, 0.4f, 0.26f);
    }

    private void AnimarEntradaJuegos()
    {
        SetScaleZero(tituloJuegos, itemJ1, itemJ2, itemJ3);
        Pop(tituloJuegos, 0.4f, 0.1f);
        Pop(itemJ1, 0.4f, 0.18f);
        Pop(itemJ2, 0.4f, 0.26f);
        Pop(itemJ3, 0.4f, 0.34f);
    }

    private void AnimarEntradaGuia()
    {
        // Ponemos a cero todos los elementos de la guía
        SetScaleZero(tituloGuia, subtituloGuia, zonaBano, zonaCocina, zonaLavanderia, zonaJardin);

        // Los hacemos aparecer en orden con un pequeño retraso (delay) entre cada uno
        Pop(tituloGuia, 0.4f, 0.1f);
        Pop(subtituloGuia, 0.4f, 0.15f);

        // Las zonas aparecen como una ráfaga
        Pop(zonaBano, 0.4f, 0.22f);
        Pop(zonaCocina, 0.4f, 0.28f);
        Pop(zonaLavanderia, 0.4f, 0.34f);
        Pop(zonaJardin, 0.4f, 0.40f);
    }

    // --- MÉTODOS HERRAMIENTA ---

    private void Pop(GameObject obj, float tiempo, float delay)
    {
        if (obj == null) return;
        LeanTween.cancel(obj);
        LeanTween.scale(obj, Vector3.one, tiempo).setEase(LeanTweenType.easeOutBack).setDelay(delay);
    }

    private void SetScaleZero(params GameObject[] objetos)
    {
        foreach (GameObject obj in objetos) if (obj != null) obj.transform.localScale = Vector3.zero;
    }

    private void EjecutarTransicionCompleta(Sprite nuevoFondo, Color nuevoColorTexto, GameObject panelDestino)
    {
        if (fondoPrincipal != null && nuevoFondo != null)
        {
            LeanTween.cancel(fondoPrincipal.gameObject);
            LeanTween.alpha(fondoPrincipal.rectTransform, 0.3f, 0.2f).setEase(LeanTweenType.easeInOutQuad).setOnComplete(() => {
                fondoPrincipal.sprite = nuevoFondo;
                LeanTween.alpha(fondoPrincipal.rectTransform, 1f, 0.2f).setEase(LeanTweenType.easeInOutQuad);
            });
        }
        AnimarColorTexto(txtGotaAGota, nuevoColorTexto);
        AnimarColorTexto(txtGuiaFamiliar, nuevoColorTexto);
        DesactivarTodosLosPaneles();
        if (panelDestino != null) panelDestino.SetActive(true);
    }

    private void AnimarColorTexto(TextMeshProUGUI texto, Color col)
    {
        if (texto == null) return;
        LeanTween.value(texto.gameObject, texto.color, col, 0.4f).setOnUpdate((Color c) => { texto.color = c; });
    }

    private void DesactivarTodosLosPaneles()
    {
        if (panelInicio) panelInicio.SetActive(false);
        if (panelJuegos) panelJuegos.SetActive(false);
        if (panelGuia) panelGuia.SetActive(false);
    }
}