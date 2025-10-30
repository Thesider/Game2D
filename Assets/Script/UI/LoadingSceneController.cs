using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class LoadingSceneController : MonoBehaviour
{
    private Slider progressBar;
    private Label loadingLabel;

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        progressBar = root.Q<Slider>("progressBar");
        loadingLabel = root.Q<Label>("loadingLabel");

        if (string.IsNullOrEmpty(ScenesManager.NextSceneName))
        {
            Debug.LogError("[LoadingSceneController] No target scene set!");
            return;
        }

        StartCoroutine(LoadSceneAsync(ScenesManager.NextSceneName));
    }

    private IEnumerator LoadSceneAsync(string targetScene)
    {
        Debug.Log($"[LoadingSceneController] Loading: {targetScene}");
        AsyncOperation op = SceneManager.LoadSceneAsync(targetScene);
        op.allowSceneActivation = false;

        while (!op.isDone)
        {
            float progress = Mathf.Clamp01(op.progress / 0.9f);
            progressBar.value = progress * 100f;
            loadingLabel.text = $"Loading... {Mathf.RoundToInt(progress * 100)}%";

            if (op.progress >= 0.9f)
            {
                yield return new WaitForSeconds(0.3f);
                op.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
