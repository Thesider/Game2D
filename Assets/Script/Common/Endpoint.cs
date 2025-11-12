using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class TriggerPoint : MonoBehaviour
{
    private static readonly ScenesManager.Scene[] SceneOrder =
        (ScenesManager.Scene[])Enum.GetValues(typeof(ScenesManager.Scene));

    public enum TriggerType
    {
        LevelEnd,
        DeathZone
    }

    [SerializeField] private TriggerType triggerType = TriggerType.LevelEnd;
    [SerializeField] private bool advanceToNextSceneInList = true;
    [SerializeField] private ScenesManager.Scene manualSceneOverride = ScenesManager.Scene.MainMenu;
    [SerializeField, Min(0f)] private float additionalSceneLoadDelay = 0f;
    [Header("Spawn Options")]
    [SerializeField] private string nextSceneSpawnId = ScenesManager.DefaultSpawnId;

    private bool hasTriggered;

    private void OnTriggerEnter(Collider other)
    {
        TryHandleTrigger(other != null ? other.gameObject : null);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryHandleTrigger(other != null ? other.gameObject : null);
    }

    private void TryHandleTrigger(GameObject hitObject)
    {
        if (hitObject == null || !hitObject.CompareTag("Player"))
        {
            return;
        }

        switch (triggerType)
        {
            case TriggerType.LevelEnd:
                if (hasTriggered)
                {
                    Debug.Log("[TriggerPoint] LevelEnd trigger already consumed. Ignoring re-entry.");
                    return;
                }

                Debug.Log("[TriggerPoint] Player entered LevelEnd trigger.");
                hasTriggered = true;
                HandleLevelEnd();
                break;

            case TriggerType.DeathZone:
                HandleDeathZone(hitObject);
                break;
        }
    }

    private void HandleLevelEnd()
    {
        float delay = 0f;

        AudioManager manager = ResolveAudioManager();
        if (manager != null)
        {
            delay = manager.PlayLevelEnd();
        }
        else
        {
            Debug.LogWarning("[TriggerPoint] No AudioManager available. Skipping level-end audio.");
        }

        delay = Mathf.Max(delay, additionalSceneLoadDelay);

        ScenesManager.Scene targetScene = DetermineNextScene();
        string spawnId = string.IsNullOrWhiteSpace(nextSceneSpawnId) ? ScenesManager.DefaultSpawnId : nextSceneSpawnId.Trim();
        StartCoroutine(LoadSceneAfterDelay(targetScene, delay, spawnId));
    }

    private ScenesManager.Scene DetermineNextScene()
    {
        if (advanceToNextSceneInList && TryGetNextScene(out ScenesManager.Scene nextScene))
        {
            return nextScene;
        }

        return manualSceneOverride;
    }

    private static bool TryGetNextScene(out ScenesManager.Scene nextScene)
    {
        nextScene = ScenesManager.Scene.MainMenu;

        string activeSceneName = SceneManager.GetActiveScene().name;
        if (!Enum.TryParse(activeSceneName, out ScenesManager.Scene currentScene))
        {
            Debug.LogWarning($"[TriggerPoint] Active scene '{activeSceneName}' is not part of ScenesManager.Scene enum.");
            return false;
        }

        int currentIndex = Array.IndexOf(SceneOrder, currentScene);
        if (currentIndex < 0)
        {
            return false;
        }

        for (int i = currentIndex + 1; i < SceneOrder.Length; i++)
        {
            ScenesManager.Scene candidate = SceneOrder[i];
            if (candidate == ScenesManager.Scene.LoadingScene)
            {
                continue;
            }

            nextScene = candidate;
            return true;
        }

        Debug.LogWarning("[TriggerPoint] No subsequent scene found in ScenesManager.Scene order. Consider disabling 'Advance To Next Scene'.");
        return false;
    }

    private void HandleDeathZone(GameObject playerObject)
    {
        PlayerStatus status = playerObject.GetComponent<PlayerStatus>();
        if (status == null)
        {
            status = playerObject.GetComponentInParent<PlayerStatus>();
        }

        if (status != null)
        {
            status.ForceKill();
        }
        else
        {
            Debug.LogWarning("[TriggerPoint] PlayerStatus component not found on the player object.");
        }
    }

    private IEnumerator LoadSceneAfterDelay(ScenesManager.Scene scene, float delay, string spawnId)
    {
        if (delay > 0f)
        {
            yield return new WaitForSeconds(delay);
        }

        ScenesManager manager = ResolveScenesManager();
        if (manager != null)
        {
            manager.LoadScene(scene, spawnId);
            yield break;
        }

        Debug.LogWarning("[TriggerPoint] ScenesManager instance not found. Falling back to direct scene load.");
        ScenesManager.SetNextSpawn(spawnId);
        SceneManager.LoadScene(scene.ToString());
    }

    private void Reset()
    {
        if (name.IndexOf("Death", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            triggerType = TriggerType.DeathZone;
        }
        else if (name.IndexOf("End", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            triggerType = TriggerType.LevelEnd;
        }
    }
}
