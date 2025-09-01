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
    [Tooltip("The \"Time to start\" starts counting together with the previous group")] public bool chained;
    [Tooltip("Time to start this group when the previous one ends")] public float timeToStart;
    [Tooltip("How the enemy appears")] public Enums.SpawnType spawnType;
    [Tooltip("Float custom (Especific, etc)"), Range(-2, 2)] public float customFloat;
    [Tooltip("Min time between each enemy's appearance")] public float minTimeBetweenSpawn;
    [Tooltip("Max time between each enemy's appearance")] public float maxTimeBetweenSpawn;

    [Space]
    [Tooltip("Number of enemies")] public int count;
    [Tooltip("Enemy")] public ShipScriptable ship;

    [Space]
    [Tooltip("PowerUp to spawn at the end of this group (empty means none will spawn)")] public PowerUpBase spawnPowerUp;
    [Tooltip("Use a random PowerUp from all the existing ones?")] public bool randomPowerUp;
}