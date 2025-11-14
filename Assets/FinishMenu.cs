using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class FinishMenu : MonoBehaviour
{
    private Button mainMenuBtn;
    private Button quitBtn;

    private void Awake()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        mainMenuBtn = root.Q<Button>("MainMenuButton");
        quitBtn = root.Q<Button>("QuitButton");

        if (mainMenuBtn != null)
            mainMenuBtn.clicked += ReturnToMenu;

        if (quitBtn != null)
            quitBtn.clicked += QuitGame;
    }

    private void ReturnToMenu()
    {
        Debug.Log("Returning to Main Menu...");

        if (ScenesManager.Instance != null)
        {
            ScenesManager.Instance.LoadMainMenu();
        }
        else
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    private void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
