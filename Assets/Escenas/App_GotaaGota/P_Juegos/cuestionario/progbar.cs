using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProgressBarUI : MonoBehaviour
{
    [Header("Barra")]
    public Image fillImage;

    [Header("Textos")]
    public TMP_Text textoEstado;
    public TMP_Text textoPorcentaje;

    void Start()
    {
        if (fillImage != null)
        {
            // Inicia vacía
            fillImage.rectTransform.localScale =
                new Vector3(0f, 1f, 1f);
        }
    }

    public void Actualizar(
        int actual,
        int total,
        string prefijo)
    {
        if (total <= 0)
            return;

        float progreso =
            (float)actual / total;

        // Evitar valores fuera de rango
        progreso = Mathf.Clamp01(progreso);

        // Animar la barra
        if (fillImage != null)
        {
            LeanTween.cancel(fillImage.gameObject);

            LeanTween.scaleX(
                fillImage.gameObject,
                progreso,
                0.3f
            ).setEaseOutQuad();
        }

        // Texto: Pregunta 3 de 5 / Fase 2 de 5
        if (textoEstado != null)
        {
            textoEstado.text =
                prefijo + " " +
                actual + " de " +
                total;
        }

        // Texto porcentaje
        if (textoPorcentaje != null)
        {
            textoPorcentaje.text =
                Mathf.RoundToInt(
                    progreso * 100f
                ) + "%";
        }
    }

    public void ReiniciarBarra()
    {
        if (fillImage != null)
        {
            fillImage.rectTransform.localScale =
                new Vector3(0f, 1f, 1f);
        }

        if (textoEstado != null)
        {
            textoEstado.text = "";
        }

        if (textoPorcentaje != null)
        {
            textoPorcentaje.text = "0%";
        }
    }
}