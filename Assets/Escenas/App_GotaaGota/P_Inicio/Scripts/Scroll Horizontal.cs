using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SnapScrollHorizontal : MonoBehaviour, IEndDragHandler, IDragHandler
{
    [Header("Configuración de Referencias")]
    public ScrollRect scrollRect;
    public RectTransform contentPanel;
    public RectTransform[] tarjetas;

    [Header("Ajustes de Movimiento")]
    public float snapSpeed = 10f;

    private bool isSnapping;
    private Vector2 targetPosition;

    void Start()
    {
        // Si no asignaste las tarjetas manualmente, el script las busca automáticamente
        if (tarjetas.Length == 0)
        {
            tarjetas = new RectTransform[contentPanel.childCount];
            for (int i = 0; i < contentPanel.childCount; i++)
            {
                tarjetas[i] = contentPanel.GetChild(i) as RectTransform;
            }
        }
    }

    void Update()
    {
        // Si el usuario no está arrastrando y necesitamos ajustar la posición
        if (isSnapping)
        {
            contentPanel.anchoredPosition = Vector2.Lerp(contentPanel.anchoredPosition, targetPosition, snapSpeed * Time.deltaTime);

            // Detener el snapping cuando estemos muy cerca del objetivo
            if (Vector2.Distance(contentPanel.anchoredPosition, targetPosition) < 0.1f)
            {
                contentPanel.anchoredPosition = targetPosition;
                isSnapping = false;
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Al soltar el click/dedo, buscamos la tarjeta más cercana al centro
        float closestDistance = float.MaxValue;
        int closestIndex = 0;

        // La posición central del "Viewport" es 0 en el sistema local del contenido si está bien centrado
        for (int i = 0; i < tarjetas.Length; i++)
        {
            // Calculamos la distancia de la tarjeta respecto al centro del panel padre
            float distance = Mathf.Abs(tarjetas[i].position.x - transform.position.x);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestIndex = i;
            }
        }

        // Calculamos cuánto debemos mover el panel de contenido para centrar esa tarjeta
        float targetX = -tarjetas[closestIndex].anchoredPosition.x;
        targetPosition = new Vector2(targetX, contentPanel.anchoredPosition.y);
        isSnapping = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Mientras el usuario arrastra, detenemos el auto-ajuste (snapping)
        isSnapping = false;
    }
}