using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropZone : MonoBehaviour, IDropHandler
{
    public DragItem objetoCorrecto;   // El objeto que debe ir aquí
    public DragItem objetoActual;     // El objeto soltado

    private Image imagen;

    void Start()
    {
        imagen = GetComponent<Image>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        DragItem item = eventData.pointerDrag.GetComponent<DragItem>();

        if (item != null)
        {
            item.transform.SetParent(transform);
            item.transform.localPosition = Vector3.zero;

            objetoActual = item;
        }
    }

    public bool EsCorrecto()
    {
        return objetoActual == objetoCorrecto;
    }

    public void MarcarCorrecto()
    {
        imagen.color = new Color(0.6f, 1f, 0.6f);
    }

    public void MarcarIncorrecto()
    {
        imagen.color = new Color(1f, 0.6f, 0.6f);
    }

    public void ResetZona()
    {
        objetoActual = null;
        imagen.color = Color.white;
    }
}