using UnityEngine;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IDropHandler
{
    public string idCorrecto;
    public string idActual;

    public void OnDrop(PointerEventData eventData)
    {
        DragItem item = eventData.pointerDrag.GetComponent<DragItem>();

        if (item != null)
        {
            item.transform.position = transform.position;
            idActual = item.gameObject.name;
        }
    }
}