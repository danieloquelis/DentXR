using System;
using UnityEngine;

public class CustomStylusHandler : MonoBehaviour
{
    [SerializeField] private Transform offsetTip;

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
    }

    private void UpdateStylusPose()
    {
        var leftDevice = OVRPlugin.GetCurrentInteractionProfileName(OVRPlugin.Hand.HandLeft);
        var rightDevice = OVRPlugin.GetCurrentInteractionProfileName(OVRPlugin.Hand.HandRight);
    
        _stylus.isActive = leftDevice.Contains("logitech") || rightDevice.Contains("logitech");
        _stylus.isOnRightHand = rightDevice.Contains("logitech");
    
        var poseAction = _stylus.isOnRightHand ? InkPoseRight : InkPoseLeft;

        if (!OVRPlugin.GetActionStatePose(poseAction, out var handPose)) return;

        var rawPosition = handPose.Position.FromFlippedZVector3f();
        var rawRotation = handPose.Orientation.FromFlippedZQuatf();

        if (offsetTip)
        {
            transform.position = offsetTip.TransformPoint(rawPosition);
            transform.rotation = offsetTip.rotation * rawRotation;
        }
        else
        {
            transform.localPosition = rawPosition;
            transform.localRotation = rawRotation;
        }

        _stylus.inkingPose.position = transform.position;
        _stylus.inkingPose.rotation = transform.rotation;
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

    // private void GenerateHapticFeedback()
    // {
    //     var holdingHand = _stylus.isOnRightHand ? OVRPlugin.Hand.HandRight : OVRPlugin.Hand.HandLeft;
    //     GenerateHapticClick(_stylus.tip_value, holdingHand);
    //     GenerateHapticClick(_stylus.cluster_middle_value, holdingHand);
    // }

    // private void GenerateHapticClick(float analogValue, OVRPlugin.Hand hand)
    // {
    //     if (analogValue >= hapticClickMinThreshold)
    //     {
    //         TriggerHapticFeedback(hand);
    //     }
    // }

    // private void TriggerHapticFeedback(OVRPlugin.Hand hand)
    // {
    //     OVRPlugin.TriggerVibrationAction(InkHapticPulse, hand, hapticClickDuration, hapticClickAmplitude);
    // }

}
