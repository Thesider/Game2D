using UnityEngine;

public partial class TriggerPoint : MonoBehaviour
{
    [Header("Audio Overrides")]
    [SerializeField] private AudioManager audioManagerOverride;

    [Header("Scene Overrides")]
    [SerializeField] private ScenesManager scenesManagerOverride;

    private AudioManager cachedAudioManager;
    private ScenesManager cachedScenesManager;

    protected AudioManager ResolveAudioManager()
    {
        if (audioManagerOverride != null)
        {
            return audioManagerOverride;
        }

        if (AudioManager.Instance != null)
        {
            return AudioManager.Instance;
        }

        if (cachedAudioManager == null)
        {
            cachedAudioManager = FindFirstObjectByType<AudioManager>();
        }

        return cachedAudioManager;
    }

    protected ScenesManager ResolveScenesManager()
    {
        if (scenesManagerOverride != null)
        {
            return scenesManagerOverride;
        }

        if (ScenesManager.Instance != null)
        {
            return ScenesManager.Instance;
        }

        if (cachedScenesManager == null)
        {
            cachedScenesManager = FindFirstObjectByType<ScenesManager>();
        }

        return cachedScenesManager;
    }
}
