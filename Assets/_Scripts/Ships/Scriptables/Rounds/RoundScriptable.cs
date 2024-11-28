using UnityEngine;

[System.Serializable]
public struct Group
{
    public bool chained;
    public float timeToStart;

    [Space]
    public Enums.SpawnType spawnType;
    [Range(-2, 2)] public float customFloat;
    public float minTimeBetweenSpawn;
    public float maxTimeBetweenSpawn;

    [Space]
    public int count;
    public ShipScriptable ship;
}

[CreateAssetMenu(fileName = "Round ", menuName = "Gameplay/Level design/Round", order = 1)]
public class RoundScriptable : ScriptableObject
{
    public Group[] groups;
}