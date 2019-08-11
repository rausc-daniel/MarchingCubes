using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WorldController))]
public class WorldControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var controller = (WorldController) target;

        controller.ChunkSize = EditorGUILayout.IntField("Chunk Size", controller.ChunkSize);
        controller.ChunkRes = EditorGUILayout.IntField("Chunk Resolution", controller.ChunkRes);
        
        controller.material =
            EditorGUILayout.ObjectField("Material", controller.material, typeof(Material), false) as Material;

        controller.Type = (WorldType) EditorGUILayout.EnumPopup("World Type", controller.Type);

        if (controller.Type == WorldType.Static)
        {
            controller.spread = EditorGUILayout.Vector3IntField(
                new GUIContent("Spread", "How many Chunks should be generated in each direction"), controller.spread);
        }
        else
        {
            controller.spread.x = EditorGUILayout.IntField(
                new GUIContent("Spread", "How many Chunks should be generated in each direction"), controller.spread.x);
        }

        var typeName = controller.Type == WorldType.Static ? "static" : "dynamic";
        
        if (GUILayout.Button($"Generate {typeName} World"))
        {
            controller.GenerateWorld();
        }
    }
}