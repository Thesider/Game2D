using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoBehaviour
{
    public static ScenesManager Instance;
<<<<<<< HEAD

    void Awake()
    {
        Instance = this;
    }
    public enum Scene
    {
        MainMenu,
        SampleScene_Map_0
    }


    public void LoadScene(Scene scene)
    {
        SceneManager.LoadScene(scene.ToString());
    }

    public void LoadNewGame()
    {
        SceneManager.LoadScene(Scene.SampleScene_Map_0.ToString());
    }

    public void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(Scene.MainMenu.ToString());
=======
    public static string NextSceneName;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Only the manager stays
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public enum Scene
    {
        MainMenu,
        LoadingScene,
        SampleScene_Map_0,
        Map_1
    }

    // This method always goes through the loading scene
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

    private void DestroyIfExists(string name)
    {
        GameObject obj = GameObject.Find(name);
        if (obj != null)
        {
            Debug.Log($"[ScenesManager] Destroying leftover object: {name}");
            Destroy(obj);
        }
>>>>>>> main
    }
}
