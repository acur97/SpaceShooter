using System;
using UnityEngine;

public class PoolsUpdateManager : MonoBehaviour
{
    public static Action PoolUpdate;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        PoolUpdate = null;
    }

    private void Update()
    {
        PoolUpdate?.Invoke();
    }
}