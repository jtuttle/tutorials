﻿using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BezierCurve))]
public class BezierCurveInspector : Editor {
    private const int LINE_STEPS = 10;
    private const float DIRECTION_SCALE = 0.5f;

    private BezierCurve _curve;
    private Transform _handleTransform;
    private Quaternion _handleRotation;

    private void OnSceneGUI() {
        _curve = target as BezierCurve;
        _handleTransform = _curve.transform;
        _handleRotation = Tools.pivotRotation == PivotRotation.Local ?
            _handleTransform.rotation : Quaternion.identity;

        Vector3 p0 = ShowPoint(0);
        Vector3 p1 = ShowPoint(1);
        Vector3 p2 = ShowPoint(2);
        Vector3 p3 = ShowPoint(3);

        Handles.color = Color.gray;
        Handles.DrawLine(p0, p1);
        Handles.DrawLine(p2, p3);

        ShowDirections();
        Handles.DrawBezier(p0, p3, p1, p2, Color.white, null, 2f);
    }

    private Vector3 ShowPoint(int index) {
        Vector3 point = _handleTransform.TransformPoint(_curve.points[index]);

        EditorGUI.BeginChangeCheck();
        point = Handles.DoPositionHandle(point, _handleRotation);
        if(EditorGUI.EndChangeCheck()) {
            Undo.RecordObject(_curve, "Move Point");
            EditorUtility.SetDirty(_curve);
            _curve.points[index] = _handleTransform.InverseTransformPoint(point);
        }
        return point;
    }

    private void ShowDirections() {
        Handles.color = Color.green;
        Vector3 point = _curve.GetPoint(0);
        Handles.DrawLine(point, point + _curve.GetDirection(0f) * DIRECTION_SCALE);
        for(int i = 1; i < LINE_STEPS; i++) {
            point = _curve.GetPoint(i / (float)LINE_STEPS);
            Handles.DrawLine(point, point + _curve.GetDirection(i / (float)LINE_STEPS) * DIRECTION_SCALE);
        }
    }
}
