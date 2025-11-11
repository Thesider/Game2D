using UnityEngine;

public class CameraProperty : MonoBehaviour
{
    private void Awake()
    {
        CameraProperty[] cameras = FindObjectsByType<CameraProperty>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        for (int i = 0; i < cameras.Length; i++)
        {
            CameraProperty cameraProperty = cameras[i];
            if (cameraProperty != this)
            {
                Debug.LogWarning("[CameraProperty] Duplicate camera detected. Destroying the newest instance.");
                Destroy(transform.root.gameObject);
                return;
            }
        }

        DontDestroyOnLoad(transform.root.gameObject);
    }
}
