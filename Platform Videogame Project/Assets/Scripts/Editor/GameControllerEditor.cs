using UnityEditor;

[CustomEditor(typeof(GameController))]
public class GameControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();
        
        GameController gameController = (GameController) target;

        EditorGUILayout.PropertyField(serializedObject.FindProperty("gameMode"));

        switch(gameController.gameMode)
        {
            case GameMode.regular: 

                EditorGUILayout.PropertyField(serializedObject.FindProperty("currentStageText"));

                EditorGUILayout.PropertyField(serializedObject.FindProperty("lastStageText"));

                EditorGUILayout.PropertyField(serializedObject.FindProperty("keysText"));
            
            break;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
