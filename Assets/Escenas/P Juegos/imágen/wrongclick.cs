using UnityEngine;
using UnityEngine.EventSystems;

public class WrongClick : MonoBehaviour, IPointerClickHandler
{
    public SpotWaterManager manager;

    public void OnPointerClick(PointerEventData eventData)
    {
        manager.WrongClick();
    }
}