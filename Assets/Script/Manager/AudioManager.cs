using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("-----------Audio Source-------------")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("-----------Audio Clip-------------")]
    [SerializeField, FormerlySerializedAs("background")] private AudioClip defaultBgm;
    [SerializeField] private AudioClip defaultMissionStartClip;
    [SerializeField] private AudioClip levelCompleteClip;
    [SerializeField] private bool playLevelStartByDefault = true;
    [SerializeField] private bool stopMusicWhenLevelEnds = true;
    [SerializeField, Min(0f)] private float minimumLevelEndDelay = 0.5f;

    [Header("Scene Music Overrides")]
    [SerializeField] private SceneAudioConfig[] sceneAudioConfigs = new SceneAudioConfig[0];

    private Coroutine musicRoutine;

    [Serializable]
    private class SceneAudioConfig
    {
        public string sceneName;
        public AudioClip backgroundMusic;
        public AudioClip missionStartClip;
        public bool loopMusic = true;
        public bool playLevelStartSfx = true;
        [Tooltip("Force a delay (seconds) before the BGM starts. Set < 0 to use the intro length.")]
        public float musicDelay = -1f;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= HandleSceneLoaded;
        }
    }

    private void Start()
    {
        if (Instance == this)
        {
            HandleSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
        }
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (string.Equals(scene.name, "LoadingScene", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        if (musicRoutine != null)
        {
            StopCoroutine(musicRoutine);
            musicRoutine = null;
        }

        SceneAudioConfig config = FindConfig(scene.name);

        bool shouldPlayMissionStart = config != null ? config.playLevelStartSfx : playLevelStartByDefault;
        float introDuration = 0f;
        if (shouldPlayMissionStart)
        {
            introDuration = PlayMissionStart(config);
        }

        AudioClip clipToPlay = config != null && config.backgroundMusic != null
            ? config.backgroundMusic
            : defaultBgm;

        if (clipToPlay == null)
        {
            StopMusic();
            return;
        }

        if (musicSource == null)
        {
            return;
        }

    bool loop = config != null ? config.loopMusic : true;
    float delay = config != null && config.musicDelay >= 0f ? config.musicDelay : introDuration;

        musicRoutine = StartCoroutine(PlayMusicAfterDelay(clipToPlay, loop, delay));
    }

    private SceneAudioConfig FindConfig(string sceneName)
    {
        if (sceneAudioConfigs == null)
        {
            return null;
        }

        for (int i = 0; i < sceneAudioConfigs.Length; i++)
        {
            SceneAudioConfig config = sceneAudioConfigs[i];
            if (!string.IsNullOrEmpty(config.sceneName) &&
                string.Equals(config.sceneName, sceneName, StringComparison.OrdinalIgnoreCase))
            {
                return config;
            }
        }

        return null;
    }

    private IEnumerator PlayMusicAfterDelay(AudioClip clip, bool loop, float delay)
    {
        if (delay > 0f)
        {
            yield return new WaitForSeconds(delay);
        }

        if (musicSource == null)
        {
            yield break;
        }

        musicSource.loop = loop;
        musicSource.clip = clip;
        musicSource.Play();
        musicRoutine = null;
    }

    private float PlayMissionStart(SceneAudioConfig config)
    {
        AudioClip clip = null;

        if (config != null)
        {
            clip = config.missionStartClip;
            if (clip == null && defaultMissionStartClip == null && config.playLevelStartSfx)
            {
                Debug.LogWarning($"[AudioManager] Scene '{config.sceneName}' requested mission start audio but no clip was provided.");
            }
        }

        if (clip == null)
        {
            clip = defaultMissionStartClip;
        }

        if (clip == null)
        {
            return 0f;
        }

        return PlaySfx(clip);
    }


    public float PlayLevelEnd()
    {
        if (stopMusicWhenLevelEnds)
        {
            StopMusic();
        }

        float clipLength = PlaySfx(levelCompleteClip);
        return Mathf.Max(clipLength, minimumLevelEndDelay);
    }

    public void StopMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
            musicSource.clip = null;
        }
    }

    public void PlayMusicImmediate(AudioClip clip, bool loop = true)
    {
        if (musicSource == null || clip == null)
        {
            return;
        }

        if (musicRoutine != null)
        {
            StopCoroutine(musicRoutine);
            musicRoutine = null;
        }

        musicSource.loop = loop;
        musicSource.clip = clip;
        musicSource.Play();
    }

    private float PlaySfx(AudioClip clip)
    {
        if (clip == null)
        {
            return 0f;
        }

        AudioSource targetSource = sfxSource != null ? sfxSource : musicSource;
        if (targetSource == null)
        {
            Debug.LogWarning("[AudioManager] No audio source available for SFX playback.");
            return 0f;
        }

        targetSource.PlayOneShot(clip);
        return clip.length;
    }
}
