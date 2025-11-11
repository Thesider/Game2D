using UnityEngine;

[DisallowMultipleComponent]
public class PlayerSpawnPoint : MonoBehaviour
{
    [SerializeField] private string spawnId = ScenesManager.DefaultSpawnId;

    public string SpawnId => string.IsNullOrWhiteSpace(spawnId) ? ScenesManager.DefaultSpawnId : spawnId.Trim();

    private void OnValidate()
    {
        spawnId = string.IsNullOrWhiteSpace(spawnId) ? ScenesManager.DefaultSpawnId : spawnId.Trim();
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 0.3f);
        Gizmos.DrawLine(transform.position, transform.position + transform.right * 0.5f);
        UnityEditor.Handles.color = Color.cyan;
        UnityEditor.Handles.Label(transform.position + Vector3.up * 0.35f, SpawnId);
    }
#endif
}
