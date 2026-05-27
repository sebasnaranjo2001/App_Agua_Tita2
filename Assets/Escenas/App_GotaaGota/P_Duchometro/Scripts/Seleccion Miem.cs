using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SeleccionMiembros : MonoBehaviour
{
    [Header("Referencias de la Tarjeta")]
    public Image imagenFondo;
    public TMP_Text textoNombre;
    public TMP_Text textoEdad;
    public TMP_Text textoDuchas;

    private Color colorBaseDeEstaTarjeta;
    private bool estaInicializado = false;

    void Start()
    {
        // Guardamos el color crema/base exacto que tú le pusiste en el Inspector
        if (imagenFondo != null)
        {
            colorBaseDeEstaTarjeta = imagenFondo.color;
            estaInicializado = true;
        }
    }

    public void SeleccionarEsteMiembro()
    {
        if (Avisos.instance != null) Avisos.instance.RegistrarSeleccion(this);

        // Oscurecemos la tarjeta multiplicando su color original por 0.7f
        if (estaInicializado && imagenFondo != null)
        {
            imagenFondo.color = colorBaseDeEstaTarjeta * 0.7f;
        }

        Debug.Log("Has seleccionado a: " + gameObject.name);
    }

    public void Deseleccionar()
    {
        // Le quitamos el "filtro" oscuro y la dejamos normal
        if (estaInicializado && imagenFondo != null)
        {
            imagenFondo.color = colorBaseDeEstaTarjeta;
        }
    }
}