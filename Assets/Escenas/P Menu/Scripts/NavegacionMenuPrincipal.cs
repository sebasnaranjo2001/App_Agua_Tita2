using UnityEngine;
using UnityEngine.UI;

public class NavegacionMenuPrincipal : MonoBehaviour
{
    [Header("Paneles (RectTransform)")]
    public RectTransform panelJuegos;   // Izquierda
    public RectTransform panelInicio;   // Centro
    public RectTransform panelGuia;     // Derecha

    [Header("Referencias de la Barra")]
    public Image fondoBarra;
    public Button btnJuegos, btnInicio, btnGuia;

    [Header("Configuración Visual")]
    public Color colorJuegos = new Color32(52, 201, 70, 255); // Verde
    public Color colorInicio = new Color32(52, 152, 219, 255); // Azul
    public Color colorGuia = new Color32(241, 196, 15, 255);   // Amarillo

    public float duracion = 0.6f;
    private float distH = 1200f; // Ajusta según tu resolución

    // Variable estática para recordar qué panel abrir al cargar la escena
    public static string panelAbridor = "inicio";

    void Start()
    {
        // Al arrancar, verificamos qué panel debe estar en el centro
        ConfigurarPanelInicial();
    }

    private void ConfigurarPanelInicial()
    {
        if (panelAbridor == "juegos")
        {
            // Posicionamos los paneles para que Juegos esté al centro
            panelJuegos.anchoredPosition = new Vector2(0, 0);
            panelInicio.anchoredPosition = new Vector2(distH, 0);
            panelGuia.anchoredPosition = new Vector2(distH * 2, 0);

            ActualizarInterfaz("juegos");
        }
        else if (panelAbridor == "guia")
        {
            // Posicionamos los paneles para que Guía esté al centro
            panelJuegos.anchoredPosition = new Vector2(-distH * 2, 0);
            panelInicio.anchoredPosition = new Vector2(-distH, 0);
            panelGuia.anchoredPosition = new Vector2(0, 0);

            ActualizarInterfaz("guia");
        }
        else
        {
            // Posicionamiento estándar: Inicio al centro
            panelJuegos.anchoredPosition = new Vector2(-distH, 0);
            panelInicio.anchoredPosition = Vector2.zero;
            panelGuia.anchoredPosition = new Vector2(distH, 0);

            ActualizarInterfaz("inicio");
        }

        // Importante: Resetear la variable para que la próxima vez entre por Inicio
        // a menos que otro script la cambie de nuevo.
        panelAbridor = "inicio";
    }

    public void IrAJuegos()
    {
        MoverPaneles(new Vector2(0, 0), new Vector2(distH, 0), new Vector2(distH * 2, 0));
        ActualizarInterfaz("juegos");
    }

    public void IrAInicio()
    {
        MoverPaneles(new Vector2(-distH, 0), new Vector2(0, 0), new Vector2(distH, 0));
        ActualizarInterfaz("inicio");
    }

    public void IrAGuia()
    {
        MoverPaneles(new Vector2(-distH * 2, 0), new Vector2(-distH, 0), new Vector2(0, 0));
        ActualizarInterfaz("guia");
    }

    private void MoverPaneles(Vector2 posJue, Vector2 posIni, Vector2 posGui)
    {
        LeanTween.cancel(panelJuegos.gameObject);
        LeanTween.cancel(panelInicio.gameObject);
        LeanTween.cancel(panelGuia.gameObject);

        LeanTween.move(panelJuegos, posJue, duracion).setEase(LeanTweenType.easeOutCubic);
        LeanTween.move(panelInicio, posIni, duracion).setEase(LeanTweenType.easeOutCubic);
        LeanTween.move(panelGuia, posGui, duracion).setEase(LeanTweenType.easeOutCubic);
    }

    private void ActualizarInterfaz(string activa)
    {
        // 1. Bloquear/Desbloquear botones
        if (btnJuegos) btnJuegos.interactable = (activa != "juegos");
        if (btnInicio) btnInicio.interactable = (activa != "inicio");
        if (btnGuia) btnGuia.interactable = (activa != "guia");

        // 2. Cambiar color de la barra con transición suave
        Color colorDestino = colorInicio;
        if (activa == "juegos") colorDestino = colorJuegos;
        if (activa == "guia") colorDestino = colorGuia;

        if (fondoBarra != null)
        {
            LeanTween.cancel(fondoBarra.gameObject);
            LeanTween.value(fondoBarra.gameObject, fondoBarra.color, colorDestino, duracion)
                .setOnUpdate((Color c) => fondoBarra.color = c);
        }

        // 3. "Marcar" el botón (Efecto de escala)
        if (btnJuegos) AnimarEscalaBoton(btnJuegos, activa == "juegos");
        if (btnInicio) AnimarEscalaBoton(btnInicio, activa == "inicio");
        if (btnGuia) AnimarEscalaBoton(btnGuia, activa == "guia");
    }

    private void AnimarEscalaBoton(Button btn, bool esActivo)
    {
        float escala = esActivo ? 1.2f : 1.0f;
        LeanTween.cancel(btn.gameObject);
        LeanTween.scale(btn.gameObject, Vector3.one * escala, 0.3f).setEaseOutBack();
    }
}