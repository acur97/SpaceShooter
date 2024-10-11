using UnityEngine;

[CreateAssetMenu(fileName = "Level ", menuName = "ScriptableObjects/Level properties", order = 2)]
public class LevelScriptable : ScriptableObject
{
    public RoundScriptable[] rounds;
}