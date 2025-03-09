using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(Item))]
public class ItemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Item item = (Item)target;

        if(item.Type == ItemType.Weapon)
            EditorGUILayout.PropertyField(serializedObject.FindProperty("WeaponPrefab"));
        else if(item.Type == ItemType.Medkit)
            EditorGUILayout.PropertyField(serializedObject.FindProperty("HealAmount"));

        if(serializedObject.hasModifiedProperties)
            serializedObject.ApplyModifiedProperties();
    }
}
#endif
