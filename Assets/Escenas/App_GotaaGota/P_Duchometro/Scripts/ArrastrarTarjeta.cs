using UnityEngine;
using UnityEngine.EventSystems;

public class ArrastrarTarjeta : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    [Header("--- REFERENCIAS ---")]
    public RectTransform panelTarjeta;
    public ManejadorNavegacion navegador;
    public Canvas canvasPrincipal;           // <-- NUEVO: Para arreglar la velocidad del mouse

    [Header("--- CONFIGURACIÓN EXACTA ---")]
    public float posicionYParaCerrar = -300f; // <-- NUEVO: Medida exacta en Y para cerrarse
    public float posAbiertaY = -104.14f;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (panelTarjeta != null)
        {
            LeanTween.cancel(panelTarjeta.gameObject);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (panelTarjeta == null || canvasPrincipal == null) return;

        // SOLUCIÓN DE VELOCIDAD: Dividimos el movimiento del mouse por la escala de tu Canvas
        // Así el panel se pegará a tu cursor 1 a 1, sin ir más rápido o más lento
        float movimientoCorregido = eventData.delta.y / canvasPrincipal.scaleFactor;

        float nuevaPosY = panelTarjeta.anchoredPosition.y + movimientoCorregido;

        if (nuevaPosY > posAbiertaY)
        {
            nuevaPosY = posAbiertaY;
        }

        panelTarjeta.anchoredPosition = new Vector2(panelTarjeta.anchoredPosition.x, nuevaPosY);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (panelTarjeta == null) return;

        // SOLUCIÓN DE MEDIDA: Si la posición actual es más baja que tu límite, se cierra
        if (panelTarjeta.anchoredPosition.y < posicionYParaCerrar)
        {
            if (navegador != null) navegador.CerrarTarjetaRegistro();
        }
        else
        {
            // Si la soltaste arriba de ese límite, rebota a su posición abierta
            LeanTween.moveY(panelTarjeta, posAbiertaY, 0.3f).setEase(LeanTweenType.easeOutBack);
        }
    }
}