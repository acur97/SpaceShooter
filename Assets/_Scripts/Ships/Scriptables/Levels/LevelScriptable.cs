using UnityEngine;

[CreateAssetMenu(fileName = "Level ", menuName = "Gameplay/Level", order = 0)]
public class LevelScriptable : ScriptableObject
{
    public RoundScriptable[] rounds;
}