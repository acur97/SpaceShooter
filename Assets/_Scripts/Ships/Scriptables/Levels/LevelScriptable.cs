using UnityEngine;

[CreateAssetMenu(fileName = "Level ", menuName = "Gameplay/Level design/Level", order = 0)]
public class LevelScriptable : ScriptableObject
{
    public RoundScriptable[] rounds;

    [TextArea] public string notes;
}