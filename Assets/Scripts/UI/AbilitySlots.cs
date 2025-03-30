using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class AbilitySlots : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        if (transform.childCount == 0 && transform.gameObject.tag == "Active ability in shop") {
            GameObject dropped = eventData.pointerDrag;
            DraggableItem draggableItem = dropped.GetComponent<DraggableItem>();
            draggableItem.parentAfterDrag = transform;
        }
    }
}
