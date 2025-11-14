using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishTrigger : MonoBehaviour
{
    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;

        if (other.CompareTag("Player"))
        {
            triggered = true;
            Debug.Log("🏁 Player reached finish line!");

            // If you have ScenesManager
            if (ScenesManager.Instance != null)
            {
                ScenesManager.Instance.LoadScene(ScenesManager.Scene.LevelFinish);
            }
            else
            {
                SceneManager.LoadScene("FinishScene");
            }
        }
    }
}
