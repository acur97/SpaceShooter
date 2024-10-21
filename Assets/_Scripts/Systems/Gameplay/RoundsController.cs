using UnityEngine;

public class RoundsController : MonoBehaviour
{
    public static RoundsController Instance { get; private set; }

    public LevelScriptable level;

    [SerializeField] private int roundCount = -1;
    [SerializeField] private int groupCount = -1;

    public void Init()
    {
        Instance = this;
    }

    public void StartRound()
    {
        if (roundCount < level.rounds.Length - 1)
        {
            roundCount++;
            groupCount = -1;

            StartGroup();
        }
        else
        {
            GameManager.Instance.EndLevel();
        }
    }

    public void StartGroup()
    {
        if (groupCount < level.rounds[roundCount].groups.Length - 1)
        {
            groupCount++;

            GameManager.Instance.leftForNextGroup = level.rounds[roundCount].groups[groupCount].count;
            EnemySpawns.Instance.InstantiateEnemys(level.rounds[roundCount].groups[groupCount]).Forget();
        }
        else
        {
            StartRound();
        }
    }
}