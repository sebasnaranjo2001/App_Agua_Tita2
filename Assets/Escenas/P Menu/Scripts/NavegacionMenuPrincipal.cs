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

    void Start()
    {
        // Posición inicial: Inicio al centro
        panelJuegos.anchoredPosition = new Vector2(-distH, 0);
        panelInicio.anchoredPosition = Vector2.zero;
        panelGuia.anchoredPosition = new Vector2(distH, 0);

        ActualizarInterfaz("inicio");
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
        LeanTween.move(panelJuegos, posJue, duracion).setEase(LeanTweenType.easeOutCubic);
        LeanTween.move(panelInicio, posIni, duracion).setEase(LeanTweenType.easeOutCubic);
        LeanTween.move(panelGuia, posGui, duracion).setEase(LeanTweenType.easeOutCubic);
    }

    private void ActualizarInterfaz(string activa)
    {
        // 1. Bloquear/Desbloquear botones
        btnJuegos.interactable = (activa != "juegos");
        btnInicio.interactable = (activa != "inicio");
        btnGuia.interactable = (activa != "guia");

        // 2. Cambiar color de la barra con transición suave
        Color colorDestino = colorInicio;
        if (activa == "juegos") colorDestino = colorJuegos;
        if (activa == "guia") colorDestino = colorGuia;

        LeanTween.value(fondoBarra.gameObject, fondoBarra.color, colorDestino, duracion)
            .setOnUpdate((Color c) => fondoBarra.color = c);

        // 3. "Marcar" el botón (Efecto de escala)
        AnimarEscalaBoton(btnJuegos, activa == "juegos");
        AnimarEscalaBoton(btnInicio, activa == "inicio");
        AnimarEscalaBoton(btnGuia, activa == "guia");
    }

    private void AnimarEscalaBoton(Button btn, bool esActivo)
    {
        float escala = esActivo ? 1.2f : 1.0f;
        LeanTween.scale(btn.gameObject, Vector3.one * escala, 0.3f).setEaseOutBack();
    }
}