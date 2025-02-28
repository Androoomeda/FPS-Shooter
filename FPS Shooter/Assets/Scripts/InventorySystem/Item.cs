using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Item", menuName = "Scriptable object/Item")]
public class Item : ScriptableObject
{
    public TileBase Tile;
    public ItemType Type;
    public Sprite Sprite;
}

public enum ItemType
{
    Weapon,
    Stuff
}
