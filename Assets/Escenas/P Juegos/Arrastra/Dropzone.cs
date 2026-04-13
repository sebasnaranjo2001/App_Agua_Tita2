using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropZone : MonoBehaviour, IDropHandler
{
    public string idCorrecto;
    public string idActual;

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

            idActual = item.gameObject.name.Replace("(Clone)", "").Trim().ToUpper();
        }
    }

    public void MarcarCorrecto()
    {
        imagen.color = new Color(0.6f, 1f, 0.6f);
    }

    public void MarcarIncorrecto()
    {
        imagen.color = new Color(1f, 0.6f, 0.6f);
    }

    public void ResetColor()
    {
        imagen.color = Color.white;
    }

    public void ResetZona()
    {
        idActual = "";
        ResetColor();
    }
}