using UnityEngine;

[System.Serializable]
public struct Group
{
    public float timeToStart;

    public enum SpawnType { random, row, only, allAtOnce }

    [Space]
    public SpawnType spawnType;
    public int count;
    public ShipScriptable ship;
    public float minTimeBetweenSpawn;
    public float maxTimeBetweenSpawn;
}

[CreateAssetMenu(fileName = "Round ", menuName = "ScriptableObjects/Round properties", order = 1)]
public class RoundScriptable : ScriptableObject
{
    public Group[] groups;
}