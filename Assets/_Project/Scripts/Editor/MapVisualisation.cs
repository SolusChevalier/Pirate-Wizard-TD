using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapGenerator))]
public class MapVisualisation : Editor
{
    public override void OnInspectorGUI()
    {
        bool autoUpdateFlag = true;
        //base.OnInspectorGUI();
        MapGenerator map = target as MapGenerator;
        if (DrawDefaultInspector())
        {
            if (map.autoUpdate)
            {
                map.GenerateMap();
                autoUpdateFlag = true;
            }
            if (!map.autoUpdate && autoUpdateFlag)
            {
                map.ClearMap();
                autoUpdateFlag = false;
            }
        }
        //map.GenerateMap();
    }
}