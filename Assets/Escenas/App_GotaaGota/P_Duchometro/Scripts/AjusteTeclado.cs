using UnityEngine;

public class AjusteTeclado : MonoBehaviour
{
    [Header("--- REFERENCIAS ---")]
    public RectTransform panelTarjeta;

    [Header("--- POSICIONES Y ---")]
    public float posAbiertaNormalY = -104.14f; // Su posición original
    public float posConTecladoY = 400f;        // La posición alta para esquivar el teclado

    private bool tecladoAbierto = false;

    void Update()
    {
        // Esta función nativa detecta mágicamente si el teclado de Android/iOS está en pantalla
        if (TouchScreenKeyboard.visible && !tecladoAbierto)
        {
            tecladoAbierto = true;
            LeanTween.cancel(panelTarjeta.gameObject);
            // Sube el panel suavemente
            LeanTween.moveY(panelTarjeta, posConTecladoY, 0.25f).setEaseOutQuad();
        }
        else if (!TouchScreenKeyboard.visible && tecladoAbierto)
        {
            tecladoAbierto = false;

            // Lo devuelve a su lugar original solo si no lo estamos cerrando con el dedo
            if (panelTarjeta.anchoredPosition.y > posAbiertaNormalY)
            {
                LeanTween.cancel(panelTarjeta.gameObject);
                LeanTween.moveY(panelTarjeta, posAbiertaNormalY, 0.25f).setEaseOutQuad();
            }
        }
    }
}