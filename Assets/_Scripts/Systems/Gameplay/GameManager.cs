using Cysharp.Threading.Tasks;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public static Action<bool> GameStart;

    [Header("Editor")]
    [SerializeField] private bool debugMode = true;

    [Space]
    [ReadOnly] public bool hasStarted = false;
    [ReadOnly] public bool isPlaying = false;
    [ReadOnly] public bool hasEnded = false;
    private float currentTimeScale = 1f;
    [ReadOnly] public int leftForNextGroup = 0;

    [Header("Score")]
    [SerializeField] private TextMeshProUGUI scoreText;
    private const string preScore = "Score: {0}";
    public int score = 0;
    private float accumulatedScore = 0f;
    private int pointsToAdd = 0;
    [SerializeField] private TextMeshProUGUI endScore;
    private const string postScore = "Final score:\n {0}";

    [Header("Coins")]
    [SerializeField] private TextMeshProUGUI coinsText;
    private const string preCoins = "Coins: {0}";
    [SerializeField] private TextMeshProUGUI endCoins;

    [Header("Ui")]
    [SerializeField] private RectTransform uiRect;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private TextMeshProUGUI wNewTxt;

    [Header("Screen Properties Percentage")]
    [SerializeField, ColorField(0, 0, 1)] private float innerLimit = -1.6f;
    private Vector2 innerLimits = Vector2.zero;
    public static Vector2 InnerLimits => Instance.innerLimits;

    [SerializeField, ColorField(0, 1, 1)] private float playerLimit = -0.5f;
    private Vector2 playerLimits = Vector2.zero;
    public static Vector2 PlayerLimits => Instance.playerLimits;
    [SerializeField, ColorField(0, 1, 0)] private float playerLimitTop = -1f;
    private float playerLimitTops = 0f;
    public static float PlayerLimitTop => Instance.playerLimitTops;

    [SerializeField, ColorField(1, 0, 0)] private float bulletLimit = 0.05f;
    private Vector2 bulletLimits = Vector2.zero;
    public static Vector2 BulletLimits => Instance.bulletLimits;

    [SerializeField, ColorField(1, 0, 1)] private float boundsLimit = 0.5f;
    private Vector2 boundsLimits = Vector2.zero;
    public static Vector2 BoundsLimits => Instance.boundsLimits;

    [SerializeField, ColorField(1, 0.92f, 0.016f)] private float enemyLine = 1.55f;
    private float enemyLineLimit;
    public static float EnemyLine => Instance.enemyLineLimit;

    private float horizontalMultiplier = 1f;
    public static float HorizontalMultiplier => Instance.horizontalMultiplier;
    private float horizontalInvertedMultiplier = 1f;
    public static float HorizontalInvertedMultiplier => Instance.horizontalInvertedMultiplier;

    [Header("Managers")]
    public GameplayScriptable gameplayScriptable;
    [SerializeField] private UiManager uiManager;
    [SerializeField] private RoundsController roundsController;
    [SerializeField] private ControlsManager controlsManager;
    [SerializeField] private PostProcessingController postProcessingController;
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private PowerUpsManager powerUpsManager;
    [SerializeField] private PlayerController playerController;

    [ContextMenu("Delete PlayerProgress")]
    public void DeletePlayerProgress()
    {
        PlayerPrefs.DeleteKey("PlayerProgress");
    }

    private void OnDrawGizmosSelected()
    {
        if (!debugMode)
        {
            return;
        }

        RectTransformUtility.ScreenPointToWorldPointInRectangle(uiRect, uiRect.position, mainCamera, out Vector3 canvasBorders);

        // Inner Limits
        Gizmos.color = Color.blue;
        Gizmos.DrawLineList(new Vector3[8]
            {
            new(-(canvasBorders.x - innerLimit), canvasBorders.y - innerLimit),
            new(canvasBorders.x - innerLimit, canvasBorders.y - innerLimit),

            new(-(canvasBorders.x - innerLimit), -(canvasBorders.y - innerLimit)),
            new(canvasBorders.x - innerLimit, -(canvasBorders.y - innerLimit)),

            new(-(canvasBorders.x - innerLimit), -(canvasBorders.y - innerLimit)),
            new(-(canvasBorders.x - innerLimit), canvasBorders.y - innerLimit),

            new(canvasBorders.x - innerLimit, -(canvasBorders.y - innerLimit)),
            new(canvasBorders.x - innerLimit, canvasBorders.y - innerLimit)
            });


        // Player Limits
        Gizmos.color = Color.cyan;
        Gizmos.DrawLineList(new Vector3[6]
            {
            new(-(canvasBorders.x - playerLimit), canvasBorders.y - playerLimit),
            new(canvasBorders.x - playerLimit, canvasBorders.y - playerLimit),

            new(-(canvasBorders.x - playerLimit), -(canvasBorders.y - playerLimitTop)),
            new(-(canvasBorders.x - playerLimit), canvasBorders.y - playerLimit),

            new(canvasBorders.x - playerLimit, -(canvasBorders.y - playerLimitTop)),
            new(canvasBorders.x - playerLimit, canvasBorders.y - playerLimit)
            });
        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector2(-(canvasBorders.x - playerLimit), -(canvasBorders.y - playerLimitTop)), new Vector2(canvasBorders.x - playerLimit, -(canvasBorders.y - playerLimitTop)));


        // Bullet Limits
        Gizmos.color = Color.red;
        Gizmos.DrawLineList(new Vector3[8]
            {
            new(-(canvasBorders.x - bulletLimit), canvasBorders.y - bulletLimit),
            new(canvasBorders.x - bulletLimit, canvasBorders.y - bulletLimit),

            new(-(canvasBorders.x - bulletLimit), -(canvasBorders.y - bulletLimit)),
            new(canvasBorders.x - bulletLimit, -(canvasBorders.y - bulletLimit)),

            new(-(canvasBorders.x - bulletLimit), -(canvasBorders.y - bulletLimit)),
            new(-(canvasBorders.x - bulletLimit), canvasBorders.y - bulletLimit),

            new(canvasBorders.x - bulletLimit, -(canvasBorders.y - bulletLimit)),
            new(canvasBorders.x - bulletLimit, canvasBorders.y - bulletLimit)
            });


        // Bounds Limits
        Gizmos.color = Color.blue + Color.red;
        Gizmos.DrawLineList(new Vector3[8]
            {
            new(-(canvasBorders.x - boundsLimit), canvasBorders.y - boundsLimit),
            new(canvasBorders.x - boundsLimit, canvasBorders.y - boundsLimit),

            new(-(canvasBorders.x - boundsLimit), -(canvasBorders.y - boundsLimit)),
            new(canvasBorders.x - boundsLimit, -(canvasBorders.y - boundsLimit)),

            new(-(canvasBorders.x - boundsLimit), -(canvasBorders.y - boundsLimit)),
            new(-(canvasBorders.x - boundsLimit), canvasBorders.y - boundsLimit),

            new(canvasBorders.x - boundsLimit, -(canvasBorders.y - boundsLimit)),
            new(canvasBorders.x - boundsLimit, canvasBorders.y - boundsLimit)
            });


        // Enemy Line
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new Vector2(canvasBorders.x, canvasBorders.y - (enemyLine * canvasBorders.y)), new Vector2(-canvasBorders.x, canvasBorders.y - (enemyLine * canvasBorders.y)));
    }

    public void StartGame()
    {
        if (hasStarted)
        {
            return;
        }

        GameStart?.Invoke(true);

        hasStarted = true;
        isPlaying = true;

        switch (roundsController.levelType)
        {
            case RoundsController.LevelType.Normal:

                Time.timeScale = 1f;

                playerController.SetHealth((int)gameplayScriptable.playerHealth);
                playerController.SetHealthUi((int)gameplayScriptable.playerHealth);

                playerController.shoot.canShoot = true;
                break;

            case RoundsController.LevelType.Inifinite:

                Time.timeScale = 1.1f;

                playerController.SetHealth((int)gameplayScriptable.playerHealthInfinite);
                playerController.SetHealthUi((int)gameplayScriptable.playerHealthInfinite);

                playerController.shoot.canShoot = false;
                break;
        }

        scoreText.SetText(preScore, score);
        coinsText.SetText(preCoins, PlayerProgress.GetCoins());

        uiManager.SetUi(UiType.Gameplay, true);
        uiManager.SetUi(UiType.Select, false, 1);

        roundsController.StartRound();

        audioManager.PlaySound(Enums.AudioType.Start, 2f);
    }

    public void OpenWnew(bool on)
    {
        uiManager.SetUi(UiType.Select, !on, 0.5f);
        uiManager.SetUi(UiType.Wnew, on, 0.5f);
    }

    public void GodMode(bool on)
    {
        playerController.SetHealth(on ? 10000000 : playerController._properties.health);
    }

    private void Awake()
    {
        PlayerLoopSystem loop = PlayerLoop.GetCurrentPlayerLoop();
        PlayerLoopHelper.Initialize(ref loop, InjectPlayerLoopTimings.Minimum);
        //PlayerLoopHelper.DumpCurrentPlayerLoop();

        Instance = this;

        uiManager.Init();
        roundsController.Init();
        //controlsManager.Init();
        audioManager.Init();
        postProcessingController.Init();
        powerUpsManager.Init();
        playerController.Init();

        Time.timeScale = 1;
        audioManager.mixer.SetFloat(MixerParameters.MusicPitch, 1f);
        audioManager.mixer.SetFloat(MixerParameters.MasterVolume, 0f);

        PlayerProgress.Init(gameplayScriptable);
        SetCustoms(gameplayScriptable.selectedCustoms);

        wNewTxt.text = $"What's new    {Application.version}";
    }

    public void SetCustoms(ShipScriptable value)
    {
        gameplayScriptable.selectedCustoms = value;
        playerController._properties.color = gameplayScriptable.selectedCustoms.color; ;
        playerController.SetColor();
    }

    private void Update()
    {
        UpdateBorders();

        if (hasStarted)
        {
            if (Input.GetButtonDown(Inputs.Pause))
            {
                Pause();
            }

            if (isPlaying)
            {
                if (leftForNextGroup <= 0)
                {
                    roundsController.StartGroup();
                }

                if (roundsController.levelType == RoundsController.LevelType.Inifinite)
                {
                    accumulatedScore += gameplayScriptable.scoreScaleIncrease * Time.deltaTime * Time.timeScale;

                    if (accumulatedScore >= 1f)
                    {
                        pointsToAdd = Mathf.FloorToInt(accumulatedScore);
                        accumulatedScore -= pointsToAdd;

                        UpScore(pointsToAdd);
                    }
                }

                //DebugMode
                //if (Input.GetKeyDown(KeyCode.P))
                //{
                //    leftForNextGroup = 0;
                //}
            }
        }
    }

    private void UpdateBorders()
    {
        RectTransformUtility.ScreenPointToWorldPointInRectangle(uiRect, uiRect.position, mainCamera, out Vector3 canvasBorders);

        innerLimits = new Vector2(-canvasBorders.x + innerLimit, -canvasBorders.y + innerLimit);
        playerLimits = new Vector2(-canvasBorders.x + playerLimit, -canvasBorders.y + playerLimit);
        playerLimitTops = -canvasBorders.y + playerLimitTop;
        bulletLimits = new Vector2(-canvasBorders.x + bulletLimit, -canvasBorders.y + bulletLimit);
        boundsLimits = new Vector2(-canvasBorders.x + boundsLimit, -canvasBorders.y + boundsLimit);

        enemyLineLimit = canvasBorders.y - (enemyLine * canvasBorders.y);
        horizontalMultiplier = mainCamera.aspect * 0.5625f;
        horizontalInvertedMultiplier = horizontalMultiplier.Remap(1, 0, 0, 1) + 1;
    }

    public void UpScore(int value)
    {
        if (isPlaying)
        {
            score += value;
            scoreText.SetText(preScore, score);
        }
    }

    public void UpCoins(int value)
    {
        PlayerProgress.UpCoins(value);
        coinsText.SetText(preCoins, PlayerProgress.GetCoins());
        audioManager.PlaySound(Enums.AudioType.Coin);
    }

    public void EndLevel()
    {
        if (isPlaying)
        {
            hasEnded = true;
            isPlaying = false;

            GameStart?.Invoke(false);

            uiManager.SetUi(UiType.Pause, false);
            uiManager.SetUi(UiType.End, true, 1, () => uiManager.SetUi(UiType.Gameplay, false));

            endScore.SetText(postScore, score);
            endCoins.SetText(preCoins, PlayerProgress.GetCoins());

            audioManager.mixer.SetFloat(MixerParameters.MasterVolume, -1f);
            audioManager.PlaySound(Enums.AudioType.End, 2.5f);

            Time.timeScale = 0.5f;

            leftForNextGroup = -1;

            PlayerProgress.SaveAll();

#if !UNITY_EDITOR && UNITY_WEBGL
            Vibrate(gameplayScriptable.vibrationDeath);
#else
            Handheld.Vibrate();
#endif
        }
    }

#if !UNITY_EDITOR && UNITY_WEBGL
    [DllImport("__Internal")]
    private static extern void Vibrate(int ms);
#endif

    public void Pause()
    {
        if (hasEnded)
        {
            return;
        }

        if (isPlaying)
        {
            audioManager.mixer.SetFloat(MixerParameters.MasterVolume, -15f);

            uiManager.SetUi(UiType.Pause, true);

            currentTimeScale = Time.timeScale;
            Time.timeScale = 0;

            isPlaying = false;
        }
        else
        {
            audioManager.mixer.SetFloat(MixerParameters.MasterVolume, 0f);

            uiManager.SetUi(UiType.Pause, false);

            Time.timeScale = currentTimeScale;

            isPlaying = true;
        }
    }

    public void Retry()
    {
        PlayerProgress.SaveCustoms(true);

        uiManager.SetUi(UiType.Fade, true, 0.25f, () => SceneManager.LoadScene(0));
    }
}