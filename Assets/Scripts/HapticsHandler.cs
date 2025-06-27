using UnityEngine;

public class HapticsHandler : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private CustomStylusHandler stylusHandler;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSourceA;
    [SerializeField] private AudioSource audioSourceB;
    [SerializeField] private AudioClip rampUpClip;
    [SerializeField] private AudioClip loopClip;
    [SerializeField] private AudioClip rampDownClip;
    [SerializeField] private float loopOverlap = 0.05f;

    [Header("Drill Pressure Threshold")]
    [SerializeField] private float pressureThreshold = 0.5f;

    private bool isLooping;
    private bool wasAboveThreshold;
    private bool useA = true;
    private float nextLoopTime;

    private void Update()
    {
        var pressure = stylusHandler.Stylus.cluster_middle_value;

        if (pressure >= pressureThreshold && !wasAboveThreshold)
        {
            PlayOnce(rampUpClip);
            Invoke(nameof(StartLoop), rampUpClip.length - loopOverlap);
        }
        else if (pressure < pressureThreshold && wasAboveThreshold)
        {
            StopLoop();
            PlayOnce(rampDownClip);
        }

        if (isLooping && Time.time >= nextLoopTime)
        {
            CrossfadeLoop();
        }

        wasAboveThreshold = pressure >= pressureThreshold;;
    }

    private void PlayOnce(AudioClip clip)
    {
        if (clip == null) return;

        var activeSource = useA ? audioSourceA : audioSourceB;
        activeSource.Stop();
        activeSource.clip = clip;
        activeSource.loop = false;
        activeSource.volume = 1f;
        activeSource.Play();
    }

    private void StartLoop()
    {
        isLooping = true;
        nextLoopTime = Time.time + loopClip.length - loopOverlap;
        CrossfadeLoop();
    }

    private void CrossfadeLoop()
    {
        var current = useA ? audioSourceA : audioSourceB;
        var next = useA ? audioSourceB : audioSourceA;

        next.clip = loopClip;
        next.loop = false;
        next.volume = 0f;
        next.Play();
        nextLoopTime = Time.time + loopClip.length - loopOverlap;

        // Optional: fade in the new source
        StartCoroutine(FadeIn(next, 0.1f));

        useA = !useA;
    }

    private void StopLoop()
    {
        isLooping = false;
        audioSourceA.Stop();
        audioSourceB.Stop();
    }

    private System.Collections.IEnumerator FadeIn(AudioSource source, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            source.volume = Mathf.Clamp01(elapsed / duration);
            yield return null;
        }
        source.volume = 1f;
    }
}
