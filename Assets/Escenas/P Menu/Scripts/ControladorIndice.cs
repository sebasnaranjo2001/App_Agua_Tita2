using UnityEngine;
using UnityEngine.UI;

public class ControladorIndice : MonoBehaviour
{
    [Header("Configuración del Índice")]
    public GameObject listaOpciones;
    public Image imgBotonCabecera;
    public Sprite spriteSeleccionaUno;
    public GameObject iconoFlecha;
    private bool indiceAbierto = false;

    [Header("Sprites de los Temas")]
    public Sprite[] spritesTemas; // Una lista de tus imágenes de Illustrator

    [Header("Paneles de Contenido")]
    public GameObject[] panelesContenido; // Una lista de tus objetos de info

    void Start()
    {
        // Al empezar, que siempre pida seleccionar uno
        PrepararInicio();
    }

    public void PrepararInicio()
    {
        indiceAbierto = true;
        listaOpciones.SetActive(true);
        if (iconoFlecha) iconoFlecha.SetActive(false);
        imgBotonCabecera.sprite = spriteSeleccionaUno;
        CerrarTodosLosContenidos();

        // Animación PUM inicial
        listaOpciones.transform.localScale = Vector3.zero;
        LeanTween.scale(listaOpciones, Vector3.one, 0.4f).setEase(LeanTweenType.easeOutBack);
    }

    public void ToggleIndice()
    {
        indiceAbierto = !indiceAbierto;
        listaOpciones.SetActive(indiceAbierto);

        if (indiceAbierto)
        {
            imgBotonCabecera.sprite = spriteSeleccionaUno;
            if (iconoFlecha) iconoFlecha.SetActive(false);
            CerrarTodosLosContenidos();

            listaOpciones.transform.localScale = Vector3.zero;
            LeanTween.scale(listaOpciones, Vector3.one, 0.4f).setEase(LeanTweenType.easeOutBack);
        }
        else
        {
            if (iconoFlecha) iconoFlecha.SetActive(true);
        }
    }

    // Esta función es mágica: sirve para CUALQUIER botón
    // Solo le pasas el número del botón (0, 1, 2, 3...)
    public void SeleccionarOpcion(int indice)
    {
        if (imgBotonCabecera) imgBotonCabecera.sprite = spritesTemas[indice];

        indiceAbierto = false;
        listaOpciones.SetActive(false);
        if (iconoFlecha) iconoFlecha.SetActive(true);

        CerrarTodosLosContenidos();
        GameObject panelAMostrar = panelesContenido[indice];
        panelAMostrar.SetActive(true);

        panelAMostrar.transform.localScale = Vector3.zero;
        LeanTween.scale(panelAMostrar, Vector3.one, 0.3f).setEase(LeanTweenType.easeOutBack);
    }

    private void CerrarTodosLosContenidos()
    {
        foreach (GameObject go in panelesContenido)
        {
            if (go != null) go.SetActive(false);
        }
    }
}