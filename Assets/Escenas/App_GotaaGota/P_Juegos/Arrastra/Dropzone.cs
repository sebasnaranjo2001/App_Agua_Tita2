using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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

        if (item != null)
        {
            // 2. Si ya había un objeto aquí, lo mandamos a su posición original
            if (objetoActual != null && objetoActual != item)
            {
                objetoActual.ResetPosition();
            }

            // 3. Emparentamos el objeto a la zona
            item.transform.SetParent(transform);

            // 4. RESET DE TRANSFORM (Aquí es donde solía fallar)
            // Esto asegura que se centre perfecto y no herede escalas raras
            item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            item.transform.localScale = Vector3.one;

            objetoActual = item;
            Debug.Log($"<color=green>OBJETO SOLTADO:</color> {item.name} entró en {gameObject.name}");
        }
    }

    public bool EsCorrecto()
    {
        return objetoActual == objetoCorrecto;
    }

    public void MarcarCorrecto()
    {
        if (imagen != null) imagen.color = new Color(0.6f, 1f, 0.6f); // Verde suave
    }

    public void MarcarIncorrecto()
    {
        if (imagen != null) imagen.color = new Color(1f, 0.6f, 0.6f); // Rojo suave
    }

    public void ResetZona()
    {
        objetoActual = null;
        if (imagen != null) imagen.color = Color.white;
    }
}