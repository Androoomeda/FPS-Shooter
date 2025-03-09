using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    [HideInInspector] public Item Item;
    [HideInInspector] public Transform parent;

    private Image image;
    private GameObject player;

    private void OnEnable()
    {
        image = GetComponent<Image>();
    }

    public void InitializeItem(Item newItem, GameObject player)
    {
        Item = newItem;
        image.sprite = newItem.Sprite;
        this.player = player;
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

    public void OnPointerClick(PointerEventData eventData)
    {
        switch(Item.Type)
        {
            case ItemType.Weapon:
                player.GetComponent<PlayerWeaponsManager>().SwitchWeapon(Item.WeaponPrefab);
                break;
            case ItemType.Medkit:
                player.GetComponent<Health>().Heal(Item.HealAmount);
                break;
        }

        Destroy(gameObject);
    }

}
