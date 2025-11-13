using UnityEngine;

public static class GenericNotImplementedError <T> {
    public static T TryGet(T  value, string name) {
        if(value != null) {
            return value;
        }
        Debug.LogError($"The {typeof(T)} is not implemented." + name);
        return default;
    }


}
