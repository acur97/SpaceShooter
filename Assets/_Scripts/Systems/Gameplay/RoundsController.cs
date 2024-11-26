using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class RoundsController : MonoBehaviour
{
    public static RoundsController Instance { get; private set; }

    [Header("Start UI")]
    [SerializeField] private TextMeshProUGUI modeText;
    [SerializeField] private Image modeTextLine;
    [SerializeField] private Color normalColor;
    [SerializeField] private Color infiniteColor;
    private Color bGColor;

    [Header("Leaderboard UI")]
    [SerializeField] private Toggle leaderboardInfiniteTgl;
    [SerializeField] private Image leaderboardBg;
    [SerializeField] private GameObject modesPanel;
    [SerializeField] private RectTransform tablePanel;

    public enum LevelType
    {
        Normal,
        Inifinite
    }
    [Header("Level")]
    public LevelType levelType;
    [SerializeField] private GameplayScriptable gameplayScriptable;

    [Space]
    [SerializeField, ReadOnly] private int roundCount = -1;
    [SerializeField, ReadOnly] private int groupCount = -1;

    private const string _MusicPitch = "MusicPitch";
    private const string _Normal = "Normal";
    private const string _Infinite = "Infinite";
    private const string lastLevelType = "LastLevelType";

    public void Init()
    {
        Instance = this;

        SetMode(Convert.ToBoolean(PlayerPrefs.GetInt(lastLevelType, 0)));

        GameManager.GameStart += DisableLeaderboardModes;
    }

    private void OnDisable()
    {
        GameManager.GameStart -= DisableLeaderboardModes;
    }

    private void DisableLeaderboardModes(bool on)
    {
        if (on)
        {
            modesPanel.SetActive(false);
            tablePanel.anchorMax = new Vector2(0.5f, 0.878f);
        }
    }

    public void ChangeMode()
    {
        if (levelType == LevelType.Normal)
        {
            SetMode(true);
        }
        else
        {
            SetMode(false);
        }
    }

    public void SetMode(bool infinite)
    {
        if (infinite)
        {
            bGColor = infiniteColor;
            modeTextLine.color = bGColor;
            modeText.color = bGColor;
            modeText.text = _Infinite;

            bGColor.a = 0.1f;
            leaderboardBg.color = bGColor;

            levelType = LevelType.Inifinite;
            PlayerPrefs.SetInt(lastLevelType, 1);
        }
        else
        {
            bGColor = normalColor;
            modeTextLine.color = bGColor;
            modeText.color = bGColor;
            modeText.text = _Normal;

            bGColor.a = 0.1f;
            leaderboardBg.color = bGColor;

            levelType = LevelType.Normal;
            PlayerPrefs.SetInt(lastLevelType, 0);
        }

        leaderboardInfiniteTgl.SetIsOnWithoutNotify(infinite);
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