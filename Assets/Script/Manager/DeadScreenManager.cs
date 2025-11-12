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
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnHome()
    {
        Time.timeScale = 1f;
        ScenesManager.Instance.LoadMainMenu();
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
