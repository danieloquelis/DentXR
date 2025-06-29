using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using DrillSystem;

public class DrillingManager : MonoBehaviour
{
    public enum DrillLevel { IDLE, FIRST_LAYER, SECOND_LAYER, THIRD_LAYER }

    [Header("Dependencies")]
    [SerializeField] private CustomStylusHandler stylusHandler;

    [Header("Events")]
    public UnityEvent onDrillStarted;
    public UnityEvent onDrillStopped;

    [Header("Threshold")]
    public float hapticClickMinThreshold = 0.9f;

    [Header("Vibration Settings")]
    public float idleAmplitude = 0.2f;
    public float firstLayerAmplitude = 0.5f;
    public float secondLayerAmplitude = 0.8f;
    public float thirdLayerMaxAmplitude = 1f;
    public float pulseInterval = 0.5f;
    public float pulseDuration = 0.05f;

    private bool _isDrilling;
    private DrillLevel _currentLevel = DrillLevel.IDLE;
    private Coroutine _thirdLayerRoutine;

    private void Start()
    {
        DrillController.OnDrillingCollision += isDrillingTooth =>
        {
            SetDrillLevel(isDrillingTooth ? DrillLevel.IDLE : DrillLevel.FIRST_LAYER);
        };

        DrillController.OnNerveTouched += _ => SetDrillLevel(DrillLevel.THIRD_LAYER);
    }

    private void Update()
    {
        var pressure = stylusHandler.Stylus.cluster_middle_value;
        var isAboveThreshold = pressure >= hapticClickMinThreshold;

        switch (isAboveThreshold)
        {
            case true when !_isDrilling:
                _isDrilling = true;
                onDrillStarted?.Invoke();
                break;
            case false when _isDrilling:
                _isDrilling = false;
                onDrillStopped?.Invoke();
                SetDrillLevel(DrillLevel.IDLE);
                break;
        }

        if (_isDrilling && _currentLevel == DrillLevel.IDLE)
        {
            // Light vibration while idle during drilling
            TriggerHaptic(idleAmplitude);
        }
    }
    
    // Public method as we can modify this from any part of the app
    // As of not it is not being used, we will see a warning.
    public void SetDrillLevel(DrillLevel level)
    {
        if (_currentLevel == level) return;

        StopThirdLayerRoutine();

        _currentLevel = level;

        if (!_isDrilling)
        {
            // Don't do anything unless we're actively drilling
            _currentLevel = DrillLevel.IDLE;
            return;
        }

        switch (level)
        {
            case DrillLevel.FIRST_LAYER:
                TriggerHaptic(firstLayerAmplitude);
                break;
            case DrillLevel.SECOND_LAYER:
                TriggerHaptic(secondLayerAmplitude);
                break;
            case DrillLevel.THIRD_LAYER:
                _thirdLayerRoutine = StartCoroutine(ThirdLayerPulse());
                break;
        }
    }

    private void TriggerHaptic(float amplitude)
    {
        var hand = stylusHandler.Stylus.isOnRightHand ? OVRPlugin.Hand.HandRight : OVRPlugin.Hand.HandLeft;
        OVRPlugin.TriggerVibrationAction("haptic_pulse", hand, pulseDuration, amplitude);
    }

    private IEnumerator ThirdLayerPulse()
    {
        var hand = stylusHandler.Stylus.isOnRightHand ? OVRPlugin.Hand.HandRight : OVRPlugin.Hand.HandLeft;

        while (true)
        {
            OVRPlugin.TriggerVibrationAction("haptic_pulse", hand, pulseDuration, thirdLayerMaxAmplitude);
            yield return new WaitForSeconds(pulseInterval);
        }
    }

    private void StopThirdLayerRoutine()
    {
        if (_thirdLayerRoutine == null) return;
        StopCoroutine(_thirdLayerRoutine);
        _thirdLayerRoutine = null;
    }

    public DrillLevel GetCurrentLevel() => _currentLevel;
    public bool IsDrilling() => _isDrilling;
}
