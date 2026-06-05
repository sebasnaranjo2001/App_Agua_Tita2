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

    private Color colorBaseDeEstaTarjeta;
    private bool estaInicializado = false;

    void Start()
    {
        if (imagenFondo != null)
        {
            colorBaseDeEstaTarjeta = imagenFondo.color;
            estaInicializado = true;
        }
    }

    public void SeleccionarEsteMiembro()
    {
        if (Avisos.instance != null) Avisos.instance.RegistrarSeleccion(this);

        if (estaInicializado && imagenFondo != null)
        {
            imagenFondo.color = colorBaseDeEstaTarjeta * 0.7f;
        }

        Debug.Log("Has seleccionado a: " + gameObject.name);
    }

    public void Deseleccionar()
    {
        if (estaInicializado && imagenFondo != null)
        {
            imagenFondo.color = colorBaseDeEstaTarjeta;
        }
    }

    // --- NUEVA FUNCIÓN: APLICA LOS COLORES SEGÚN EL ÍNDICE ---
    public void AplicarTema(int indiceTema)
    {
        Color colFondo = Color.white;
        Color colTexto = Color.black;
        Color colInsignia = Color.gray;
        Color colTextoInsignia = Color.white; // Siempre blanco

        switch (indiceTema)
        {
            case 0: // Kit 2: Bosque
                ColorUtility.TryParseHtmlString("#B7E4C7", out colFondo);
                ColorUtility.TryParseHtmlString("#1B4332", out colTexto);
                ColorUtility.TryParseHtmlString("#2C6A4D", out colInsignia);
                break;
            case 1: // Kit 3: Amanecer
                ColorUtility.TryParseHtmlString("#FFD6A5", out colFondo);
                ColorUtility.TryParseHtmlString("#5B3E35", out colTexto);
                ColorUtility.TryParseHtmlString("#855631", out colInsignia);
                break;
            case 2: // Kit 4: Lavanda
                ColorUtility.TryParseHtmlString("#CDB4DB", out colFondo);
                ColorUtility.TryParseHtmlString("#3C096C", out colTexto);
                ColorUtility.TryParseHtmlString("#421D61", out colInsignia);
                break;
            case 3: // Kit 5: Sandía
                ColorUtility.TryParseHtmlString("#FFC8DD", out colFondo);
                ColorUtility.TryParseHtmlString("#590D22", out colTexto);
                ColorUtility.TryParseHtmlString("#840C2E", out colInsignia);
                break;
            case 4: // Kit 6: Girasol
                ColorUtility.TryParseHtmlString("#FFF3B0", out colFondo);
                ColorUtility.TryParseHtmlString("#523906", out colTexto);
                ColorUtility.TryParseHtmlString("#916525", out colInsignia);
                break;
            default: // Por si hay error, usa el bosque por defecto
                ColorUtility.TryParseHtmlString("#B7E4C7", out colFondo);
                ColorUtility.TryParseHtmlString("#1B4332", out colTexto);
                ColorUtility.TryParseHtmlString("#2C6A4D", out colInsignia);
                break;
        }

        // Aplicamos los colores a los componentes de la tarjeta
        if (imagenFondo != null)
        {
            imagenFondo.color = colFondo;
            colorBaseDeEstaTarjeta = colFondo; // Actualizamos para que la selección (oscurecer) funcione bien
            estaInicializado = true;
        }

        if (textoNombre != null) textoNombre.color = colTexto;
        if (textoEdad != null) textoEdad.color = colTexto;

        if (panelInsignia != null) panelInsignia.color = colInsignia;
        if (textoDuchas != null) textoDuchas.color = colTextoInsignia;
    }
}