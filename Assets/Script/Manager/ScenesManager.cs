using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoBehaviour
{
    public static ScenesManager Instance;
    public static string NextSceneName;
    public static string NextSpawnId { get; private set; } = DefaultSpawnId;

    public const string DefaultSpawnId = "Default";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void ResetStatics()
    {
        Instance = null;
        NextSceneName = string.Empty;
        NextSpawnId = DefaultSpawnId;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += HandleSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= HandleSceneLoaded;
        }
    }

    public enum Scene
    {
        MainMenu,
        LoadingScene,
        SampleScene_Map_0,
        Map_1,
        LevelFinish  
    }


    public void LoadScene(Scene target)
    {
        if (target == Scene.LoadingScene)
        {
            Debug.LogWarning("Cannot load LoadingScene directly.");
            return;
        }

        // Before changing, clean up any old menu UI
        DestroyIfExists("MainMenu");
        DestroyIfExists("PauseMenu");

        NextSceneName = target.ToString();
        Debug.Log($"[ScenesManager] → Preparing to load {NextSceneName}");

        SceneManager.LoadScene(Scene.LoadingScene.ToString());
    }

    public void LoadNewGame() => LoadScene(Scene.SampleScene_Map_0);
    public void LoadMainMenu() => LoadScene(Scene.MainMenu);

    private void DestroyIfExists(string name, string exception = null)
    {
        if (!string.IsNullOrEmpty(exception) && name == exception)
            return;

        GameObject obj = GameObject.Find(name);
        if (obj != null)
        {
            Debug.Log($"[ScenesManager] Destroying leftover object: {name}");
            Destroy(obj);
        }
    }

    public void LoadScene(Scene target, string spawnId = DefaultSpawnId)
    {
        if (target == Scene.LoadingScene)
        {
            Debug.LogWarning("Cannot load LoadingScene directly.");
            return;
        }
        DestroyIfExists("MainMenu", target.ToString());
        DestroyIfExists("PauseMenu", target.ToString());


        NextSceneName = target.ToString();
        SetNextSpawn(spawnId);
        Debug.Log($"[ScenesManager] → Preparing to load {NextSceneName} (Spawn: {NextSpawnId})");

        SceneManager.LoadScene(Scene.LoadingScene.ToString());
    }

   

    public static void SetNextSpawn(string spawnId)
    {
        if (string.IsNullOrWhiteSpace(spawnId))
        {
            NextSpawnId = DefaultSpawnId;
            return;
        }

        NextSpawnId = spawnId.Trim();
    }

    
    private void HandleSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode)
    {
        if (scene.name == Scene.LoadingScene.ToString())
        {
            return;
        }

        TryMovePlayerToSpawn(scene.name);
        NextSceneName = string.Empty;
        SetNextSpawn(DefaultSpawnId);
    }

    private void TryMovePlayerToSpawn(string sceneName)
    {
        bool requestedSpawn = !string.IsNullOrEmpty(NextSceneName);

        PlayerController[] players = FindObjectsByType<PlayerController>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        if (players == null || players.Length == 0)
        {
            if (requestedSpawn)
            {
                Debug.LogWarning($"[ScenesManager] No PlayerController found when entering scene '{sceneName}'.");
            }
            return;
        }

        PlayerController player = players[0];

        PlayerSpawnPoint[] spawnPoints = FindObjectsByType<PlayerSpawnPoint>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            if (requestedSpawn)
            {
                Debug.LogWarning($"[ScenesManager] Scene '{sceneName}' has no PlayerSpawnPoint. Player position unchanged.");
            }
            return;
        }

        string desiredSpawn = string.IsNullOrWhiteSpace(NextSpawnId) ? DefaultSpawnId : NextSpawnId;
        PlayerSpawnPoint target = null;

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            PlayerSpawnPoint candidate = spawnPoints[i];
            if (candidate != null && string.Equals(candidate.SpawnId, desiredSpawn, StringComparison.OrdinalIgnoreCase))
            {
                target = candidate;
                break;
            }
        }

        if (target == null)
        {
            for (int i = 0; i < spawnPoints.Length; i++)
            {
                PlayerSpawnPoint candidate = spawnPoints[i];
                if (candidate != null && string.Equals(candidate.SpawnId, DefaultSpawnId, StringComparison.OrdinalIgnoreCase))
                {
                    target = candidate;
                    break;
                }
            }

            if (target == null)
            {
                target = spawnPoints[0];
            }

            if (!string.Equals(target.SpawnId, desiredSpawn, StringComparison.OrdinalIgnoreCase))
            {
                Debug.LogWarning($"[ScenesManager] Spawn '{desiredSpawn}' not found. Using '{target.SpawnId}'.");
            }
        }

        player.transform.SetPositionAndRotation(target.transform.position, target.transform.rotation);

        Rigidbody2D body = player.GetComponent<Rigidbody2D>();
        if (body != null)
        {
            body.linearVelocity = Vector2.zero;
            body.angularVelocity = 0f;
        }
    }
    //
}
