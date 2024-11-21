using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Editor")]
    [SerializeField] private bool debugMode = true;

    [Space]
    public bool hasStarted = false;
    public bool isPlaying = false;
    private float currentTimeScale = 1f;
    public int leftForNextGroup = 0;

    [Header("Score")]
    [SerializeField] private TextMeshProUGUI scoreText;
    private const string preScore = "Score: ";
    public int score = 0;
    [SerializeField] private TextMeshProUGUI endScore;
    private const string postScore = "Final score:\n ";

    [Header("Coins")]
    [SerializeField] private TextMeshProUGUI coinsText;
    private const string preCoins = "Coins: ";
    [SerializeField] private TextMeshProUGUI endCoins;

    [Header("Ui")]
    [SerializeField] private RectTransform uiRect;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private TextMeshProUGUI wNewTxt;

    [Header("Screen Properties")]
    [SerializeField] private float innerLimit = -1.6f;
    private Vector2 innerLimits = Vector2.zero;
    public static Vector2 InnerLimits => Instance.innerLimits;

    [SerializeField] private float playerLimit = -0.4f;
    private Vector2 playerLimits = Vector2.zero;
    public static Vector2 PlayerLimits => Instance.playerLimits;

    [SerializeField] private float bulletLimit = 0.1f;
    private Vector2 bulletLimits = Vector2.zero;
    public static Vector2 BulletLimits => Instance.bulletLimits;

    [SerializeField] private float boundsLimit = 0.5f;
    private Vector2 boundsLimits = Vector2.zero;
    public static Vector2 BoundsLimits => Instance.boundsLimits;

    [SerializeField] private float enemyLine = 1.55f;
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

    private const string _Cancel = "Cancel";

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
        Gizmos.DrawLineList(new Vector3[8]
            {
            new(-(canvasBorders.x - playerLimit), canvasBorders.y - playerLimit),
            new(canvasBorders.x - playerLimit, canvasBorders.y - playerLimit),

            new(-(canvasBorders.x - playerLimit), -(canvasBorders.y - playerLimit)),
            new(canvasBorders.x - playerLimit, -(canvasBorders.y - playerLimit)),

            new(-(canvasBorders.x - playerLimit), -(canvasBorders.y - playerLimit)),
            new(-(canvasBorders.x - playerLimit), canvasBorders.y - playerLimit),

            new(canvasBorders.x - playerLimit, -(canvasBorders.y - playerLimit)),
            new(canvasBorders.x - playerLimit, canvasBorders.y - playerLimit)
            });


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

        hasStarted = true;
        isPlaying = true;
        Time.timeScale = 1;

        scoreText.SetText(preScore + score);
        coinsText.SetText(preCoins + PlayerProgress.GetCoins());

        UiManager.Instance.SetUi(UiType.Gameplay, true);
        UiManager.Instance.SetUi(UiType.Select, false, 1);

        RoundsController.Instance.StartRound();

        AudioManager.Instance.PlaySound(AudioManager.AudioType.Start, 2f);
    }

    public void OpenWnew(bool on)
    {
        UiManager.Instance.SetUi(UiType.Select, !on, 0.5f);
        UiManager.Instance.SetUi(UiType.Wnew, on, 0.5f);
    }

    public void GodMode(bool on)
    {
        PlayerController.Instance.SetHealth(on ? 10000000 : PlayerController.Instance._properties.health);
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

        PlayerProgress.Init(gameplayScriptable);
        SetCustoms(gameplayScriptable.selectedCustoms);

        wNewTxt.text = $"What's new    {Application.version}";
    }

    public void SetCustoms(ShipScriptable value)
    {
        gameplayScriptable.selectedCustoms = value;
        PlayerController.Instance._properties.color = gameplayScriptable.selectedCustoms.color; ;
        PlayerController.Instance.SetColor();
    }

    private void Update()
    {
        UpdateBorders();

        if (hasStarted)
        {
            if (Input.GetButtonDown(_Cancel))
            {
                Pause();
            }

            if (isPlaying)
            {
                if (leftForNextGroup <= 0)
                {
                    RoundsController.Instance.StartGroup();
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
        bulletLimits = new Vector2(-canvasBorders.x + bulletLimit, -canvasBorders.y + bulletLimit);
        boundsLimits = new Vector2(-canvasBorders.x + boundsLimit, -canvasBorders.y + boundsLimit);

        enemyLineLimit = canvasBorders.y - (enemyLine * canvasBorders.y);
        horizontalMultiplier = mainCamera.aspect * 0.5625f;
        horizontalInvertedMultiplier = horizontalMultiplier.Remap(1, 0, 0, 1) + 1;
    }

    public void UpScore(int value, bool enemy = true)
    {
        if (isPlaying)
        {
            if (enemy)
            {
                score += value;
                scoreText.SetText(preScore + score);
            }
            else if (PlayerController.Instance.health <= PlayerController.Instance._properties.health)
            {
                PlayerProgress.UpCoins(value);
                coinsText.SetText(preCoins + PlayerProgress.GetCoins());
                AudioManager.Instance.PlaySound(AudioManager.AudioType.Coin);
            }
        }
    }

    public void EndLevel()
    {
        if (isPlaying)
        {
            isPlaying = false;
            hasStarted = false;

            UiManager.Instance.SetUi(UiType.Pause, false);
            UiManager.Instance.SetUi(UiType.End, true, 1, () => UiManager.Instance.SetUi(UiType.Gameplay, false));

            endScore.SetText(postScore + score);
            endCoins.SetText(preCoins + PlayerProgress.GetCoins());
            AudioManager.Instance.PlaySound(AudioManager.AudioType.End, 2.5f);

            Time.timeScale = 0.5f;

            leftForNextGroup = -1;

            PlayerProgress.SaveAll();
        }
    }

    public void Pause()
    {
        if (isPlaying)
        {
            AudioManager.Instance.source.volume = 0.25f;

            UiManager.Instance.SetUi(UiType.Pause, true);

            currentTimeScale = Time.timeScale;
            Time.timeScale = 0;

            isPlaying = false;
        }
        else
        {
            AudioManager.Instance.source.volume = 1f;

            UiManager.Instance.SetUi(UiType.Pause, false);

            Time.timeScale = currentTimeScale;

            isPlaying = true;
        }
    }

    public void Retry()
    {
        PlayerProgress.SaveCustoms(true);

        UiManager.Instance.SetUi(UiType.Fade, true, 0.25f, () => SceneManager.LoadScene(0));
    }
}