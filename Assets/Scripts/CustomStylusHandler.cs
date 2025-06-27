using System;
using UnityEngine;

public class CustomStylusHandler : MonoBehaviour
{
    [SerializeField] private float hapticClickDuration = 0.05f;
    [SerializeField] private float hapticClickAmplitude = 0.9f;
    [SerializeField] private float hapticClickMinThreshold = 0.9f;

    private StylusInputs _stylus;
    public StylusInputs Stylus => _stylus;

    public event Action OnFrontPressed;
    public event Action OnFrontReleased;
    public event Action OnBackPressed;
    public event Action OnBackReleased;
    public event Action OnDocked;
    public event Action OnUndocked;

    private bool _previousFrontValue;
    private bool _previousBackValue;
    private bool _previousDockedValue;

    private const string InkPoseRight = "aim_right";
    private const string InkPoseLeft = "aim_left";
    private const string InkHapticPulse = "haptic_pulse";

    private void Update()
    {
        OVRInput.Update();
        UpdateStylusPose();
        UpdateStylusInputs();
        CheckBooleanEvents();
        GenerateHapticFeedback();
    }

    private void UpdateStylusPose()
    {
        var leftDevice = OVRPlugin.GetCurrentInteractionProfileName(OVRPlugin.Hand.HandLeft);
        var rightDevice = OVRPlugin.GetCurrentInteractionProfileName(OVRPlugin.Hand.HandRight);
        
        _stylus.isActive = leftDevice.Contains("logitech") || rightDevice.Contains("logitech");
        _stylus.isOnRightHand = rightDevice.Contains("logitech");
        
        var poseAction = _stylus.isOnRightHand ? InkPoseRight : InkPoseLeft;

        if (!OVRPlugin.GetActionStatePose(poseAction, out var handPose)) return;
        transform.localPosition = handPose.Position.FromFlippedZVector3f();
        transform.localRotation = handPose.Orientation.FromFlippedZQuatf();
        _stylus.inkingPose.position = transform.localPosition;
        _stylus.inkingPose.rotation = transform.localRotation;
    }

    private void UpdateStylusInputs()
    {
        _stylus.tip_value = GetActionStateFloat("tip");
        _stylus.cluster_middle_value = GetActionStateFloat("middle");
        _stylus.cluster_front_value = GetActionStateBoolean("front");
        _stylus.cluster_back_value = GetActionStateBoolean("back");
        _stylus.docked = GetActionStateBoolean("dock");
    }

    private float GetActionStateFloat(string actionName)
    {
        return OVRPlugin.GetActionStateFloat(actionName, out var value) ? value : 0f;
    }

    private bool GetActionStateBoolean(string actionName)
    {
        return OVRPlugin.GetActionStateBoolean(actionName, out var value) && value;
    }

    private void CheckBooleanEvents()
    {
        switch (_stylus.cluster_front_value)
        {
            case true when !_previousFrontValue:
                OnFrontPressed?.Invoke();
                break;
            case false when _previousFrontValue:
                OnFrontReleased?.Invoke();
                break;
        }
        _previousFrontValue = _stylus.cluster_front_value;

        switch (_stylus.cluster_back_value)
        {
            case true when !_previousBackValue:
                OnBackPressed?.Invoke();
                break;
            case false when _previousBackValue:
                OnBackReleased?.Invoke();
                break;
        }
        _previousBackValue = _stylus.cluster_back_value;

        switch (_stylus.docked)
        {
            case true when !_previousDockedValue:
                OnDocked?.Invoke();
                break;
            case false when _previousDockedValue:
                OnUndocked?.Invoke();
                break;
        }

        _previousDockedValue = _stylus.docked;
    }

    private void GenerateHapticFeedback()
    {
        var holdingHand = _stylus.isOnRightHand ? OVRPlugin.Hand.HandRight : OVRPlugin.Hand.HandLeft;
        GenerateHapticClick(_stylus.tip_value, holdingHand);
        GenerateHapticClick(_stylus.cluster_middle_value, holdingHand);
    }

    private void GenerateHapticClick(float analogValue, OVRPlugin.Hand hand)
    {
        if (analogValue >= hapticClickMinThreshold)
        {
            TriggerHapticFeedback(hand);
        }
    }

    private void TriggerHapticFeedback(OVRPlugin.Hand hand)
    {
        OVRPlugin.TriggerVibrationAction(InkHapticPulse, hand, hapticClickDuration, hapticClickAmplitude);
    }

}
