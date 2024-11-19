using UnityEngine;

public class RoundsController : MonoBehaviour
{
    public static RoundsController Instance { get; private set; }

    [SerializeField] private GameplayScriptable gameplayScriptable;

    [Header("-1 means it starts at 0")]
    [SerializeField] private int roundCount = -1;
    [SerializeField] private int groupCount = -1;

    public void Init()
    {
        Instance = this;
    }

    public void StartRound()
    {
        if (roundCount < gameplayScriptable.levels[0].rounds.Length - 1)
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
        if (groupCount < gameplayScriptable.levels[0].rounds[roundCount].groups.Length - 1)
        {
            groupCount++;

            GameManager.Instance.leftForNextGroup = gameplayScriptable.levels[0].rounds[roundCount].groups[groupCount].count;
            EnemySpawns.Instance.InstantiateEnemys(gameplayScriptable.levels[0].rounds[roundCount].groups[groupCount]).Forget();

            for (int i = groupCount + 1; i < gameplayScriptable.levels[0].rounds[roundCount].groups.Length; i++)
            {
                if (gameplayScriptable.levels[0].rounds[roundCount].groups[i].chained)
                {
                    groupCount++;

                    GameManager.Instance.leftForNextGroup += gameplayScriptable.levels[0].rounds[roundCount].groups[i].count;
                    EnemySpawns.Instance.InstantiateEnemys(gameplayScriptable.levels[0].rounds[roundCount].groups[i]).Forget();
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