using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuEventListener : MonoBehaviour
{
    private UIDocument _document;
    private Button _newGameButton;
    private Button _loadGameButton;
    private Button _quitGameButton;
    private Button _settingGameButton;
    private VisualElement _buttonsWrapper;

    [SerializeField] private VisualTreeAsset _settingButtonPage;
    private VisualElement _settingButton;

    private Slider _musicSlider;

    private void Awake()
    {
        _document = GetComponent<UIDocument>();

        _newGameButton = _document.rootVisualElement.Q<Button>("NewGame");
        _loadGameButton = _document.rootVisualElement.Q<Button>("LoadGame");
        _settingGameButton = _document.rootVisualElement.Q<Button>("Setting");
        _quitGameButton = _document.rootVisualElement.Q<Button>("QuitGame");
        _buttonsWrapper = _document.rootVisualElement.Q<VisualElement>("Container");

        if (_newGameButton != null)
            _newGameButton.RegisterCallback<ClickEvent>(OnNewGameClick);

        if (_loadGameButton != null)
            _loadGameButton.RegisterCallback<ClickEvent>(OnLoadGameClick);

        if (_quitGameButton != null)
            _quitGameButton.RegisterCallback<ClickEvent>(OnQuitGameClick);

        _settingGameButton.clicked += SettingButtonOnClicked;

        // Prepare the setting page but don't show it yet
        _settingButton = _settingButtonPage.CloneTree();
        var backButton = _settingButton.Q<Button>("BackButton");
        backButton.clicked += BackButtonOnClicked;

        // Get the slider from the cloned settings page
        _musicSlider = _settingButton.Q<Slider>("MusicSlider");

        // Initialize and register events for music volume
        if (_musicSlider != null)
        {
            // Set initial value based on current AudioManager volume
            if (AudioManager.Instance != null)
                _musicSlider.value = AudioManager.Instance.GetMusicVolume() * 100f;

            // When user moves the slider, change volume
            _musicSlider.RegisterValueChangedCallback(evt =>
            {
                if (AudioManager.Instance != null)
                {
                    float newVolume = evt.newValue / 100f;
                    AudioManager.Instance.SetMusicVolume(newVolume);
                }
            });
        }
    }

    private void SettingButtonOnClicked()
    {
        _buttonsWrapper.Clear();
        _buttonsWrapper.Add(_settingButton);
    }

    private void BackButtonOnClicked()
    {
        _buttonsWrapper.Clear();
        _buttonsWrapper.Add(_newGameButton);
        _buttonsWrapper.Add(_loadGameButton);
        _buttonsWrapper.Add(_settingGameButton);
        _buttonsWrapper.Add(_quitGameButton);
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
            ScenesManager.Instance.LoadNewGame();
        else
            Debug.LogError("ScenesManager.Instance is NULL! Did you put ScenesManager in the scene?");
    }

    private void OnLoadGameClick(ClickEvent evt)
    {
        Debug.Log("Load Game button pressed!");

        if (ScenesManager.Instance != null)
            ScenesManager.Instance.LoadNewGame();
        else
            Debug.LogError("ScenesManager.Instance is NULL! Did you put ScenesManager in the scene?");
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
