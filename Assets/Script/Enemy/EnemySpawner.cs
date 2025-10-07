using System.Collections.Generic;
using UnityEngine;

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
            if (enemy == null)
            {
                spawned.RemoveAt(i);
                continue;
            }

            try
            {
                GameObject go = ((MonoBehaviour)enemy).gameObject;
                if (go != null)
                {
                    go.SetActive(false);
                    pool.Enqueue(go);
                }
            }
            catch (System.Exception)
            {
                // If accessing gameObject throws (destroyed), just ignore and continue.
            }

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
            // If the reference is already null according to Unity, remove it.
            if (enemy == null)
            {
                spawned.RemoveAt(i);
                continue;
            }

            // Safely check activeInHierarchy without throwing if the native object was destroyed.
            try
            {
                var mb = enemy as MonoBehaviour;
                if (mb == null || mb.gameObject == null || !mb.gameObject.activeInHierarchy)
                {
                    spawned.RemoveAt(i);
                }
            }
            catch (System.Exception)
            {
                // Accessing properties of a destroyed Unity object can throw in some editor/runtime states.
                // Remove the entry to avoid MissingReferenceException.
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
}