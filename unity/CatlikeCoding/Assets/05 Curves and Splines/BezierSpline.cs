using System;

using UnityEngine;

public enum BezierControlPointMode {
    Free,
    Aligned,
    Mirrored
}

public class BezierSpline : MonoBehaviour {
    [SerializeField]
    private Vector3[] _points;

    [SerializeField]
    private BezierControlPointMode[] _modes;

    public int ControlPointCount {
        get { return _points.Length; }
    }

    public int CurveCount {
        get { return (_points.Length - 1) / 3; }
    }

    public void Reset() {
        _points = new Vector3[] {
            new Vector3(1f, 0f, 0f),
            new Vector3(2f, 0f, 0f),
            new Vector3(3f, 0f, 0f),
            new Vector3(4f, 0f, 0f)
        };

        _modes = new BezierControlPointMode[] {
            BezierControlPointMode.Free,
            BezierControlPointMode.Free
        };
    }

    public Vector3 GetControlPoint(int index) {
        return _points[index];
    }

    public void SetControlPoint(int index, Vector3 point) {
        _points[index] = point;
    }

    public BezierControlPointMode GetControlPointMode(int index) {
        return _modes[(index + 1) / 3];
    }

    public void SetControlPointMode(int index, BezierControlPointMode mode) {
        _modes[(index + 1) / 3] = mode;
    }

    public Vector3 GetPoint(float t) {
        int i;
        if(t >= 1f) {
            t = 1f;
            i = _points.Length - 4;
        } else {
            t = Mathf.Clamp01(t) * CurveCount;
            i = (int)t;
            t -= i;
            i *= 3;
        }

        return transform.TransformPoint(
            Bezier.GetPoint(_points[i], _points[i + 1], _points[i + 2], _points[i + 3], t));
    }

    public Vector3 GetVelocity(float t) {
        int i;
        if(t >= 1f) {
            t = 1f;
            i = _points.Length - 4;
        } else {
            t = Mathf.Clamp01(t) * CurveCount;
            i = (int)t;
            t -= i;
            i *= 3;
        }

        return transform.TransformPoint(
            Bezier.GetFirstDerivative(_points[i], _points[i + 1], _points[i + 2], _points[i + 3], t)) -
            transform.position;
    }

    public Vector3 GetDirection(float t) {
        return GetVelocity(t).normalized;
    }

    public void AddCurve() {
        Vector3 point = _points[_points.Length - 1];
        Array.Resize(ref _points, _points.Length + 3);
        point.x += 1f;
        _points[_points.Length - 3] = point;
        point.x += 1f;
        _points[_points.Length - 2] = point;
        point.x += 1f;
        _points[_points.Length - 1] = point;

        Array.Resize(ref _modes, _modes.Length + 1);
        _modes[_modes.Length - 1] = _modes[_modes.Length - 2];
    }
}
