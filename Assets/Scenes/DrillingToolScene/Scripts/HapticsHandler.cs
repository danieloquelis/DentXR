using System.Collections;
using UnityEngine;

public class HapticsHandler : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private CustomStylusHandler stylusHandler;
    [SerializeField] private DrillingManager drillingManager;
    
    [Header("Audio")]
    [SerializeField] private AudioSource audioSourceA;
    [SerializeField] private AudioSource audioSourceB;
    [SerializeField] private AudioClip rampUpClip;
    [SerializeField] private AudioClip loopClip;
    [SerializeField] private AudioClip rampDownClip;
    [SerializeField] private float loopOverlap = 0.05f;

    private bool _isLooping;
    private bool _wasAboveThreshold;
    private bool _useA = true;
    private float _nextLoopTime;

    private void Update()
    {
        var pressure = stylusHandler.Stylus.cluster_middle_value;

        if (pressure >= drillingManager.hapticClickMinThreshold && !_wasAboveThreshold)
        {
            PlayOnce(rampUpClip);
            Invoke(nameof(StartLoop), rampUpClip.length - loopOverlap);
        }
        else if (pressure < drillingManager.hapticClickMinThreshold && _wasAboveThreshold)
        {
            StopLoop();
            PlayOnce(rampDownClip);
        }

        if (_isLooping && Time.time >= _nextLoopTime)
        {
            CrossfadeLoop();
        }

        _wasAboveThreshold = pressure >= drillingManager.hapticClickMinThreshold;;
    }

    private void PlayOnce(AudioClip clip)
    {
        if (!clip) return;

        var activeSource = _useA ? audioSourceA : audioSourceB;
        activeSource.Stop();
        activeSource.clip = clip;
        activeSource.loop = false;
        activeSource.volume = 1f;
        activeSource.Play();
    }

    private void StartLoop()
    {
        if (!_wasAboveThreshold) return; // prevent late start

        _isLooping = true;
        _nextLoopTime = Time.time + loopClip.length - loopOverlap;
        CrossfadeLoop();
    }

    private void CrossfadeLoop()
    {
        var current = _useA ? audioSourceA : audioSourceB;
        var next = _useA ? audioSourceB : audioSourceA;

        next.clip = loopClip;
        next.loop = false;
        next.volume = 0f;
        next.Play();
        _nextLoopTime = Time.time + loopClip.length - loopOverlap;

        StartCoroutine(FadeIn(next, 0.1f));

        _useA = !_useA;
    }

    private void StopLoop()
    {
        _isLooping = false;
        CancelInvoke(nameof(StartLoop)); // Prevent delayed loop if user released quickly
        audioSourceA.Stop();
        audioSourceB.Stop();
    }

    private static IEnumerator FadeIn(AudioSource source, float duration)
    {
        var elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            source.volume = Mathf.Clamp01(elapsed / duration);
            yield return null;
        }
        source.volume = 1f;
    }
}
