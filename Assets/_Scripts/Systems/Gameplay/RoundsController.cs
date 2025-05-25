using Cysharp.Text;
using Cysharp.Threading.Tasks;
using System;
using TMPro;
#if UNITY_EDITOR
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
#endif
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class RoundsController : MonoBehaviour
{
    public static RoundsController Instance { get; private set; }

    [Header("Start UI")]
    [SerializeField] private TextMeshProUGUI modeText;
    [SerializeField] private Color normalColor;
    [SerializeField] private Color infiniteColor;
    private Color bGColor;

    [Header("Leaderboard UI")]
    [SerializeField] private Toggle leaderboardInfiniteTgl;
    [SerializeField] private Image leaderboardBg;
    [SerializeField] private GameObject modesPanel;
    [SerializeField] private RectTransform tablePanel;

    [Header("Gameplay")]
    [SerializeField] private TextMeshProUGUI newLevelText;

    public enum LevelType
    {
        Normal,
        Infinite
    }
    [Header("Level")]
    public LevelType levelType;
    [SerializeField] private GameplayScriptable gameplayScriptable;

    [Space]
    [SerializeField, ReadOnly] public int levelCount = 0;
    [SerializeField, ReadOnly] public int roundCount = -1;
    [SerializeField, ReadOnly] public int groupCount = -1;
    private int prevLevelCount;
    private int prevRoundCount;
    private int prevGroupCount;
    private bool waiting = false;

    private const string _Normal = "Normal";
    private const string _Infinite = "Infinite";
    private const string lastLevelType = "LastLevelType";
    private const string levelComplete = "level #{0}\r\ncompleted";

#if UNITY_EDITOR
    public void OnPreprocessBuild(BuildReport report)
    {
        if (!ComprobarRondas())
        {
            throw new BuildFailedException("Error en los niveles.");
        }

        Debug.Log("Comprobacion de niveles correcta.");
    }

    private bool ComprobarRondas()
    {
        for (int i = 0; i < gameplayScriptable.levels.Length; i++)
        {
            if (gameplayScriptable.levels[i] == null)
            {
                Debug.LogError("Level " + i + " is empty");
                return false;
            }
            else
            {
                for (int j = 0; j < gameplayScriptable.levels[i].rounds.Length; j++)
                {
                    if (gameplayScriptable.levels[i].rounds[j] == null)
                    {
                        Debug.LogError("Round " + j + " in level " + i + " is empty");
                        return false;
                    }
                    else
                    {
                        for (int l = 0; l < gameplayScriptable.levels[i].rounds[j].groups.Length; l++)
                        {
                            if (gameplayScriptable.levels[i].rounds[j].groups[l].ship == null)
                            {
                                Debug.LogError("Ship " + l + " in round " + j + " in level " + i + " is empty");
                                return false;
                            }
                        }
                    }
                }
            }
        }

        for (int j = 0; j < gameplayScriptable.infiniteLevels[0].rounds.Length; j++)
        {
            if (gameplayScriptable.levels[0].rounds[j] == null)
            {
                Debug.LogError("Round " + j + " in infinite level " + 0 + " is empty");
                return false;
            }
            else
            {
                for (int l = 0; l < gameplayScriptable.levels[0].rounds[j].groups.Length; l++)
                {
                    if (gameplayScriptable.levels[0].rounds[j].groups[l].ship == null)
                    {
                        Debug.LogError("Ship " + l + " in round " + j + " in infinite level " + 0 + " is empty");
                        return false;
                    }
                }
            }
        }

        return true;
    }
#endif

    public void Init()
    {
        Random.InitState(DateTime.Now.Millisecond);

        Instance = this;

        SetMode(Convert.ToBoolean(PlayerPrefs.GetInt(lastLevelType, 0)));

        gameplayScriptable.bordersShip.countForGroup = false;
        newLevelText.transform.localScale = Vector3.zero;

        GameManager.GameStart += DisableLeaderboardModes;
    }

    private void OnDestroy()
    {
        GameManager.GameStart -= DisableLeaderboardModes;
    }

    private void DisableLeaderboardModes(bool start)
    {
        if (start)
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
            modeText.color = bGColor;
            modeText.text = _Infinite;

            bGColor.a = 0.1f;
            leaderboardBg.color = bGColor;

            levelType = LevelType.Infinite;
            PlayerPrefs.SetInt(lastLevelType, 1);
        }
        else
        {
            bGColor = normalColor;
            modeText.color = bGColor;
            modeText.text = _Normal;

            bGColor.a = 0.1f;
            leaderboardBg.color = bGColor;

            levelType = LevelType.Normal;
            PlayerPrefs.SetInt(lastLevelType, 0);
        }

        leaderboardInfiniteTgl.SetIsOnWithoutNotify(infinite);
    }

    private LevelScriptable[] CurrentLevels
    {
        get
        {
            return levelType switch
            {
                LevelType.Normal => gameplayScriptable.levels,
                LevelType.Infinite => gameplayScriptable.infiniteLevels,
                _ => null,
            };
        }
    }

    public void StartRound()
    {
        switch (levelType)
        {
            case LevelType.Normal:
                StartNormalRound();
                break;

            case LevelType.Infinite:
                StartInfinite();
                break;
        }
    }

    private void StartNormalRound()
    {
        if (roundCount < CurrentLevels[levelCount].rounds.Length - 1)
        {
            UpRound();

            StartGroup();
        }
        else
        {
            LevelComplete().Forget();
        }
    }

    private async UniTaskVoid LevelComplete()
    {
        levelCount++;

        if (levelCount < CurrentLevels.Length)
        {
            waiting = true;

            newLevelText.transform.localScale = Vector3.zero;
            newLevelText.SetTextFormat(levelComplete, levelCount);

            LeanTween.scale(newLevelText.gameObject, Vector3.one, 0.25f);
            LeanTween.value(0, 1, 0.3f).setOnUpdate((value) =>
            {
                newLevelText.fontMaterial.SetFloat(MaterialProperties.GlowPower, value);
            }).setEaseInExpo().setOnComplete(() =>
            {
                LeanTween.value(1, 0, 1).setOnUpdate((value) =>
                {
                    newLevelText.fontMaterial.SetFloat(MaterialProperties.GlowPower, value);
                });
            });

            await UniTask.WaitForSeconds(1.2f);

            LeanTween.scale(newLevelText.gameObject, Vector3.zero, 1.2f).setEaseInCubic();

            await UniTask.WaitForSeconds(1.2f);

            roundCount = -1;
            StartRound();

            waiting = false;
        }
        else
        {
            GameManager.Instance.EndLevel(false);
        }
    }

    private void StartInfinite()
    {
        roundCount = Random.Range(-1, CurrentLevels[levelCount].rounds.Length - 1);

        if (roundCount < CurrentLevels[levelCount].rounds.Length - 1)
        {
            UpRound();
        }
        else
        {
            roundCount--;
        }

        StartGroup();
    }

    private void UpRound()
    {
        roundCount++;
        groupCount = -1;
    }

    private void Update()
    {
        if (GameManager.Instance.hasStarted &&
            GameManager.Instance.isPlaying &&
            GameManager.Instance.leftForNextGroup <= 0 &&
            !waiting)
        {
            prevGroupCount = groupCount;
            prevRoundCount = roundCount;
            prevLevelCount = levelCount;

            StartGroup();

            if (!GameManager.Instance.hasEnded)
            {
                if (CurrentLevels[prevLevelCount].rounds[prevRoundCount].groups[prevGroupCount].randomPowerUp)
                {
                    PowerUpsManager.Instance.InstantiatePowerUp(gameplayScriptable.powerUps[Random.Range(0, gameplayScriptable.powerUps.Count)]);
                }
                else if (CurrentLevels[prevLevelCount].rounds[prevRoundCount].groups[prevGroupCount].spawnPowerUp != null)
                {
                    PowerUpsManager.Instance.InstantiatePowerUp(CurrentLevels[prevLevelCount].rounds[prevRoundCount].groups[prevGroupCount].spawnPowerUp);
                }
            }
        }
    }

    public void StartGroup()
    {
        if (CurrentLevels[levelCount].rounds[roundCount] == null)
        {
            StartRound();
            return;
        }
        if (groupCount < CurrentLevels[levelCount].rounds[roundCount].groups.Length - 1)
        {
            groupCount++;

            GameManager.Instance.leftForNextGroup = CurrentLevels[levelCount].rounds[roundCount].groups[groupCount].count;
            EnemySpawns.Instance.InstantiateEnemys(CurrentLevels[levelCount].rounds[roundCount].groups[groupCount]).Forget();

            for (int i = groupCount + 1; i < CurrentLevels[levelCount].rounds[roundCount].groups.Length; i++)
            {
                if (CurrentLevels[levelCount].rounds[roundCount].groups[i].chained)
                {
                    groupCount++;

                    GameManager.Instance.leftForNextGroup += CurrentLevels[levelCount].rounds[roundCount].groups[i].count;
                    EnemySpawns.Instance.InstantiateEnemys(CurrentLevels[levelCount].rounds[roundCount].groups[i]).Forget();
                }
                else
                {
                    break;
                }
            }

            if (levelType == LevelType.Infinite)
            {
                Time.timeScale += gameplayScriptable.timeScaleIncrease;

                AudioManager.Instance.MusicPitch = Mathf.Min(AudioManager.Instance.MusicPitch + 0.02f, 1.2f);
            }
        }
        else
        {
            StartRound();
        }
    }

    public void InitBorderShip()
    {
        EnemySpawns.Instance.InitEnemy(gameplayScriptable.bordersShip, Enums.SpawnType.Specific, 0, 0, PlayerController.Instance.transform.position.x);
    }
}