using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [HideInInspector] public Item Item;
    [HideInInspector] public Transform parent;

    private Image image;

    private void Start()
    {
        image = GetComponent<Image>();
        InitializeItem(Item);
    }

    public void InitializeItem(Item newItem)
    {
        Item = newItem;
        image.sprite = newItem.Sprite;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        image.raycastTarget = false;
        parent = transform.parent;
        transform.SetParent(transform.root);
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        image.raycastTarget = true;
        transform.SetParent(parent);
    }
}
