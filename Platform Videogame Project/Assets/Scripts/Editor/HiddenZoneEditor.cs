using UnityEditor;
[CustomEditor(typeof(HiddenZone))]
public class HiddenZoneEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();

        HiddenZone hiddenZone = (HiddenZone) target;

        EditorGUILayout.PropertyField(serializedObject.FindProperty("hiddenZoneType"));

        switch(hiddenZone.hiddenZoneType)
        {
            case HiddenZoneType.tilemap: EditorGUILayout.PropertyField(serializedObject.FindProperty("tilemap")); break;
            case HiddenZoneType.gameObject: EditorGUILayout.PropertyField(serializedObject.FindProperty("spriteRenderer")); break;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
