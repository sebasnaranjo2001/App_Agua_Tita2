using UnityEngine;
using UnityEngine.UI;

public class ControladorIndice : MonoBehaviour
{
    [Header("--- CONFIGURACIÓN DEL ÍNDICE ---")]
    public GameObject panelCabeceraLogo; // <-- NUEVO: Arrastra aquí el panel con el fondo y el logo
    public GameObject listaOpciones;
    public Image imgBotonCabecera;
    public Sprite spriteSeleccionaUno;
    public GameObject iconoFlecha;
    private bool indiceAbierto = false;

    [Header("--- ACTIVOS DE DISEÑO (ILLUSTRATOR) ---")]
    public Sprite[] spritesTemas;

    [Header("--- PANELES DE INFORMACIÓN ---")]
    public GameObject[] panelesContenido;

    void Start()
    {
        PrepararInicio();
    }

    // Se ejecuta cada vez que el panel de la habitación (Baño, Cocina, etc.) se activa
    private void OnEnable()
    {
        float delayAnimacion = 0.15f;

        // 1. SIEMPRE animamos la cabecera del logo al entrar
        if (panelCabeceraLogo != null)
        {
            panelCabeceraLogo.transform.localScale = Vector3.zero;
            LeanTween.scale(panelCabeceraLogo, Vector3.one, 0.4f)
                .setEase(LeanTweenType.easeOutBack);
        }

        // 2. Animamos lo que esté abierto debajo (Lista o Panel de Contenido)
        if (indiceAbierto && listaOpciones != null)
        {
            listaOpciones.transform.localScale = Vector3.zero;
            LeanTween.scale(listaOpciones, Vector3.one, 0.4f)
                .setEase(LeanTweenType.easeOutBack)
                .setDelay(delayAnimacion);
        }
        else if (!indiceAbierto)
        {
            foreach (GameObject go in panelesContenido)
            {
                if (go != null && go.activeSelf)
                {
                    go.transform.localScale = Vector3.zero;
                    LeanTween.scale(go, Vector3.one, 0.4f)
                        .setEase(LeanTweenType.easeOutBack)
                        .setDelay(delayAnimacion);
                }
            }
        }
    }

    public void PrepararInicio()
    {
        indiceAbierto = true;
        if (listaOpciones) listaOpciones.SetActive(true);
        if (iconoFlecha) iconoFlecha.SetActive(false);
        if (imgBotonCabecera) imgBotonCabecera.sprite = spriteSeleccionaUno;

        CerrarTodosLosContenidos();

        // Animación inicial de la cabecera y la lista
        if (panelCabeceraLogo) panelCabeceraLogo.transform.localScale = Vector3.zero;
        if (listaOpciones) listaOpciones.transform.localScale = Vector3.zero;

        LeanTween.scale(panelCabeceraLogo, Vector3.one, 0.4f).setEase(LeanTweenType.easeOutBack);
        LeanTween.scale(listaOpciones, Vector3.one, 0.4f).setEase(LeanTweenType.easeOutBack).setDelay(0.1f);
    }

    public void ToggleIndice()
    {
        indiceAbierto = !indiceAbierto;
        if (listaOpciones) listaOpciones.SetActive(indiceAbierto);

        if (indiceAbierto)
        {
            if (imgBotonCabecera) imgBotonCabecera.sprite = spriteSeleccionaUno;
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

    public void SeleccionarOpcion(int indice)
    {
        if (imgBotonCabecera && indice < spritesTemas.Length)
            imgBotonCabecera.sprite = spritesTemas[indice];

        indiceAbierto = false;
        if (listaOpciones) listaOpciones.SetActive(false);
        if (iconoFlecha) iconoFlecha.SetActive(true);

        CerrarTodosLosContenidos();

        if (indice < panelesContenido.Length && panelesContenido[indice] != null)
        {
            GameObject panelAMostrar = panelesContenido[indice];
            panelAMostrar.SetActive(true);
            panelAMostrar.transform.localScale = Vector3.zero;
            LeanTween.scale(panelAMostrar, Vector3.one, 0.35f).setEase(LeanTweenType.easeOutBack);
        }
    }

    private void CerrarTodosLosContenidos()
    {
        foreach (GameObject go in panelesContenido)
        {
            if (go != null) go.SetActive(false);
        }
    }
}