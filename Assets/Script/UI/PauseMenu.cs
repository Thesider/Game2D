using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    public UIDocument pauseUIDocument;
    public InputActionAsset inputActions;  // Drag your InputActions asset here in Inspector

    private VisualElement root;
    private Button resumeBtn, restartBtn, saveGameBtn, quitToMenuBtn, quitBtn;
        private bool isPaused = false;

    private InputAction toggleMenu;
    private InputActionMap playerMap;
    private InputActionMap uiMap;

    void Awake()
    {
        if (pauseUIDocument == null) pauseUIDocument = GetComponent<UIDocument>();
        root = pauseUIDocument.rootVisualElement;

        // Get UI Buttons
        resumeBtn = root.Q<Button>("ResumeButton");
        restartBtn = root.Q<Button>("RestartButton");
        quitToMenuBtn = root.Q<Button>("QuitToMenuButton");
        quitBtn = root.Q<Button>("QuitToDesktopButton");

        saveGameBtn = root.Q<Button>("SaveGameButton");
        if (saveGameBtn != null) saveGameBtn.clicked += OnSaveGame;

        if (resumeBtn != null) resumeBtn.clicked += OnResume;
        if (restartBtn != null) restartBtn.clicked += OnRestart;
        if (quitToMenuBtn != null) quitToMenuBtn.clicked += OnQuitToMenu;
        if (quitBtn != null) quitBtn.clicked += OnQuit;

        // Get action maps
        playerMap = inputActions.FindActionMap("Player"); // your gameplay map name
        uiMap = inputActions.FindActionMap("UI");

        if (uiMap != null)
        {
            toggleMenu = uiMap.FindAction("ToggleMenu");
            if (toggleMenu == null)
                Debug.LogError("❌ ToggleMenu action not found in UI map!");
        }
        else
        {
            Debug.LogError("❌ UI map not found in InputActions!");
        }
    }

    


    void Start()
    {
        // ✅ Always start unpaused
        Resume();
    }

    void OnEnable()
    {
        if (toggleMenu != null)
            toggleMenu.performed += OnPausePressed;
    }

    void OnDisable()
    {
        if (toggleMenu != null)
            toggleMenu.performed -= OnPausePressed;
    }

    private void OnPausePressed(InputAction.CallbackContext ctx)
    {
        if (isPaused) Resume();
        else Pause();
    }

    public void Pause()
    {
        isPaused = true;
        root.style.display = DisplayStyle.Flex;
        Time.timeScale = 0f;

        // disable player controls, keep UI
        playerMap?.Disable();
    }

    public void Resume()
    {
        isPaused = false;
        root.style.display = DisplayStyle.None;
        Time.timeScale = 1f;

        // re-enable player controls
        playerMap?.Enable();
        uiMap?.Enable(); // keep listening for ESC
    }

    // Button callbacks
    private void OnResume() => Resume();

    private void OnRestart()
    {
        Time.timeScale = 1f;
        root.style.display = DisplayStyle.None;
        StartCoroutine(RestartLevelClean());
    }

    private System.Collections.IEnumerator RestartLevelClean()
    {
        yield return null;

        if (ScenesManager.Instance != null)
        {
            var current = SceneManager.GetActiveScene().name;
            ScenesManager.SetNextSpawn(ScenesManager.DefaultSpawnId);

            if (System.Enum.TryParse(current, out ScenesManager.Scene scene))
            {
                ScenesManager.Instance.LoadScene(scene);
            }
            else
            {
                // fallback if not enum name
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
    private void OnSaveGame()
    {
        PlayerStatus player = FindObjectOfType<PlayerStatus>();
        if (player != null)
        {
            player.SavePlayerData();
            Debug.Log("💾 Game saved via Pause Menu!");
        }
        else
        {
            Debug.LogWarning("⚠️ No PlayerStatus found to save!");
        }
    }



    private void OnQuitToMenu()
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
