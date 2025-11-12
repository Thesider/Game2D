using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private Transform player;
    [SerializeField] private float spawnInterval = 5.0f;
    [SerializeField] private int maxSpawned = 0;
    [SerializeField] private bool spawnOnStart = true;
    [SerializeField] private int poolSize = 10;

    private float timer;
    private readonly List<IEnemy> spawned = new List<IEnemy>();
    private ObjectPool<GameObject> enemyPool;

    private void InitializePool()
    {
        if (enemyPrefab == null)
        {
            enemyPool = null;
            return;
        }

        int defaultCapacity = Mathf.Max(poolSize, 1);
        int maxCapacity = poolSize > 0 ? Mathf.Max(poolSize, poolSize * 2) : Mathf.Max(defaultCapacity * 4, 16);

        enemyPool = new ObjectPool<GameObject>(
            CreateEnemy,
            OnTakeFromPool,
            OnReturnedToPool,
            DestroyPooledEnemy,
            collectionCheck: false,
            defaultCapacity: defaultCapacity,
            maxSize: maxCapacity);

        if (poolSize > 0)
        {
            for (int i = 0; i < poolSize; i++)
            {
                var go = enemyPool.Get();
                if (go == null)
                    break;
                enemyPool.Release(go);
            }
        }
    }

    void Start()
    {
        timer = spawnOnStart ? spawnInterval : 0f;
        if (enemyPrefab == null)
            Debug.LogWarning($"{nameof(EnemySpawner)} on '{gameObject.name}' has no enemyPrefab assigned.");
        if (spawnPoints == null || spawnPoints.Length == 0)
            Debug.LogWarning($"{nameof(EnemySpawner)} on '{gameObject.name}' has no spawnPoints assigned.");

        InitializePool();
    }

    void Update()
    {
        if (enemyPrefab == null || spawnPoints == null || spawnPoints.Length == 0)
            return;

        // Remove destroyed or inactive enemies safely before accessing the list.
        CleanUpSpawned();

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
        // Set player on the enemy before enabling so states can use it immediately
        IEnemy enemy = go.GetComponent<IEnemy>();
        if (enemy != null)
        {
            // If spawner has no player assigned, try to find one in scene once
            if (player == null)
            {
                GameObject p = GameObject.FindGameObjectWithTag("Player");
                if (p != null) player = p.transform;
            }

            if (player != null)
            {
                try { enemy.Player = player; } catch { }
            }

            spawned.Add(enemy);
        }

        go.SetActive(true);
    }

    private GameObject GetPooledEnemy()
    {
        if (enemyPrefab == null)
            return null;

        return enemyPool?.Get();
    }

    private void ReleaseEnemy(IEnemy enemy)
    {
        if (enemy == null || enemyPool == null)
            return;

        var mono = enemy as MonoBehaviour;
        if (mono == null)
            return;

        var go = mono.gameObject;
        if (go == null)
            return;

        if (go.activeSelf)
            go.SetActive(false);

        enemyPool.Release(go);
    }

    public void SpawnOnce()
    {
        if (enemyPrefab == null || spawnPoints == null || spawnPoints.Length == 0)
            return;

        CleanUpSpawned();

        if (maxSpawned > 0 && spawned.Count >= maxSpawned)
            return;

        SpawnAtRandomPoint();
        timer = 0f;
    }

    public void ClearAllSpawned()
    {
        for (int i = spawned.Count - 1; i >= 0; i--)
        {
            var enemy = spawned[i];
            ReleaseEnemy(enemy);
            spawned.RemoveAt(i);
        }
        spawned.Clear();
    }

    // Remove destroyed or inactive entries from the spawned list safely.
    private void CleanUpSpawned()
    {
        for (int i = spawned.Count - 1; i >= 0; i--)
        {
            var enemy = spawned[i];
            if (enemy == null)
            {
                spawned.RemoveAt(i);
                continue;
            }

            var mono = enemy as MonoBehaviour;
            if (mono == null)
            {
                spawned.RemoveAt(i);
                continue;
            }

            var go = mono.gameObject;
            if (go == null)
            {
                spawned.RemoveAt(i);
                continue;
            }

            if (!go.activeInHierarchy)
            {
                ReleaseEnemy(enemy);
                spawned.RemoveAt(i);
            }
        }
    }

    public void SpawnWave(int count)
    {
        for (int i = 0; i < count; i++)
        {
            SpawnOnce();
        }
    }

    private GameObject CreateEnemy()
    {
        if (enemyPrefab == null)
            return null;

        var go = Instantiate(enemyPrefab, transform);
        go.SetActive(false);
        return go;
    }

    private void OnTakeFromPool(GameObject go)
    {
        if (go == null)
            return;

        if (go.transform.parent != transform)
            go.transform.SetParent(transform, false);

        go.SetActive(false);
    }

    private void OnReturnedToPool(GameObject go)
    {
        if (go == null)
            return;

        go.SetActive(false);
        if (go.transform.parent != transform)
            go.transform.SetParent(transform, false);
    }

    private void DestroyPooledEnemy(GameObject go)
    {
        if (go != null)
            Destroy(go);
    }
}