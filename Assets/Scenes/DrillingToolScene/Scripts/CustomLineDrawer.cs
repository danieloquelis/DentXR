using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

public class CustomLineDrawer : MonoBehaviour
{
    private List<GameObject> _lines = new();
    private LineRenderer _currentLine;
    private List<float> _currentLineWidths = new();
    
    [SerializeField] private float maxLineWidth = 0.01f;
    [SerializeField] private float minLineWidth = 0.0005f;
    [SerializeField] private Material material;
    [SerializeField] private Color lineColor;
    [SerializeField] private Color highlightColor;
    [SerializeField] private CustomStylusHandler stylusHandler;
    [SerializeField] private MeshRenderer tipIndicator;
    [SerializeField] private float highlightThreshold = 0.01f;

    [Tooltip("Event triggered when fading between Passthrough and VR.")]
    [SerializeField] private UnityEvent onFadeToggled = new();

    private enum AppMode { Idle, Drawing, Highlighted, Grabbing }
    private const float MinDistanceBetweenLinePoints = 0.0005f;
    private AppMode _currentMode = AppMode.Idle;
    private Vector3 _previousLinePoint;
    private Color _cachedColor;

    private GameObject _highlightedLine;
    private Vector3 _grabStartPosition;
    private Quaternion _grabStartRotation;
    private Vector3[] _originalLinePositions;

    private void Awake()
    {
        //tipIndicator.material.color = lineColor;
        _cachedColor = lineColor;
        stylusHandler.OnFrontPressed += HandleFrontPressed;
        stylusHandler.OnFrontReleased += HandleFrontReleased;
        stylusHandler.OnBackPressed += HandleBackPressed;
        stylusHandler.OnBackReleased += HandleBackReleased;
        stylusHandler.OnDocked += HandleDocked;
        stylusHandler.OnUndocked += HandleUndocked;
    }

    private void OnDestroy()
    {
        stylusHandler.OnFrontPressed -= HandleFrontPressed;
        stylusHandler.OnFrontReleased -= HandleFrontReleased;
        stylusHandler.OnBackPressed -= HandleBackPressed;
        stylusHandler.OnBackReleased -= HandleBackReleased;
        stylusHandler.OnDocked -= HandleDocked;
        stylusHandler.OnUndocked -= HandleUndocked;
    }

    private void StartNewLine()
    {
        var lineObject = new GameObject(name: "Line");
        var lineRenderer = lineObject.AddComponent<LineRenderer>();
        _currentLine = lineRenderer;
        _currentLine.positionCount = 0;
        _currentLine.material = new Material(material) { color = lineColor };
        _currentLine.startWidth = minLineWidth;
        _currentLine.endWidth = minLineWidth;
        _currentLine.useWorldSpace = true;
        _currentLine.alignment = LineAlignment.View;
        _currentLine.widthCurve = new AnimationCurve();
        _currentLine.shadowCastingMode = ShadowCastingMode.Off;
        _currentLine.receiveShadows = false;
        _lines.Add(lineObject);
        _previousLinePoint = Vector3.zero;
        _currentLineWidths.Clear();
    }

    private void AddPointToLine(Vector3 position, float pressure)
    {
        if (!(Vector3.Distance(position, _previousLinePoint) > MinDistanceBetweenLinePoints)) return;

        _previousLinePoint = position;
        _currentLine.positionCount++;
        _currentLineWidths.Add(Math.Max(pressure * maxLineWidth, minLineWidth));
        _currentLine.SetPosition(_currentLine.positionCount - 1, position);

        var curve = new AnimationCurve();
        for (var i = 0; i < _currentLineWidths.Count; i++)
        {
            curve.AddKey(i / (float)(_currentLineWidths.Count - 1), _currentLineWidths[i]);
        }

        _currentLine.widthCurve = curve;
    }
 
    private void Update()
    {
        var stylus = stylusHandler.Stylus;
        var analogInput = Mathf.Max(stylus.tip_value, stylus.cluster_middle_value);

        if (analogInput > 0 && CanDraw())
        {
            if (_currentMode != AppMode.Drawing) StartNewLine();
            _currentMode = AppMode.Drawing;
            AddPointToLine(stylus.inkingPose.position, analogInput);
        }
        else if (_currentMode == AppMode.Drawing)
        {
            _currentMode = AppMode.Idle;
        }

        if (_currentMode != AppMode.Drawing && _currentMode != AppMode.Grabbing)
        {
            TryHighlightLine();
        }

        if (_currentMode == AppMode.Grabbing)
        {
            MoveHighlightedLine();
        }
    }
    
