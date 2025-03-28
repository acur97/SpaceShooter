using UnityEngine;

[CreateAssetMenu(fileName = "Round ", menuName = "Gameplay/Level design/Round", order = 1)]
public class RoundScriptable : ScriptableObject
{
    public Group[] groups;

    // Editor
    public bool editorNotes = true;
    public string notes;
}

[System.Serializable]
public struct Group
{
    // Editor
    public bool groupEnable;

    public bool chained;
    public float timeToStart;
    public Enums.SpawnType spawnType;
    [Range(-2, 2)] public float customFloat;
    public float minTimeBetweenSpawn;
    public float maxTimeBetweenSpawn;

    [Space]
    public int count;
    public ShipScriptable ship;

    [Space]
    public bool randomPowerUp;
    public PowerUpBase spawnPowerUp;
}