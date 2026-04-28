using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ManejadorSwiper : MonoBehaviour, IDragHandler, IEndDragHandler
{
    [Header("Referencias de la UI")]
    public RectTransform contenedor;
    public TextMeshProUGUI txtFrase;

    [Header("Indicadores de Progreso")]
    public LayoutElement[] puntos; // Arrastra los 3 puntos aquí
    public Color colorActivo = Color.white;
    public Color colorInactivo = new Color(1f, 1f, 1f, 0.3f); // Blanco con transparencia

    [Header("Configuración del Contenido")]
    [TextArea(3, 10)] public string[] frases;

    [Header("Ajustes de Movimiento")]
    public float altoTarjeta = 573f;
    public float espacioEntreTarjetas = 100f;
    public float sensibilidadSwipe = 20f;

    [Header("Ajustes Visuales Puntos")]
    public float tamañoPuntoActivo = 60f;
    public float tamañoPuntoInactivo = 40f;

    private int indexActual = 0;
    private Vector2 posInicialContenedor;

    void Start()
    {
        if (frases.Length > 0 && txtFrase != null)
            txtFrase.text = frases[Random.Range(0, frases.Length)];

        if (contenedor != null)
            posInicialContenedor = contenedor.anchoredPosition;

        ActualizarIndicadores(true);
    }

    public void OnDrag(PointerEventData eventData) { }

    public void OnEndDrag(PointerEventData eventData)
    {
        float diferenciaY = eventData.pressPosition.y - eventData.position.y;

        if (diferenciaY < -sensibilidadSwipe && indexActual < 2)
        {
            indexActual++;
            MoverContenedor();
        }
        else if (diferenciaY > sensibilidadSwipe && indexActual > 0)
        {
            indexActual--;
            MoverContenedor();
        }
    }

    void MoverContenedor()
    {
        float nuevaY = posInicialContenedor.y + (indexActual * (altoTarjeta + espacioEntreTarjetas));

        LeanTween.cancel(contenedor.gameObject);
        LeanTween.move(contenedor, new Vector2(contenedor.anchoredPosition.x, nuevaY), 0.5f)
            .setEase(LeanTweenType.easeOutBack);

        ActualizarIndicadores(false);
    }

    void ActualizarIndicadores(bool instantaneo)
    {
        for (int i = 0; i < puntos.Length; i++)
        {
            bool esActivo = (i == indexActual);
            float tamañoObjetivo = esActivo ? tamañoPuntoActivo : tamañoPuntoInactivo;
            Color colorObjetivo = esActivo ? colorActivo : colorInactivo;
            float tiempo = instantaneo ? 0f : 0.3f;

            LayoutElement el = puntos[i];
            Image img = puntos[i].GetComponent<Image>();

            // 1. Animamos el Ancho y el Alto al mismo tiempo para que siga siendo un círculo
            LeanTween.cancel(el.gameObject);

            // Animación de tamaño
            LeanTween.value(el.gameObject, el.preferredHeight, tamañoObjetivo, tiempo)
                .setOnUpdate((float val) => {
                    el.preferredHeight = val;
                    el.preferredWidth = val;
                });

            // 2. Animación de color
            if (img != null)
            {
                LeanTween.color(img.rectTransform, colorObjetivo, tiempo);
            }
        }
    }
}