using UnityEngine;

public class HelpPanel : MonoBehaviour
{
    public GameObject panelAyuda;
    private RectTransform panelRect;

    private void Start()
    {
        panelRect = panelAyuda.GetComponent<RectTransform>();

        panelAyuda.SetActive(false);
    }

    public void AbrirAyuda()
    {
        panelAyuda.SetActive(true);

        // Reinicia la escala
        panelRect.localScale = Vector3.zero;

        // Animacion de entrada con rebote
        LeanTween.scale(panelRect, Vector3.one, 0.4f)
            .setEaseOutBack();
    }

    public void CerrarAyuda()
    {
        // Animacion de salida
        LeanTween.scale(panelRect, Vector3.zero, 0.25f)
            .setEaseInBack()
            .setOnComplete(() =>
            {
                panelAyuda.SetActive(false);
            });
    }
}