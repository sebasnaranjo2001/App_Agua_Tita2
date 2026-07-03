using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

// Asegura que el objeto tenga una imagen para poder cambiarle el color
[RequireComponent(typeof(Image))]
public class DropZone : MonoBehaviour, IDropHandler
{
    public DragItem objetoCorrecto; // El objeto (A, B o C) que debe ir aquí
    public DragItem objetoActual;   // El objeto que está actualmente soltado aquí

    private Image imagen;

    void Awake()
    {
        imagen = GetComponent<Image>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        // 1. Verificamos que lo que estamos arrastrando sea válido
        if (eventData.pointerDrag == null) return;

        DragItem item = eventData.pointerDrag.GetComponent<DragItem>();

        // Si el objeto ya estaba colocado en otra zona, la liberamos.
        if (item.zonaActual != null && item.zonaActual != this)
        {
            item.zonaActual.objetoActual = null;
        }

        if (item != null)
        {
            // 2. Si ya había un objeto aquí, lo mandamos a su posición original
            if (objetoActual != null && objetoActual != item)
            {
                objetoActual.ResetPosition();
            }

            

            // 4. RESET DE TRANSFORM (Aquí es donde solía fallar)
            // Esto asegura que se centre perfecto y no herede escalas raras
            item.transform.SetParent(transform, false);

            RectTransform rect = item.GetComponent<RectTransform>();

            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);

            rect.anchoredPosition = Vector2.zero;
            rect.localScale = Vector3.one;

            objetoActual = item;
            item.zonaActual = this;
            Debug.Log($"<color=green>OBJETO SOLTADO:</color> {item.name} entró en {gameObject.name}");
        }
    }

    public bool EsCorrecto()
    {
        return objetoActual == objetoCorrecto;
    }

    public void MarcarCorrecto()
    {
        if (imagen != null)
        {
            imagen.color = new Color32(76, 175, 80, 255);

            LeanTween.scale(
                gameObject,
                Vector3.one * 1.1f,
                0.15f
            ).setLoopPingPong(1);
        }
    }


    public void MarcarIncorrecto()
    {
        if (imagen != null)
        {
            imagen.color = new Color32(244, 67, 54, 255);

            LeanTween.moveLocalX(
                gameObject,
                transform.localPosition.x + 15f,
                0.05f
            ).setLoopPingPong(3);
        }
    }

    public void ResetZona()
    {
       

        objetoActual = null;

        if (imagen != null)
        {
            imagen.color = Color.white;
        }
    }
}