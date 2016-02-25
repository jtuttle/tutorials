using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BezierSpline))]
public class BezierSplineInspector : Editor {
    private const int STEPS_PER_CURVE = 10;
    private const float DIRECTION_SCALE = 0.5f;

    private BezierSpline _spline;
    private Transform _handleTransform;
    private Quaternion _handleRotation;

    private void OnSceneGUI() {
        _spline = target as BezierSpline;
        _handleTransform = _spline.transform;
        _handleRotation = Tools.pivotRotation == PivotRotation.Local ?
            _handleTransform.rotation : Quaternion.identity;

        Vector3 p0 = ShowPoint(0);
        for(int i = 1; i < _spline.points.Length; i += 3) {
            Vector3 p1 = ShowPoint(i);
            Vector3 p2 = ShowPoint(i + 1);
            Vector3 p3 = ShowPoint(i + 2);

            Handles.color = Color.gray;
            Handles.DrawLine(p0, p1);
            Handles.DrawLine(p2, p3);

            Handles.DrawBezier(p0, p3, p1, p2, Color.white, null, 2f);
            p0 = p3;
        }

        ShowDirections();
    }

    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        _spline = target as BezierSpline;
        if(GUILayout.Button("Add Curve")) {
            Undo.RecordObject(_spline, "Add Curve");
            _spline.AddCurve();
            EditorUtility.SetDirty(_spline);
        }
    }

    private Vector3 ShowPoint(int index) {
        Vector3 point = _handleTransform.TransformPoint(_spline.points[index]);

        EditorGUI.BeginChangeCheck();
        point = Handles.DoPositionHandle(point, _handleRotation);
        if(EditorGUI.EndChangeCheck()) {
            Undo.RecordObject(_spline, "Move Point");
            EditorUtility.SetDirty(_spline);
            _spline.points[index] = _handleTransform.InverseTransformPoint(point);
        }
        return point;
    }

    private void ShowDirections() {
        Handles.color = Color.green;
        Vector3 point = _spline.GetPoint(0);
        Handles.DrawLine(point, point + _spline.GetDirection(0f) * DIRECTION_SCALE);
        int steps = STEPS_PER_CURVE * _spline.CurveCount;
        for(int i = 1; i <= steps; i++) {
            point = _spline.GetPoint(i / (float)steps);
            Handles.DrawLine(point, point + _spline.GetDirection(i / (float)steps) * DIRECTION_SCALE);
        }
    }
}
