using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float spawnInterval = 5.0f;

    [Tooltip("Maximum number of spawned enemies tracked by this spawner. Set to 0 for unlimited.")]
    [SerializeField] private int maxSpawned = 0;

    [Tooltip("If true, spawns immediately on Start.")]
    [SerializeField] private bool spawnOnStart = true;

    private float timer;
    private readonly List<GameObject> spawned = new List<GameObject>();

    void Start()
    {
        timer = spawnOnStart ? spawnInterval : 0f;
        if (enemyPrefab == null)
            Debug.LogWarning($"{nameof(EnemySpawner)} on '{gameObject.name}' has no enemyPrefab assigned.");
        if (spawnPoints == null || spawnPoints.Length == 0)
            Debug.LogWarning($"{nameof(EnemySpawner)} on '{gameObject.name}' has no spawnPoints assigned.");
    }

    void Update()
    {
        if (enemyPrefab == null || spawnPoints == null || spawnPoints.Length == 0)
            return;

        for (int i = spawned.Count - 1; i >= 0; i--)
        {
            if (spawned[i] == null)
                spawned.RemoveAt(i);
        }

        if (maxSpawned > 0 && spawned.Count >= maxSpawned)
            return;
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnAtRandomPoint();
            timer = 0f;
        }
    }

    private void SpawnAtRandomPoint()
    {
        if (spawnPoints.Length == 0)
            return;

        int idx = Random.Range(0, spawnPoints.Length);
        Transform spawn = spawnPoints[idx];
        Vector3 pos = spawn != null ? spawn.position : transform.position;
        Quaternion rot = spawn != null ? spawn.rotation : Quaternion.identity;

        GameObject go = Instantiate(enemyPrefab, pos, rot);
        go.transform.SetParent(transform);
        spawned.Add(go);
    }

    public void SpawnOnce()
    {
        if (enemyPrefab == null || (spawnPoints == null || spawnPoints.Length == 0))
            return;

        for (int i = spawned.Count - 1; i >= 0; i--)
            if (spawned[i] == null) spawned.RemoveAt(i);

        if (maxSpawned > 0 && spawned.Count >= maxSpawned)
            return;

        SpawnAtRandomPoint();
        timer = 0f;
    }

    public void ClearAllSpawned()
    {
        for (int i = 0; i < spawned.Count; i++)
        {
            if (spawned[i] != null)
                Destroy(spawned[i]);
        }
        spawned.Clear();
    }
}