using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class ManejadorSwiperHorizontal : MonoBehaviour, IDragHandler, IEndDragHandler
{
    [Header("Control de Registros (Automático)")]
    public bool hayRegistros = false;
    public GameObject tarjetaLider;

    [Header("Referencias UI")]
    public RectTransform contenedor;
    public List<LayoutElement> puntos;

    [Header("Colores")]
    public Color colorActivo = Color.white;
    public Color colorInactivo = new Color(1f, 1f, 1f, 0.3f);

    [Header("Ajustes de Movimiento")]
    public float anchoTarjeta = 739f;
    public float espacioEntreTarjetas = 50f;
    public float sensibilidadSwipe = 20f;

    [Header("Ajustes Visuales Puntos")]
    public float tamañoPuntoActivo = 60f;
    public float tamañoPuntoInactivo = 40f;

    private int indexActual = 0;
    private Vector2 posInicialContenedor;

    void Start()
    {
        // 1. Primero preguntamos a la memoria si hay datos guardados
        // Buscamos la llave "HayDatosDucha". Si no existe, devuelve 0.
        int registroExiste = PlayerPrefs.GetInt("HayDatosDucha", 0);
        hayRegistros = (registroExiste == 1);

        ConfigurarInicio();

        posInicialContenedor = contenedor.anchoredPosition;
        ActualizarIndicadores(true);
    }

    void ConfigurarInicio()
    {
        // 2. Si la memoria dice que NO hay registros, ocultamos la tarjeta 1 y el punto 1
        if (!hayRegistros)
        {
            if (tarjetaLider != null) tarjetaLider.SetActive(false);

            if (puntos.Count > 0)
            {
                puntos[0].gameObject.SetActive(false);
                puntos.RemoveAt(0); // El sistema de navegación ahora ignora ese punto
            }
        }
    }

    public void OnDrag(PointerEventData eventData) { }

    public void OnEndDrag(PointerEventData eventData)
    {
        float diferenciaX = eventData.pressPosition.x - eventData.position.x;

        // puntos.Count ahora es dinámico (será 2 o 3 dependiendo de los registros)
        if (diferenciaX > sensibilidadSwipe && indexActual < puntos.Count - 1)
        {
            indexActual++;
            Mover();
        }
        else if (diferenciaX < -sensibilidadSwipe && indexActual > 0)
        {
            indexActual--;
            Mover();
        }
    }

    void Mover()
    {
        float nuevaX = posInicialContenedor.x - (indexActual * (anchoTarjeta + espacioEntreTarjetas));

        LeanTween.cancel(contenedor.gameObject);
        LeanTween.move(contenedor, new Vector2(nuevaX, contenedor.anchoredPosition.y), 0.5f)
            .setEase(LeanTweenType.easeOutBack);

        ActualizarIndicadores(false);
    }

    void ActualizarIndicadores(bool instantaneo)
    {
        for (int i = 0; i < puntos.Count; i++)
        {
            bool esActivo = (i == indexActual);
            float tamañoObjetivo = esActivo ? tamañoPuntoActivo : tamañoPuntoInactivo;
            Color colorObjetivo = esActivo ? colorActivo : colorInactivo;
            float tiempo = instantaneo ? 0f : 0.3f;

            LayoutElement el = puntos[i];
            Image img = puntos[i].GetComponent<Image>();

            LeanTween.cancel(el.gameObject);
            LeanTween.value(el.gameObject, el.preferredWidth, tamañoObjetivo, tiempo)
                .setOnUpdate((float val) => {
                    el.preferredWidth = val;
                    el.preferredHeight = val;
                });

            if (img != null) LeanTween.color(img.rectTransform, colorObjetivo, tiempo);
        }
    }
}