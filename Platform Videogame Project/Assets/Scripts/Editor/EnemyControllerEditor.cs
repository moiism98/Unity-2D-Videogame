using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EnemyController))]
public class EnemyControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();

        EnemyController enemyController = (EnemyController) target;

        EditorGUILayout.PropertyField(serializedObject.FindProperty("damagePoint"));

        EditorGUILayout.PropertyField(serializedObject.FindProperty("damageCollider"));

        switch(enemyController.GetDamageCollider())
        {
            case DamageCollider.circle: EditorGUILayout.PropertyField(serializedObject.FindProperty("damagePointRadius")); break;

            case DamageCollider.capsule: 

                EditorGUILayout.PropertyField(serializedObject.FindProperty("capsuleDirection")); 

                EditorGUILayout.PropertyField(serializedObject.FindProperty("capsuleSize")); 
                
            break;

            case DamageCollider.box: EditorGUILayout.PropertyField(serializedObject.FindProperty("boxSize")); break;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
