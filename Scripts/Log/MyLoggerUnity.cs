#if UNITY_UNITY_5_3_OR_NEWER || UNITY_2017_1_OR_NEWER

using System.Threading;
using UnityEngine;

public class MyLoggerUnity : IMyLogger
{
    void IMyLogger.Log(string msg)
    {
        Debug.Log($"{msg} <color=#0000ff>[{Thread.CurrentThread.ManagedThreadId}]</color>");
    }

    void IMyLogger.LogError(string msg)
    {
        Debug.LogError($"{msg} <color=#0000ff>[{Thread.CurrentThread.ManagedThreadId}]</color>");
    }

    void IMyLogger.LogWarning(string msg)
    {
        Debug.LogWarning($"{msg} <color=#0000ff>[{Thread.CurrentThread.ManagedThreadId}]</color>");
    }
}

#endif