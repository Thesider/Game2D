using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float spawnInterval = 5.0f;
    [SerializeField] private int maxSpawned = 0;
    [SerializeField] private bool spawnOnStart = true;
    [SerializeField] private int poolSize = 10;

    private float timer;
    private readonly List<IEnemy> spawned = new List<IEnemy>();
    private readonly Queue<GameObject> pool = new Queue<GameObject>();

    void Start()
    {
        timer = spawnOnStart ? spawnInterval : 0f;
        if (enemyPrefab == null)
            Debug.LogWarning($"{nameof(EnemySpawner)} on '{gameObject.name}' has no enemyPrefab assigned.");
        if (spawnPoints == null || spawnPoints.Length == 0)
            Debug.LogWarning($"{nameof(EnemySpawner)} on '{gameObject.name}' has no spawnPoints assigned.");

        for (int i = 0; i < poolSize; i++)
        {
            GameObject go = Instantiate(enemyPrefab);
            go.SetActive(false);
            go.transform.SetParent(transform);
            pool.Enqueue(go);
        }
    }

    void Update()
    {
        if (enemyPrefab == null || spawnPoints == null || spawnPoints.Length == 0)
            return;

        for (int i = spawned.Count - 1; i >= 0; i--)
        {
            if (spawned[i] == null || !((MonoBehaviour)spawned[i]).gameObject.activeInHierarchy)
            {
                spawned.RemoveAt(i);
            }
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

        GameObject go = GetPooledEnemy();
        if (go == null) return;

        int idx = Random.Range(0, spawnPoints.Length);
        Transform spawn = spawnPoints[idx];
        go.transform.position = spawn != null ? spawn.position : transform.position;
        go.transform.rotation = spawn != null ? spawn.rotation : Quaternion.identity;
        go.SetActive(true);

        IEnemy enemy = go.GetComponent<IEnemy>();
        if (enemy != null)
        {
            spawned.Add(enemy);
        }
    }

    private GameObject GetPooledEnemy()
    {
        if (pool.Count > 0)
        {
            return pool.Dequeue();
        }
        else
        {
            GameObject go = Instantiate(enemyPrefab);
            go.transform.SetParent(transform);
            return go;
        }
    }

    public void SpawnOnce()
    {
        if (enemyPrefab == null || spawnPoints == null || spawnPoints.Length == 0)
            return;

        for (int i = spawned.Count - 1; i >= 0; i--)
            if (spawned[i] == null || !((MonoBehaviour)spawned[i]).gameObject.activeInHierarchy)
                spawned.RemoveAt(i);

        if (maxSpawned > 0 && spawned.Count >= maxSpawned)
            return;

        SpawnAtRandomPoint();
        timer = 0f;
    }

    public void ClearAllSpawned()
    {
        foreach (IEnemy enemy in spawned)
        {
            if (enemy != null)
            {
                GameObject go = ((MonoBehaviour)enemy).gameObject;
                go.SetActive(false);
                pool.Enqueue(go);
            }
        }
        spawned.Clear();
    }

    public void SpawnWave(int count)
    {
        for (int i = 0; i < count; i++)
        {
            SpawnOnce();
        }
    }
}