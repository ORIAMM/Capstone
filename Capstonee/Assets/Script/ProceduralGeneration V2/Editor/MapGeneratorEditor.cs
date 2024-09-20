using UnityEditor;
using UnityEngine;

namespace ProceduralGeneration
{
    [CustomEditor(typeof(MapGenerator2))]
    public class MapGeneratorEditor2 : Editor
    {
        public override void OnInspectorGUI()
        {
            MapGenerator2 mapGen = (MapGenerator2)target;

            if (DrawDefaultInspector())
            {
                if (mapGen.AutoUpdate) mapGen.DrawMapInEditor();
            }
            if (GUILayout.Button("Generate Map"))
            {
                mapGen.DrawMapInEditor();
            }
        }
    }
}