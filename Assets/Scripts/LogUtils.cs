using UnityEngine;

public static class LogUtils
{
    public static void DebugLog(object message, Object context = null)
    {
#if UNITY_EDTIOR
            Debug.Log(message, context);
#endif
    }
    
    public static void DebugLogWarning(object message, Object context = null)
    {
#if UNITY_EDTIOR
            Debug.LogWarning(message, context);
#endif
    }
    
    public static void DebugLogError(object message, Object context = null)
    {
#if UNITY_EDTIOR
            Debug.LogError(message, context);
#endif
    }
}