using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public static class BulletPool
{
    private const int DefaultCapacity = 16;
    private const int MaxCapacity = 128;

    private class PoolWrapper
    {
        public ObjectPool<Bullet> Pool;
        public Transform Root;
    }

    private static readonly Dictionary<GameObject, PoolWrapper> Pools = new Dictionary<GameObject, PoolWrapper>();

    public static Bullet Spawn(
        GameObject prefab,
        Vector3 position,
        Quaternion rotation,
        Vector2 direction,
        float speed,
        float damage,
        float lifetime,
        GameObject owner = null,
        Transform parentOverride = null)
    {
        if (prefab == null)
            return null;

        PoolWrapper wrapper = GetOrCreatePool(prefab);
        if (wrapper == null)
            return null;

        Bullet bullet = wrapper.Pool.Get();
        if (bullet == null)
            return null;

        Transform parent = parentOverride != null ? parentOverride : wrapper.Root;
        if (bullet.transform.parent != parent)
            bullet.transform.SetParent(parent, false);

        bullet.transform.SetPositionAndRotation(position, rotation);
        bullet.Initialize(direction, speed, damage, lifetime, owner);
        return bullet;
    }

    internal static void Release(Bullet bullet)
    {
        bullet?.ReleaseToPool();
    }

    private static PoolWrapper GetOrCreatePool(GameObject prefab)
    {
        if (Pools.TryGetValue(prefab, out PoolWrapper existing))
            return existing;

        var rootObject = new GameObject($"{prefab.name}_Pool");
        Object.DontDestroyOnLoad(rootObject);
        Transform rootTransform = rootObject.transform;

        var wrapper = new PoolWrapper
        {
            Root = rootTransform
        };

        wrapper.Pool = new ObjectPool<Bullet>(
            () => CreateBulletInstance(prefab, wrapper),
            OnTakeFromPool,
            bullet => OnReturnedToPool(bullet, wrapper),
            DestroyPooledBullet,
            collectionCheck: false,
            defaultCapacity: DefaultCapacity,
            maxSize: MaxCapacity);

        Pools[prefab] = wrapper;
        return wrapper;
    }

    private static Bullet CreateBulletInstance(GameObject prefab, PoolWrapper wrapper)
    {
        GameObject go = Object.Instantiate(prefab, wrapper.Root);
        Bullet bullet = go.GetComponent<Bullet>();
        if (bullet == null)
        {
            bullet = go.AddComponent<Bullet>();
        }

        bullet.SetPool(wrapper.Pool);
        go.SetActive(false);
        return bullet;
    }

    private static void OnTakeFromPool(Bullet bullet)
    {
        if (bullet == null)
            return;

        bullet.gameObject.SetActive(true);
    }

    private static void OnReturnedToPool(Bullet bullet, PoolWrapper wrapper)
    {
        if (bullet == null)
            return;

        bullet.ResetAfterRelease();
        bullet.gameObject.SetActive(false);
        if (bullet.transform.parent != wrapper.Root)
            bullet.transform.SetParent(wrapper.Root, false);
    }

    private static void DestroyPooledBullet(Bullet bullet)
    {
        if (bullet == null)
            return;

        Object.Destroy(bullet.gameObject);
    }
}
