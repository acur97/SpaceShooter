using UnityEngine;

public class RoundsController : MonoBehaviour
{
    public static RoundsController Instance { get; private set; }

    public LevelScriptable level;

    [Header("-1 means it starts at 0")]
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

            for (int i = groupCount + 1; i < level.rounds[roundCount].groups.Length; i++)
            {
                if (level.rounds[roundCount].groups[i].chained)
                {
                    groupCount++;

                    GameManager.Instance.leftForNextGroup += level.rounds[roundCount].groups[i].count;
                    EnemySpawns.Instance.InstantiateEnemys(level.rounds[roundCount].groups[i]).Forget();
                }
                else
                {
                    break;
                }
            }
        }
        else
        {
            StartRound();
        }
    }
}