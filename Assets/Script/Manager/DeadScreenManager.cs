using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class DeadScreen : MonoBehaviour
{
    public UIDocument deadUIDocument;
    private VisualElement root;
    private Button restartButton, homeButton, quitButton;

    private void Awake()
    {
        if (deadUIDocument == null) deadUIDocument = GetComponent<UIDocument>();
        root = deadUIDocument.rootVisualElement;
        root.style.display = DisplayStyle.None; // hide initially

        restartButton = root.Q<Button>("restartButton");
        homeButton = root.Q<Button>("homeButton");
        quitButton = root.Q<Button>("quitButton");

        restartButton.clicked += OnRestart;
        homeButton.clicked += OnHome;
        quitButton.clicked += OnQuit;
    }

    public void Show()
    {
        root.style.display = DisplayStyle.Flex;
        Time.timeScale = 0f; // pause game
    }

    private void OnRestart()
    {
        Time.timeScale = 1f;

        // Force next spawn to Default
        ScenesManager.SetNextSpawn(ScenesManager.DefaultSpawnId);

        // Reload the current scene via ScenesManager
        if (ScenesManager.Instance != null)
        {
            ScenesManager.Instance.LoadScene((ScenesManager.Scene)Enum.Parse(typeof(ScenesManager.Scene), SceneManager.GetActiveScene().name));
        }
        else
        {
            // fallback
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }


    private void OnHome()
    {
        Debug.Log("Quit to menu button pressed!");
        if (ScenesManager.Instance != null)
        {
            ScenesManager.Instance.LoadMainMenu();
        }
        else
        {
            Debug.LogError("ScenesManager.Instance is NULL! Did you put ScenesManager in the scene?");
        }
    }

    private void OnQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
