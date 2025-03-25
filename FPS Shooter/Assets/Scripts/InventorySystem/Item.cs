using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New Item", menuName = "Scriptable object/Item")]
public class Item : ScriptableObject
{
    public Sprite Sprite;
    public bool IsStackable;
    public ItemType Type;
    public WeaponController WeaponPrefab;
    public int HealAmount;
    public int MaxCount;
}

public enum ItemType
{
    Weapon,
    Ammo,
    Medkit
}
