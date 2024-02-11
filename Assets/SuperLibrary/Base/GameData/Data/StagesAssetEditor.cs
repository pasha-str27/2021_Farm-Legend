#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

/// <summary>
/// This is sample, DO NOT MODIFY IT -> Duplicate then change namespace new game ex: Yogame.DancingBall
/// </summary>
[CustomEditor(typeof(StagesAsset))]
public class StagesAssetEditor : Editor
{
    public override void OnInspectorGUI()
    {
        StagesAsset myScript = (StagesAsset)target;

        if (GUILayout.Button("Reset Data"))
        {
            myScript.ResetData();
            AssetDatabase.SaveAssets();
        }

        if (GUILayout.Button("Add All Stages"))
        {
            myScript.AddAllStages();
            EditorUtility.SetDirty(myScript);
        }

        DrawDefaultInspector();
    }
}
#endif
