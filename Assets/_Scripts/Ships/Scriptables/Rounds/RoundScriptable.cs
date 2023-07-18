using UnityEngine;

[System.Serializable]
public class Group
{
    public int count = 10;
    public ShipScriptable ship;
}

[CreateAssetMenu(fileName = "Round ", menuName = "ScriptableObjects/Round properties", order = 1)]
public class RoundScriptable : ScriptableObject
{
    public Group[] groups;
}