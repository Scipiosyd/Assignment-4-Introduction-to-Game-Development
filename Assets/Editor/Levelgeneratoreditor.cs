#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelGeneratorInScene))]
public class LevelGeneratorInSceneEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        LevelGeneratorInScene generator = (LevelGeneratorInScene)target;
        if (GUILayout.Button("Generate Map In Scene"))
        {
            generator.GenerateLevelInScene();
        }
    }
}
#endif
