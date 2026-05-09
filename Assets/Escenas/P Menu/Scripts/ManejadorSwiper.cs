using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ManejadorSwiper : MonoBehaviour, IDragHandler, IEndDragHandler
{
    [Header("Referencias de la UI")]
    public RectTransform contenedor;
    public TextMeshProUGUI txtFrase;

    [Header("Indicadores (3 puntos)")]
    public LayoutElement[] puntos;
    public Color colorActivo = Color.white;
    public Color colorInactivo = new Color(1f, 1f, 1f, 0.3f);

    [Header("Configuración")]
    [TextArea(3, 10)] public string[] frases;
    public float sensibilidadSwipe = 20f;

    [Header("Ajustes Visuales Puntos")]
    public float tamañoPuntoActivo = 60f;
    public float tamañoPuntoInactivo = 40f;

    private int indexActual = 0;
    private Vector2 posInicialContenedor;
    private float pasoTotal = 0;

    void Start()
    {
        // Cambiar frase aleatoria
        if (frases.Length > 0 && txtFrase != null)
            txtFrase.text = frases[Random.Range(0, frases.Length)];

        // Guardamos la posición inicial EXACTA tal cual está en el editor
        if (contenedor != null)
        {
            posInicialContenedor = contenedor.anchoredPosition;
        }

        ActualizarIndicadores(true);
    }

    public void OnDrag(PointerEventData eventData) { } // Obligatorio para detectar swipe

    public void OnEndDrag(PointerEventData eventData)
    {
        // Si no hemos calculado el paso todavía, lo hacemos al primer toque
        if (pasoTotal <= 0) CalcularPaso();

        float diferenciaX = eventData.pressPosition.x - eventData.position.x;

        // Movimiento a la izquierda (Siguiente)
        if (diferenciaX > sensibilidadSwipe && indexActual < puntos.Length - 1)
        {
            indexActual++;
            MoverContenedor();
        }
        // Movimiento a la derecha (Anterior)
        else if (diferenciaX < -sensibilidadSwipe && indexActual > 0)
        {
            indexActual--;
            MoverContenedor();
        }
    }

    void CalcularPaso()
    {
        HorizontalLayoutGroup layout = contenedor.GetComponent<HorizontalLayoutGroup>();
        if (layout != null && contenedor.childCount > 0)
        {
            float anchoTarjeta = contenedor.GetChild(0).GetComponent<RectTransform>().rect.width;
            pasoTotal = anchoTarjeta + layout.spacing;
        }
        else
        {
            // Valor de respaldo si falla la detección (ajustar si tus tarjetas son distintas)
            pasoTotal = 800f;
        }
    }

    void MoverContenedor()
    {
        // Lógica de posición basada en tu configuración: Pivot X = 0
        float nuevaX = posInicialContenedor.x - (indexActual * pasoTotal);

        LeanTween.cancel(contenedor.gameObject);
        LeanTween.move(contenedor, new Vector2(nuevaX, contenedor.anchoredPosition.y), 0.5f)
            .setEase(LeanTweenType.easeOutBack);

        ActualizarIndicadores(false);
    }

    void ActualizarIndicadores(bool instantaneo)
    {
        if (puntos == null || puntos.Length == 0) return;

        for (int i = 0; i < puntos.Length; i++)
        {
            if (puntos[i] == null) continue;

            bool esActivo = (i == indexActual);
            float tamañoObjetivo = esActivo ? tamañoPuntoActivo : tamañoPuntoInactivo;
            Color colorObjetivo = esActivo ? colorActivo : colorInactivo;
            float tiempo = instantaneo ? 0f : 0.3f;

            LayoutElement el = puntos[i];
            Image img = el.GetComponent<Image>();

            LeanTween.cancel(el.gameObject);
            LeanTween.value(el.gameObject, el.preferredHeight, tamañoObjetivo, tiempo)
                .setOnUpdate((float val) => {
                    el.preferredHeight = val;
                    el.preferredWidth = val;
                });

            if (img != null) LeanTween.color(img.rectTransform, colorObjetivo, tiempo);
        }
    }
}