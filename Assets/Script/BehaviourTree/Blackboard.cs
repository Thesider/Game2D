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
            SetWeakObject(key, u);
            return;
        }

        data[key] = value;
        weakRefs.Remove(key);
    }

    public bool TryGet<T>(string key, out T value)
    {
        if (data.TryGetValue(key, out var obj) && obj is T t)
        {
            value = t;
            return true;
        }

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
