using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GroundMover))]
public class GroundMoverEditor : Editor
{
    private Vector3[] _positions = new Vector3[2];
    
    private void OnSceneGUI()
    {
        var groundMover = (GroundMover)target;
        
        EditorGUI.BeginChangeCheck();
        
        var positionsLength = groundMover.targetPositions.Length;
        if (positionsLength != _positions.Length)
        {
            _positions = new Vector3[positionsLength];
        }

        for (var i = 0; i < positionsLength; i++)
        {
            _positions[i] = Handles.PositionHandle(groundMover.targetPositions[i], quaternion.identity);
            Handles.Label(groundMover.targetPositions[i] + Vector3.down * 1f, $"Position {i}", "Box");
        }
        
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(groundMover, "Change Positions Of " + groundMover.gameObject.name);
            for (var i = 0; i < positionsLength; i++)
            {
                groundMover.targetPositions[i] = _positions[i];
            }
        }
    }
}
