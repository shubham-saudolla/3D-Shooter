/*
Copyright (c) Shubham Saudolla
https://github.com/shubham-saudolla
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;																// namespace needed for editor scripts

[CustomEditor(typeof(MapGenerator))]
public class MapEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MapGenerator map = target as MapGenerator;

        if (DrawDefaultInspector())
        {
            map.GenerateMap();
        }

        if (GUILayout.Button("Generate Map"))
        {
            map.GenerateMap();
        }
    }
}
