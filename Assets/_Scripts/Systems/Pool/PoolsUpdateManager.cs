using System;
using UnityEngine;

public class PoolsUpdateManager : MonoBehaviour
{
    public static Action PoolUpdate;

    void Update()
    {
        PoolUpdate?.Invoke();
    }
}