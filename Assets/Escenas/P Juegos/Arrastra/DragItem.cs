using UnityEngine;
using UnityEngine.EventSystems;

public class DragItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;

    private Vector2 posicionInicial;
    private Transform padreInicial;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        // Busca el canvas padre automáticamente
        canvas = GetComponentInParent<Canvas>();
    }

    void Start()
    {
        // Guarda posición inicial correcta en UI
        posicionInicial = rectTransform.anchoredPosition;
        padreInicial = transform.parent;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Permite detectar la zona detrás del objeto
        if (canvasGroup != null)
            canvasGroup.blocksRaycasts = false;

        // Lo pone temporalmente sobre todo para que se vea mientras arrastras
        transform.SetParent(canvas.transform);
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Movimiento correcto para UI Canvas
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (canvasGroup != null)
            canvasGroup.blocksRaycasts = true;
    }

    public void ResetPosition()
    {
        // Regresa al lugar original
        transform.SetParent(padreInicial);
        rectTransform.anchoredPosition = posicionInicial;
        transform.SetAsLastSibling();
    }
}