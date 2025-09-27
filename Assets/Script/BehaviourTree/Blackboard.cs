using System;
using System.Collections.Generic;
using UnityEngine;


public class Blackboard
{
    readonly Dictionary<string, object> data = new Dictionary<string, object>();
    readonly Dictionary<string, WeakReference<UnityEngine.Object>> weakRefs = new Dictionary<string, WeakReference<UnityEngine.Object>>();

    public void Set<T>(string key, T value)
    {
        if (value is UnityEngine.Object u)
        {
            // If a UnityEngine.Object is stored, prefer weak reference store so we don't keep a strong root.
            SetWeakObject(key, u);
            return;
        }

        data[key] = value;
        // Remove any weak ref stored under the same key
        weakRefs.Remove(key);
    }

    public bool TryGet<T>(string key, out T value)
    {
        // Prefer strong-typed data first
        if (data.TryGetValue(key, out var obj) && obj is T t)
        {
            value = t;
            return true;
        }

        // Then try weak refs (if expected type is UnityEngine.Object or compatible)
        if (typeof(UnityEngine.Object).IsAssignableFrom(typeof(T)))
        {
            if (TryGetWeakObject(key, out var objRef) && objRef is T casted)
            {
                value = casted;
                return true;
            }
        }

        value = default;
        return false;
    }

    // Store UnityEngine.Object as weak reference
    public void SetWeakObject(string key, UnityEngine.Object obj)
    {
        if (obj == null)
        {
            weakRefs.Remove(key);
            data.Remove(key);
            return;
        }

        weakRefs[key] = new WeakReference<UnityEngine.Object>(obj);
        data.Remove(key);
    }

    // Try to get a UnityEngine.Object stored as weak reference
    public bool TryGetWeakObject(string key, out UnityEngine.Object obj)
    {
        obj = null;
        if (weakRefs.TryGetValue(key, out var wr))
        {
            if (wr.TryGetTarget(out var target) && target != null)
            {
                obj = target;
                return true;
            }
            // Weak ref target was collected / destroyed -> remove entry
            weakRefs.Remove(key);
        }
        return false;
    }

    public bool Has(string key) => data.ContainsKey(key) || weakRefs.ContainsKey(key);
    public void Remove(string key)
    {
        data.Remove(key);
        weakRefs.Remove(key);
    }
    public void Clear()
    {
        data.Clear();
        weakRefs.Clear();
    }
}
