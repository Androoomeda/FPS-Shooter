using UnityEditor;

[CustomEditor(typeof(Item))]
public class ItemEditor : Editor
{
    #region SerializedProperties
    SerializedProperty Sprite;
    SerializedProperty IsStackable;
    SerializedProperty Type;
    SerializedProperty WeaponPrefab;
    SerializedProperty HealAmount;
    SerializedProperty AmmoCount;
    #endregion

    void OnEnable()
    {
        Sprite = serializedObject.FindProperty("Sprite");
        IsStackable = serializedObject.FindProperty("IsStackable");
        Type = serializedObject.FindProperty("Type");
        WeaponPrefab = serializedObject.FindProperty("WeaponPrefab");
        HealAmount = serializedObject.FindProperty("HealAmount");
        AmmoCount = serializedObject.FindProperty("AmmoCount");
    }

    public override void OnInspectorGUI()
    {
        Item item = (Item)target;

        EditorGUILayout.PropertyField(Sprite);
        EditorGUILayout.PropertyField(IsStackable);
        EditorGUILayout.PropertyField(Type);

        if(item.Type == ItemType.Weapon)
            EditorGUILayout.PropertyField(WeaponPrefab);
        else if(item.Type == ItemType.Medkit)
            EditorGUILayout.PropertyField(HealAmount);
        else if(item.Type == ItemType.Ammo)
            EditorGUILayout.PropertyField(AmmoCount);

        if(serializedObject.hasModifiedProperties)
            serializedObject.ApplyModifiedProperties();
    }
}
