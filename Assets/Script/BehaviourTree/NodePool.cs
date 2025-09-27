using System;
using System.Collections.Generic;

// Very small node pooling helper to reduce allocations for frequently-created node types.
// Usage: var n = NodePool.Get<MyNode>(() => new MyNode(...)); NodePool.Return(n);
// This keeps one instance per type by default; extend if you need more capacity.
public static class NodePool
{
    private static readonly Dictionary<Type, object> pool = new Dictionary<Type, object>();

    public static T Get<T>(Func<T> factory) where T : class
    {
        lock (pool)
        {
            if (pool.TryGetValue(typeof(T), out var boxed) && boxed is T instance)
            {
                pool.Remove(typeof(T));
                return instance;
            }
        }
        // fallback to factory
        return factory();
    }

    public static void Return<T>(T instance) where T : class
    {
        if (instance == null) return;
        lock (pool)
        {
            var t = typeof(T);
            if (!pool.ContainsKey(t))
                pool[t] = instance;
            // otherwise drop — simple single-slot cache per type
        }
    }

    // Clear pool (e.g., on scene unload)
    public static void Clear()
    {
        lock (pool)
        {
            pool.Clear();
        }
    }
}
