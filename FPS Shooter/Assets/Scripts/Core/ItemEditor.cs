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
    SerializedProperty MaxCount;
    #endregion

    void OnEnable()
    {
        Sprite = serializedObject.FindProperty("Sprite");
        IsStackable = serializedObject.FindProperty("IsStackable");
        Type = serializedObject.FindProperty("Type");
        WeaponPrefab = serializedObject.FindProperty("WeaponPrefab");
        HealAmount = serializedObject.FindProperty("HealAmount");
        MaxCount = serializedObject.FindProperty("MaxCount");
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

        if(item.IsStackable)
            EditorGUILayout.PropertyField(MaxCount);

        if(serializedObject.hasModifiedProperties)
            serializedObject.ApplyModifiedProperties();
    }
}
