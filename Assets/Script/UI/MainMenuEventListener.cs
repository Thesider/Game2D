using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuEventListener : MonoBehaviour
{
    private UIDocument _document;
    private Button _newGameButton;
    private Button _loadGameButton;
    private Button _quitGameButton;

    private void Awake()
    {
        _document = GetComponent<UIDocument>();

        _newGameButton = _document.rootVisualElement.Q<Button>("NewGame");
        _loadGameButton = _document.rootVisualElement.Q<Button>("LoadGame");
        _quitGameButton = _document.rootVisualElement.Q<Button>("QuitGame");

        if (_newGameButton != null)
            _newGameButton.RegisterCallback<ClickEvent>(OnNewGameClick);

        if (_loadGameButton != null)
            _loadGameButton.RegisterCallback<ClickEvent>(OnLoadGameClick);

        if (_quitGameButton != null)
            _quitGameButton.RegisterCallback<ClickEvent>(OnQuitGameClick);
    }

    private void OnDisable()
    {
        if (_newGameButton != null)
            _newGameButton.UnregisterCallback<ClickEvent>(OnNewGameClick);

        if (_loadGameButton != null)
            _loadGameButton.UnregisterCallback<ClickEvent>(OnLoadGameClick);

        if (_quitGameButton != null)
            _quitGameButton.UnregisterCallback<ClickEvent>(OnQuitGameClick);
    }

    private void OnNewGameClick(ClickEvent evt)
    {
        Debug.Log("New Game button pressed!");

        if (ScenesManager.Instance != null)
        {
            ScenesManager.Instance.LoadNewGame();
        }
        else
        {
            Debug.LogError("ScenesManager.Instance is NULL! Did you put ScenesManager in the scene?");
        }
    }


    private void OnLoadGameClick(ClickEvent evt)
    {
        Debug.Log("Load Game button pressed!");
        if (ScenesManager.Instance != null)
        {
            ScenesManager.Instance.LoadNewGame();
        }
        else
        {
            Debug.LogError("ScenesManager.Instance is NULL! Did you put ScenesManager in the scene?");
        }
    }

    private void OnQuitGameClick(ClickEvent evt)
    {
        Debug.Log("Quit Game button pressed!");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // stops play mode in Editor
#else
        Application.Quit(); // quits app in build
#endif
    }
}
