using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NavegacionMenuPrincipal : MonoBehaviour
{
    public static string panelAbridor = "";

    [Header("Referencias de Paneles")]
    public GameObject panelInicio;
    public GameObject panelJuegos;
    public GameObject panelGuia;

    [Header("Referencias de Botones (Barra Navegación)")]
    public Button btnInicio;
    public Button btnJuegos;
    public Button btnGuia;

    [Header("Referencias de Textos Estáticos")]
    public TextMeshProUGUI txtGotaAGota;
    public TextMeshProUGUI txtGuiaFamiliar;

    [Header("Fondos y Sprites")]
    public Image fondoPrincipal;
    public Sprite fondoInicio, fondoJuegos, fondoGuia;

    [Header("Colores de Texto")]
    public Color colorTextoInicio = new Color(0.1f, 0.22f, 0.37f);
    public Color colorTextoJuegos = new Color(0.18f, 0.1f, 0.28f);
    public Color colorTextoGuia = new Color(0.29f, 0.17f, 0.04f);

    [Header("Objetos Inicio")]
    public GameObject seccionAvisos;
    public GameObject seccionDuchometro;
    public GameObject seccionTarjetas;

    [Header("Objetos Juegos")]
    public GameObject tituloJuegos;
    public GameObject itemJuego1;
    public GameObject itemJuego2;
    public GameObject itemJuego3;

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

    // --- MÉTODOS DE NAVEGACIÓN ---

    public void AbrirPanelJuegos()
    {
        ActualizarInteractividadBotones("juegos");
        EjecutarTransicionCompleta(fondoJuegos, colorTextoJuegos, panelJuegos);
        AnimarEntradaJuegos();
    }

    public void AbrirPanelGuia()
    {
        ActualizarInteractividadBotones("guia");
        EjecutarTransicionCompleta(fondoGuia, colorTextoGuia, panelGuia);
        // Aquí puedes añadir AnimarEntradaGuia() si quieres
    }

    public void MostrarInicio()
    {
        ActualizarInteractividadBotones("inicio");
        EjecutarTransicionCompleta(fondoInicio, colorTextoInicio, panelInicio);
        AnimarEntradaInicio();
    }

    // --- LÓGICA DE BLOQUEO DE BOTONES ---

    private void ActualizarInteractividadBotones(string seccionActiva)
    {
        // Si el botón NO es de la sección activa, se puede cliquear (true)
        // Si el botón ES de la sección activa, se bloquea (false)
        if (btnInicio != null) btnInicio.interactable = (seccionActiva != "inicio");
        if (btnJuegos != null) btnJuegos.interactable = (seccionActiva != "juegos");
        if (btnGuia != null) btnGuia.interactable = (seccionActiva != "guia");
    }

    // --- MOTOR DE TRANSICIÓN ---

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

    // --- ANIMACIONES SECUENCIALES (EL "PUM") ---

    private void AnimarEntradaInicio()
    {
        SetScaleZero(seccionAvisos, seccionDuchometro, seccionTarjetas);
        float d = 0.08f;
        float t = 0.4f;
        Pop(seccionAvisos, t, 0.1f);
        Pop(seccionDuchometro, t, 0.1f + d);
        Pop(seccionTarjetas, t, 0.1f + (d * 2));
    }

    private void AnimarEntradaJuegos()
    {
        SetScaleZero(tituloJuegos, itemJuego1, itemJuego2, itemJuego3);
        float d = 0.08f;
        float t = 0.4f;
        Pop(tituloJuegos, t, 0.1f);
        Pop(itemJuego1, t, 0.1f + d);
        Pop(itemJuego2, t, 0.1f + (d * 2));
        Pop(itemJuego3, t, 0.1f + (d * 3));
    }

    private void Pop(GameObject obj, float tiempo, float delay)
    {
        if (obj == null) return;
        LeanTween.cancel(obj);
        LeanTween.scale(obj, Vector3.one, tiempo).setEase(LeanTweenType.easeOutBack).setDelay(delay);
    }

    private void SetScaleZero(params GameObject[] objetos)
    {
        foreach (GameObject obj in objetos)
        {
            if (obj != null) obj.transform.localScale = Vector3.zero;
        }
    }

    private void AnimarColorTexto(TextMeshProUGUI texto, Color colorObjetivo)
    {
        if (texto == null) return;
        LeanTween.value(texto.gameObject, texto.color, colorObjetivo, 0.4f)
            .setEase(LeanTweenType.easeInOutQuad)
            .setOnUpdate((Color col) => { texto.color = col; });
    }

    private void DesactivarTodosLosPaneles()
    {
        if (panelInicio != null) panelInicio.SetActive(false);
        if (panelJuegos != null) panelJuegos.SetActive(false);
        if (panelGuia != null) panelGuia.SetActive(false);
    }
}