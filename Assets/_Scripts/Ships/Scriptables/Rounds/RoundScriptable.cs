using UnityEngine;

[System.Serializable]
public class Group
{
    public float timeToStart = 2;

    [Space]
    public int count = 10;
    public ShipScriptable ship;
    public enum SpawnType { random, row, only, allAtOnce }
    public SpawnType spawnType;
    public float minTimeBetweenSpawn = 0.3f;
    public float maxTimeBetweenSpawn = 0.3f;
}

[CreateAssetMenu(fileName = "Round ", menuName = "ScriptableObjects/Round properties", order = 1)]
public class RoundScriptable : ScriptableObject
{
    public Group[] groups;
}