    private void TryHighlightLine()
    {
        var stylus = stylusHandler.Stylus;
        var closestLine = FindClosestLine(stylus.inkingPose.position);

        if (closestLine)
        {
            if (_highlightedLine != closestLine)
            {
                if (_highlightedLine)
                {
                    UnhighlightLine(_highlightedLine);
                }
                HighlightLine(closestLine);
                _currentMode = AppMode.Highlighted;
            }
        }
        else if (_highlightedLine)
        {
            UnhighlightLine(_highlightedLine);
            _currentMode = AppMode.Idle;
        }
    }

    private GameObject FindClosestLine(Vector3 position)
    {
        GameObject closestLine = null;
        var closestDistance = float.MaxValue;

        foreach (var line in _lines)
        {
            var lineRenderer = line.GetComponent<LineRenderer>();
            for (var i = 0; i < lineRenderer.positionCount - 1; i++)
            {
                var point = FindNearestPointOnLineSegment(
                    lineRenderer.GetPosition(i),
                    lineRenderer.GetPosition(i + 1),
                    position
                );

                var distance = Vector3.Distance(point, position);

                if (!(distance < closestDistance) || !(distance < highlightThreshold)) continue;

                closestDistance = distance;
                closestLine = line;
            }
        }

        return closestLine;
    }
    
    private void MoveHighlightedLine()
    {
        if (!_highlightedLine) return;

        var rotation = stylusHandler.Stylus.inkingPose.rotation * Quaternion.Inverse(_grabStartRotation);
        var lineRenderer = _highlightedLine.GetComponent<LineRenderer>();
        var newPositions = new Vector3[_originalLinePositions.Length];

        for (var i = 0; i < _originalLinePositions.Length; i++)
        {
            newPositions[i] = rotation * (_originalLinePositions[i] - _grabStartPosition) 
                              + stylusHandler.Stylus.inkingPose.position;
        }

        lineRenderer.SetPositions(newPositions);
    }
    
    private Vector3 FindNearestPointOnLineSegment(Vector3 segStart, Vector3 segEnd, Vector3 point)
    {
        var segVec = segEnd - segStart;
        var segLen = segVec.magnitude;
        var segDir = segVec.normalized;

        var pointVec = point - segStart;
        var projLen = Vector3.Dot(pointVec, segDir);
        var clampedLen = Mathf.Clamp(projLen, 0f, segLen);

        return segStart + segDir * clampedLen;
    }

    private void HighlightLine(GameObject line)
    {
        _highlightedLine = line;
        var lineRenderer = line.GetComponent<LineRenderer>();
        _cachedColor = lineRenderer.material.color;
        lineRenderer.material.color = highlightColor;
    }

    private void UnhighlightLine(GameObject line)
    {
        var lineRenderer = line.GetComponent<LineRenderer>();
        lineRenderer.material.color = _cachedColor;
        _highlightedLine = null;
    }

    private void DeleteHighlightedLine()
    {
        if (!_highlightedLine) return;
        _lines.Remove(_highlightedLine);
        Destroy(_highlightedLine);
        _highlightedLine = null;
        _currentMode = AppMode.Idle;
    }
    
    private bool CanDraw()
    {
        return stylusHandler.Stylus is { isActive: true, docked: false };
    }

    private void HandleFrontPressed()
    {
        if (_currentMode == AppMode.Highlighted)
        {
            //StartGrabbingLine();
        }
        else
        {
            //UpdateLineColor();
        }
    }

    private void HandleFrontReleased()
    {
        if (_currentMode == AppMode.Grabbing)
        {
            //StopGrabbingLine();
        }
    }

    private void HandleBackPressed()
    {
        if (_currentMode == AppMode.Highlighted)
        {
            DeleteHighlightedLine();
        }
        else
        {
            onFadeToggled?.Invoke();
        }
    }

    private void HandleBackReleased()
    {
        // Placeholder
    }

    private void HandleDocked()
    {
        _currentMode = AppMode.Idle;
        // Placeholder
    }

    private void HandleUndocked()
    {
        // Placeholder
    }
}
