using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SeleccionMiembros : MonoBehaviour
{
    [Header("Referencias de la Tarjeta")]
    public Image imagenFondo;
    public TMP_Text textoNombre;
    public TMP_Text textoEdad;

    [Header("Referencias Insignia")]
    public Image panelInsignia;
    public TMP_Text textoDuchas;

    private CanvasGroup canvasGroup;

    void Start()
    {
        // Buscamos o agregamos automáticamente el CanvasGroup
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    public void SeleccionarEsteMiembro()
    {
        if (Avisos.instance != null) Avisos.instance.RegistrarSeleccion(this);

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0.85f;
        }

        Debug.Log("Has seleccionado a: " + gameObject.name);
    }

    public void Deseleccionar()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
        }
    }

    public void AplicarTema(int indiceTema)
    {
        Color colFondo = Color.white;
        Color colTexto = Color.black;
        Color colInsignia = Color.gray;
        Color colTextoInsignia = Color.white;

        switch (indiceTema)
        {
            case 0:
                ColorUtility.TryParseHtmlString("#B7E4C7", out colFondo);
                ColorUtility.TryParseHtmlString("#1B4332", out colTexto);
                ColorUtility.TryParseHtmlString("#2C6A4D", out colInsignia);
                break;
            case 1:
                ColorUtility.TryParseHtmlString("#FFD6A5", out colFondo);
                ColorUtility.TryParseHtmlString("#5B3E35", out colTexto);
                ColorUtility.TryParseHtmlString("#855631", out colInsignia);
                break;
            case 2:
                ColorUtility.TryParseHtmlString("#CDB4DB", out colFondo);
                ColorUtility.TryParseHtmlString("#3C096C", out colTexto);
                ColorUtility.TryParseHtmlString("#421D61", out colInsignia);
                break;
            case 3:
                ColorUtility.TryParseHtmlString("#FFC8DD", out colFondo);
                ColorUtility.TryParseHtmlString("#590D22", out colTexto);
                ColorUtility.TryParseHtmlString("#840C2E", out colInsignia);
                break;
            case 4:
                ColorUtility.TryParseHtmlString("#FFF3B0", out colFondo);
                ColorUtility.TryParseHtmlString("#523906", out colTexto);
                ColorUtility.TryParseHtmlString("#916525", out colInsignia);
                break;
            default:
                ColorUtility.TryParseHtmlString("#B7E4C7", out colFondo);
                ColorUtility.TryParseHtmlString("#1B4332", out colTexto);
                ColorUtility.TryParseHtmlString("#2C6A4D", out colInsignia);
                break;
        }

        if (imagenFondo != null) imagenFondo.color = colFondo;
        if (textoNombre != null) textoNombre.color = colTexto;
        if (textoEdad != null) textoEdad.color = colTexto;
        if (panelInsignia != null) panelInsignia.color = colInsignia;
        if (textoDuchas != null) textoDuchas.color = colTextoInsignia;
    }
}