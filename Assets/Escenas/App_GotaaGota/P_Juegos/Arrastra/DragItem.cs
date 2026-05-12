using UnityEngine;
using UnityEngine.EventSystems;

// Obligamos a que tenga CanvasGroup para que el mouse pueda "atravesar" el objeto al soltarlo
[RequireComponent(typeof(CanvasGroup))]
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

        // 1. Buscamos el Canvas en los padres
        canvas = GetComponentInParent<Canvas>();

        // 2. Si no lo encuentra, lo busca en toda la escena
        if (canvas == null)
        {
            canvas = GameObject.FindAnyObjectByType<Canvas>();
        }
    }

    void Start()
    {
        // --- ESCUDO ANTI-FUGA ---
        // Si el objeto aparece fuera del Canvas al iniciar, lo metemos a la fuerza
        if (transform.parent == null || transform.parent.GetComponentInParent<Canvas>() == null)
        {
            if (canvas != null)
            {
                transform.SetParent(canvas.transform);
                Debug.Log($"<color=yellow>¡Aviso!</color> El objeto {gameObject.name} estaba fuera del Canvas y fue rescatado.");
            }
        }

        // Guardamos la posición y el padre real para poder regresar si fallamos el tiro
        posicionInicial = rectTransform.anchoredPosition;
        padreInicial = transform.parent;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (canvas == null) return;

        // Desactivamos blocksRaycasts para que la DropZone detecte el soltado
        if (canvasGroup != null)
            canvasGroup.blocksRaycasts = false;

        // Ponemos el objeto encima de todo en el Canvas mientras se arrastra
        transform.SetParent(canvas.transform);
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (canvas == null) return;

        // Movimiento ajustado a la escala del Canvas (importante para diferentes pantallas)
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Reactivamos la detección de raycasts
        if (canvasGroup != null)
            canvasGroup.blocksRaycasts = true;

        // Si el objeto no fue aceptado por una DropZone (sigue siendo hijo del Canvas), regresa
        if (transform.parent == canvas.transform)
        {
            ResetPosition();
        }
    }

    public void ResetPosition()
    {
        // Regresa a su panel y posición original
        transform.SetParent(padreInicial);
        rectTransform.anchoredPosition = posicionInicial;
    }
}