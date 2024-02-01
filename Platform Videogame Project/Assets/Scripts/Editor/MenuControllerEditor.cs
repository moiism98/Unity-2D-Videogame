using UnityEditor;

[CustomEditor(typeof(MenuController))]
public class MenuControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();

        MenuController menuController = (MenuController) target;

        EditorGUILayout.PropertyField(serializedObject.FindProperty("scene"));

        switch(menuController.scene)
        {
            case Scene.main: 

                EditorGUILayout.PropertyField(serializedObject.FindProperty("animatorControllers"));

                EditorGUILayout.PropertyField(serializedObject.FindProperty("animator"));

                EditorGUILayout.PropertyField(serializedObject.FindProperty("action"));

                EditorGUILayout.PropertyField(serializedObject.FindProperty("menu"));

                EditorGUILayout.PropertyField(serializedObject.FindProperty("controlsMenu"));
            
            break;

            case Scene.game: break;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
