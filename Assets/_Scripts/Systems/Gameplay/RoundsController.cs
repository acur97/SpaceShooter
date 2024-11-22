using UnityEngine;
using Random = UnityEngine.Random;

public class RoundsController : MonoBehaviour
{
    public static RoundsController Instance { get; private set; }

    public enum LevelType
    {
        Normal,
        Inifinite
    }
    public LevelType levelType;
    [SerializeField] private GameplayScriptable gameplayScriptable;

    [Space]
    [SerializeField, ReadOnly] private int roundCount = -1;
    [SerializeField, ReadOnly] private int groupCount = -1;

    private const string _MusicPitch = "MusicPitch";

    public void Init()
    {
        Instance = this;
    }

    public void StartRound()
    {
        switch (levelType)
        {
            case LevelType.Normal:
                StartNormalRound();
                break;

            case LevelType.Inifinite:
                StartInfinite();
                break;
        }
    }

    private LevelScriptable[] CurrentLevels
    {
        get
        {
            return levelType switch
            {
                LevelType.Normal => gameplayScriptable.levels,
                LevelType.Inifinite => gameplayScriptable.infiniteLevels,
                _ => null,
            };
        }
    }

    private void StartNormalRound()
    {
        if (roundCount < CurrentLevels[0].rounds.Length - 1)
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
        if (groupCount < CurrentLevels[0].rounds[roundCount].groups.Length - 1)
        {
            groupCount++;

            GameManager.Instance.leftForNextGroup = CurrentLevels[0].rounds[roundCount].groups[groupCount].count;
            EnemySpawns.Instance.InstantiateEnemys(CurrentLevels[0].rounds[roundCount].groups[groupCount]).Forget();

            for (int i = groupCount + 1; i < CurrentLevels[0].rounds[roundCount].groups.Length; i++)
            {
                if (CurrentLevels[0].rounds[roundCount].groups[i].chained)
                {
                    groupCount++;

                    GameManager.Instance.leftForNextGroup += CurrentLevels[0].rounds[roundCount].groups[i].count;
                    EnemySpawns.Instance.InstantiateEnemys(CurrentLevels[0].rounds[roundCount].groups[i]).Forget();
                }
                else
                {
                    break;
                }
            }

            if (levelType == LevelType.Inifinite)
            {
                //Time.timeScale = Mathf.Min(Time.timeScale + gameplayScriptable.timeScaleIncrease, 2f);
                Time.timeScale += gameplayScriptable.timeScaleIncrease;

                AudioManager.Instance.mixer.GetFloat(_MusicPitch, out float pitch);
                AudioManager.Instance.mixer.SetFloat(_MusicPitch, Mathf.Min(pitch + 0.02f, 1.2f));
            }
        }
        else
        {
            StartRound();
        }
    }

    private void StartInfinite()
    {
        roundCount = Random.Range(-1, CurrentLevels[0].rounds.Length - 1);

        if (roundCount < CurrentLevels[0].rounds.Length - 1)
        {
            roundCount++;
            groupCount = -1;
        }
        else
        {

            roundCount--;
        }

        StartGroup();
    }
}