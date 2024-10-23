using System;
using UnityEngine;

public class PoolsUpdateManager : MonoBehaviour
{
    public static Action PoolUpdate;

    private void Update()
    {
        PoolUpdate?.Invoke();
    }
}