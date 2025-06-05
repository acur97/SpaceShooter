using Cysharp.Text;
using Cysharp.Threading.Tasks;
#if UNITY_ANDROID
using Google.Play.AppUpdate;
using Google.Play.Common;
#endif
using System;
using TMPro;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public static Action GamePreStart;
    public static Action<bool> GameStart;

    [Space]
    [ReadOnly] public bool hasInitStarted = false;
    [ReadOnly] public bool hasStarted = false;
    [ReadOnly] public bool isPlaying = false;
    [ReadOnly] public bool hasEnded = false;
    private float currentTimeScale = 1f;
    [ReadOnly] public int leftForNextGroup = -1;
    private float prevTimeScale;
    private int prevLeftForNextGroup;
    private int adRevivals;
    private float timeDelayStartup = 0f;
    [ReadOnly] public float finalTimeOfGameplay = 0f;
    [ReadOnly] public bool isRevived = false;

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
    [SerializeField] private Camera mainCamera;
    [SerializeField] private TextMeshProUGUI wNewTxt;
    [SerializeField] private TextMeshProUGUI wNewParagraph;
    private const string wNewFormat = "What's new    {0}";
    [SerializeField] private GameObject adLifePanel;
    [SerializeField] private GameObject adIcon;
    [SerializeField] private GameObject adLifeEndPanel;
    [SerializeField] private TextMeshProUGUI adLifeTxt;
#if Platform_Web
    private const string adLifeFormat = "Need a Life? <color=#FFFFFF>Use <b>{0}</b> Coins!";
#else
    private const string adLifeFormat = "Need a Life? <color=#FFFFFF>Watch Ad!";
#endif
    [Header("Web QR Code")]
    [SerializeField] private GameObject qrCodePanel;
    [SerializeField] private GameObject qrCodeSprite;


    [Header("Gameplay")]
    [SerializeField] private Animator anim_count;

    [Header("Screen Borders")]
    [SerializeField] private Transform borders_top;
    [SerializeField] private Transform borders_bottom;
    [SerializeField] private Transform borders_left;
    [SerializeField] private Transform borders_right;

    private Vector4 innerLimits = Vector2.zero;
    /// <summary> x = top, y = bottom, z = left, w = right </summary>
    public static Vector4 InnerLimits => Instance.innerLimits;

    private Vector4 playerLimits = Vector2.zero;
    /// <summary> x = top, y = bottom, z = left, w = right </summary>
    public static Vector4 PlayerLimits => Instance.playerLimits;

    private float enemyLineLimit;
    /// <summary> float = top </summary>
    public static float EnemyLine => Instance.enemyLineLimit;

    private Vector4 bulletLimits = Vector2.zero;
    /// <summary> x = top, y = bottom, z = left, w = right </summary>
    public static Vector4 BulletLimits => Instance.bulletLimits;

    private Vector4 boundsLimits = Vector2.zero;
    /// <summary> x = top, y = bottom, z = left, w = right </summary>
    public static Vector4 BoundsLimits => Instance.boundsLimits;

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
    [SerializeField] private StoreManager storeManager;

    //[ContextMenu("Delete PlayerProgress")]
    //public void DeletePlayerProgress()
    //{
    //    PlayerPrefs.DeleteKey("PlayerProgress");
    //}

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        Instance = null;
        GamePreStart = null;
        GameStart = null;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!gameplayScriptable.debugEditorLimits)
        {
            return;
        }

        // Inner Limits
        Gizmos.color = Color.blue;
        Gizmos.DrawLineList(new Vector3[8]
            {
                // top
                new(borders_left.position.x * gameplayScriptable.innerLimit.w, borders_top.position.y * gameplayScriptable.innerLimit.x),
                new(borders_right.position.x * gameplayScriptable.innerLimit.z, borders_top.position.y * gameplayScriptable.innerLimit.x),

                // bottom
                new(borders_left.position.x * gameplayScriptable.innerLimit.w, borders_bottom.position.y * gameplayScriptable.innerLimit.y),
                new(borders_right.position.x * gameplayScriptable.innerLimit.z, borders_bottom.position.y * gameplayScriptable.innerLimit.y),

                // left
                new(borders_left.position.x * gameplayScriptable.innerLimit.w, borders_top.position.y * gameplayScriptable.innerLimit.x),
                new(borders_left.position.x * gameplayScriptable.innerLimit.w, borders_bottom.position.y * gameplayScriptable.innerLimit.y),

                // right
                new(borders_right.position.x * gameplayScriptable.innerLimit.z, borders_top.position.y * gameplayScriptable.innerLimit.x),
                new(borders_right.position.x * gameplayScriptable.innerLimit.z, borders_bottom.position.y * gameplayScriptable.innerLimit.y)
            });


        // Player Limits
        Gizmos.color = Color.cyan;
        Gizmos.DrawLineList(new Vector3[8]
            {
                // top
                new(borders_left.position.x * gameplayScriptable.playerLimit.w, borders_top.position.y * gameplayScriptable.playerLimit.x),
                new(borders_right.position.x * gameplayScriptable.playerLimit.z, borders_top.position.y * gameplayScriptable.playerLimit.x),

                // bottom
                new(borders_left.position.x * gameplayScriptable.playerLimit.w, borders_bottom.position.y * gameplayScriptable.playerLimit.y),
                new(borders_right.position.x * gameplayScriptable.playerLimit.z, borders_bottom.position.y * gameplayScriptable.playerLimit.y),

                // left
                new(borders_left.position.x * gameplayScriptable.playerLimit.w, borders_top.position.y * gameplayScriptable.playerLimit.x),
                new(borders_left.position.x * gameplayScriptable.playerLimit.w, borders_bottom.position.y * gameplayScriptable.playerLimit.y),

                // right
                new(borders_right.position.x * gameplayScriptable.playerLimit.z, borders_top.position.y * gameplayScriptable.playerLimit.x),
                new(borders_right.position.x * gameplayScriptable.playerLimit.z, borders_bottom.position.y * gameplayScriptable.playerLimit.y)
            });

        // Bullet Limits
        Gizmos.color = Color.red;
        Gizmos.DrawLineList(new Vector3[8]
            {
                // top
                new(borders_left.position.x * gameplayScriptable.bulletLimit.w, borders_top.position.y * gameplayScriptable.bulletLimit.x),
                new(borders_right.position.x * gameplayScriptable.bulletLimit.z, borders_top.position.y * gameplayScriptable.bulletLimit.x),

                // bottom
                new(borders_left.position.x * gameplayScriptable.bulletLimit.w, borders_bottom.position.y * gameplayScriptable.bulletLimit.y),
                new(borders_right.position.x * gameplayScriptable.bulletLimit.z, borders_bottom.position.y * gameplayScriptable.bulletLimit.y),

                // left
                new(borders_left.position.x * gameplayScriptable.bulletLimit.w, borders_top.position.y * gameplayScriptable.bulletLimit.x),
                new(borders_left.position.x * gameplayScriptable.bulletLimit.w, borders_bottom.position.y * gameplayScriptable.bulletLimit.y),

                // right
                new(borders_right.position.x * gameplayScriptable.bulletLimit.z, borders_top.position.y * gameplayScriptable.bulletLimit.x),
                new(borders_right.position.x * gameplayScriptable.bulletLimit.z, borders_bottom.position.y * gameplayScriptable.bulletLimit.y)
            });


        // Bounds Limits
        Gizmos.color = Color.blue + Color.red;
        Gizmos.DrawLineList(new Vector3[8]
            {
                // top
                new(borders_left.position.x * gameplayScriptable.boundsLimit.w, borders_top.position.y * gameplayScriptable.boundsLimit.x),
                new(borders_right.position.x * gameplayScriptable.boundsLimit.z, borders_top.position.y * gameplayScriptable.boundsLimit.x),

                // bottom
                new(borders_left.position.x * gameplayScriptable.boundsLimit.w, borders_bottom.position.y * gameplayScriptable.boundsLimit.y),
                new(borders_right.position.x * gameplayScriptable.boundsLimit.z, borders_bottom.position.y * gameplayScriptable.boundsLimit.y),

                // left
                new(borders_left.position.x * gameplayScriptable.boundsLimit.w, borders_top.position.y * gameplayScriptable.boundsLimit.x),
                new(borders_left.position.x * gameplayScriptable.boundsLimit.w, borders_bottom.position.y * gameplayScriptable.boundsLimit.y),

                // right
                new(borders_right.position.x * gameplayScriptable.boundsLimit.z, borders_top.position.y * gameplayScriptable.boundsLimit.x),
                new(borders_right.position.x * gameplayScriptable.boundsLimit.z, borders_bottom.position.y * gameplayScriptable.boundsLimit.y)
            });


        // Enemy Line
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new Vector2(borders_left.position.x * gameplayScriptable.playerLimit.w, borders_top.position.y * gameplayScriptable.enemyLine),
            new Vector2(borders_right.position.x * gameplayScriptable.playerLimit.z, borders_top.position.y * gameplayScriptable.enemyLine));
    }
#endif

    public void StartGameplayUi(bool forceStart = false)
    {
        if (hasStarted)
        {
            return;
        }

        hasInitStarted = true;

        if (!forceStart && gameplayScriptable.selectedPowerUp == null)
        {
            PopupManager.Instance.OpenPopUp(
                "Play without Power Up?",
                "Play",
                () => StartGameplayUi(true),
                "Go to Store",
                () =>
                {
                    storeManager.SetUi(true);
                    hasInitStarted = false;
                });
            return;
        }

        GamePreStart?.Invoke();

        switch (roundsController.levelType)
        {
            case RoundsController.LevelType.Normal:

                Time.timeScale = 1f;

                playerController.SetHealth((int)gameplayScriptable.playerHealth);
                playerController.SetHealthUi((int)gameplayScriptable.playerHealth);

                playerController.shoot.canShoot = true;
                break;

            case RoundsController.LevelType.Infinite:

                Time.timeScale = 1.1f;

                playerController.SetHealth((int)gameplayScriptable.playerHealthInfinite);
                playerController.SetHealthUi((int)gameplayScriptable.playerHealthInfinite);

                playerController.shoot.canShoot = false;
                break;
        }

        adRevivals = (int)gameplayScriptable.numberOfAdRevivals;
#if Platform_Mobile
        AdsManager.PrepareAd(AdsManager.AdType.Rewarded_Life);
#endif

        scoreText.SetTextFormat(preScore, score);
        coinsText.SetTextFormat(preCoins, PlayerProgress.GetCoins());

        uiManager.SetUi(UiType.Gameplay, true);
        uiManager.SetUi(UiType.Select, false, 1, () => anim_count.SetTrigger(AnimationParameters.Init));

        audioManager.PlaySound(Enums.AudioType.Start, 2f);

#if Platform_Mobile
        AdsManager.DestroyBottomBannerAd();
#endif

        AnalyticsManager.Log_LevelStart();
    }

    public void StartGame()
    {
        GameStart?.Invoke(true);

        playerController._collider.enabled = true;

        hasStarted = true;
        isPlaying = true;
        hasEnded = false;

        if (prevLeftForNextGroup != leftForNextGroup)
        {
            roundsController.StartRound();
        }

        timeDelayStartup = Time.realtimeSinceStartup - finalTimeOfGameplay;

        Vibration.InitVibrate();
    }

    public void OpenWnew(bool on)
    {
        uiManager.SetUi(UiType.Select, !on, 0.5f);
        uiManager.SetUi(UiType.Wnew, on, 0.5f);
    }

    //public void GodMode(bool on)
    //{
    //    playerController.SetHealth(on ? 10000000 : playerController._properties.health);
    //}

    public void OpenPlayStoreLink()
    {
        Application.OpenURL("https://play.google.com/store/apps/details?id=com.PolygonUs.SpaceShooter");
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
        playerController.Init(gameplayScriptable.selectedCustoms);
        AnalyticsManager.Init();

        Time.timeScale = 1;
        audioManager.SetMasterVolume(1f);
        audioManager.SetMusicPitch(1f);

        PlayerProgress.Init(gameplayScriptable);
        SetCustoms(gameplayScriptable.selectedCustoms);

        wNewTxt.SetTextFormat(wNewFormat, Application.version);
        wNewParagraph.text = gameplayScriptable.wNew;

        qrCodePanel.SetActive(false);

#if UNITY_WEBGL && !UNITY_EDITOR
        EnableMobileKeyboard(false);
#endif

#if UNITY_ANDROID && !UNITY_EDITOR
        CheckForUpdate().Forget();
#endif
    }

#if UNITY_WEBGL && !UNITY_EDITOR
    public void EnableMobileKeyboard(bool on)
    {
        WebGLInput.mobileKeyboardSupport = on;
    }
#endif

#if UNITY_ANDROID
    private async UniTaskVoid CheckForUpdate()
    {
        AppUpdateManager appUpdateManager = new();
        PlayAsyncOperation<AppUpdateInfo, AppUpdateErrorCode> appUpdateInfoOperation = appUpdateManager.GetAppUpdateInfo();

        await appUpdateInfoOperation;

        if (appUpdateInfoOperation.IsSuccessful)
        {
            AppUpdateInfo appUpdateInfoResult = appUpdateInfoOperation.GetResult();
            //Debug.Log("Days since update in Play Store " + appUpdateInfoResult.ClientVersionStalenessDays);

            if (appUpdateInfoResult.UpdateAvailability == UpdateAvailability.UpdateAvailable)
            {
                AppUpdateRequest startUpdateRequest = appUpdateManager.StartUpdate(
                  appUpdateInfoResult,
                  AppUpdateOptions.FlexibleAppUpdateOptions());

                while (!startUpdateRequest.IsDone)
                {
                    // For flexible flow, the user can continue to use the app while
                    // the update downloads in the background. You can implement a
                    // progress bar showing the download status during this time.
                    await UniTask.Yield();
                }

                PlayAsyncOperation<VoidResult, AppUpdateErrorCode> result = appUpdateManager.CompleteUpdate();
                await result; // update complete.

                // or update error:
                Debug.LogError("Update error: " + result.Error);
            }
        }
        else
        {
            Debug.LogError("Failed to get app update info: " + appUpdateInfoOperation.Error);
        }
    }
#endif

#if Platform_Mobile
    private void Start()
    {
        AdsManager.Init();
    }

    [ContextMenu("Destroy BannerAd")]
    public void DestroyBannerAd()
    {
        AdsManager.DestroyBottomBannerAd();
    }
#endif

    public void SetQR(bool isMobile)
    {
        qrCodePanel.SetActive(true);
        qrCodeSprite.SetActive(!isMobile);
    }

    public void SetCustoms(ShipScriptable value)
    {
        gameplayScriptable.selectedCustoms = value;
        playerController.SetColor(value);
    }

#if plamorm_Mobile
    private void OnApplicationFocus(bool focus)
    {
        if (hasStarted && !focus)
        {
            Pause();
        }
    }
#endif

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
                if (roundsController.levelType == RoundsController.LevelType.Infinite)
                {
                    accumulatedScore += gameplayScriptable.scoreScaleIncrease * Time.deltaTime * Time.timeScale;

                    if (accumulatedScore >= 1f)
                    {
                        pointsToAdd = Mathf.FloorToInt(accumulatedScore);
                        accumulatedScore -= pointsToAdd;

                        UpScore(pointsToAdd);
                    }
                }

                if (playerController.transform.position.x <= PlayerLimits.z + gameplayScriptable.borderLimit ||
                    playerController.transform.position.x >= PlayerLimits.w - gameplayScriptable.borderLimit)
                {
                    gameplayScriptable.countBorders += Time.deltaTime;

                    if (gameplayScriptable.countBorders >= gameplayScriptable.timeInBorder)
                    {
                        roundsController.InitBorderShip();
                        gameplayScriptable.countBorders = 0f;
                    }
                }

                // Debug
                //if (Input.GetKeyDown(KeyCode.P))
                //{
                //    leftForNextGroup = 0;
                //}
            }
        }
        else if (!hasInitStarted &&
            !PopupManager.Instance.isOpen &&
            !storeManager.gameObject.activeSelf &&
            Input.GetButtonDown(Inputs.Cancel))
        {
            PopupManager.Instance.OpenPopUp(
                "Wanto to leave?",
                "Exit",
                () => Application.Quit(),
                "Back");
        }
    }

    private void UpdateBorders()
    {
        innerLimits = new Vector4(
            borders_top.position.y * gameplayScriptable.innerLimit.x,
            borders_bottom.position.y * gameplayScriptable.innerLimit.y,
            borders_left.position.x * gameplayScriptable.innerLimit.w,
            borders_right.position.x * gameplayScriptable.innerLimit.z);

        playerLimits = new Vector4(
            borders_top.position.y * gameplayScriptable.playerLimit.x,
            borders_bottom.position.y * gameplayScriptable.playerLimit.y,
            borders_left.position.x * gameplayScriptable.playerLimit.w,
            borders_right.position.x * gameplayScriptable.playerLimit.z);

        bulletLimits = new Vector4(
            borders_top.position.y * gameplayScriptable.bulletLimit.x,
            borders_bottom.position.y * gameplayScriptable.bulletLimit.y,
            borders_left.position.x * gameplayScriptable.bulletLimit.w,
            borders_right.position.x * gameplayScriptable.bulletLimit.z);

        boundsLimits = new Vector4(
            borders_top.position.y * gameplayScriptable.boundsLimit.x,
            borders_bottom.position.y * gameplayScriptable.boundsLimit.y,
            borders_left.position.x * gameplayScriptable.boundsLimit.w,
            borders_right.position.x * gameplayScriptable.boundsLimit.z);

        enemyLineLimit = borders_top.position.y * gameplayScriptable.enemyLine;

        horizontalMultiplier = mainCamera.aspect * 0.5625f;
        horizontalInvertedMultiplier = horizontalMultiplier.Remap(1, 0, 0, 1) + 1;

        mainCamera.orthographicSize = 2.8125f * horizontalInvertedMultiplier;
    }

    public void UpScore(int value)
    {
        if (isPlaying)
        {
            score += value;
            scoreText.SetTextFormat(preScore, score);
        }
    }

    public void UpCoins(int value)
    {
        PlayerProgress.SetCoins(value);
        coinsText.SetTextFormat(preCoins, PlayerProgress.GetCoins());
        audioManager.PlaySound(Enums.AudioType.Coin);
    }

    public void EndLevel(bool hasRevival)
    {
        if (!isPlaying)
        {
            return;
        }

        isPlaying = false;
        hasEnded = true;

        GameStart?.Invoke(false);

        uiManager.SetUi(UiType.Pause, false);
        uiManager.SetUi(UiType.End, true, 1, () => uiManager.SetUi(UiType.Gameplay, false));

        endScore.SetTextFormat(postScore, score);
        endCoins.SetTextFormat(preCoins, PlayerProgress.GetCoins());

        audioManager.SetMasterVolume(0.5f);
        audioManager.PlaySound(Enums.AudioType.End, 2.5f);

        prevTimeScale = Time.timeScale;
        Time.timeScale = 0.5f;

        prevLeftForNextGroup = leftForNextGroup;
        leftForNextGroup = -1;

        finalTimeOfGameplay = Time.realtimeSinceStartup - timeDelayStartup;

#if Platform_Mobile
        if (hasRevival && adRevivals > 0 && AdsManager.RewardedLoaded)
        {
            ShowRevivalWithAd();
        }
        else if (hasRevival && adRevivals > 0 && PlayerProgress.GetCoins() >= gameplayScriptable.numberOfCoinsRevivals)
        {
            ShowRevivalWithCoins();
        }
#else
        if (hasRevival && adRevivals > 0 && PlayerProgress.GetCoins() >= gameplayScriptable.numberOfCoinsRevivals)
        {
            ShowRevivalWithCoins();
        }
#endif
        else
        {
            FinalizeGameplay();
        }

        powerUpsManager.ResetPowerUp();
        PlayerProgress.SaveAll();

        Vibration.InitVibrate();
    }

    private void ShowRevivalWithAd()
    {
        adLifePanel.SetActive(true);
        adLifeEndPanel.SetActive(false);

        adIcon.SetActive(true);
        adLifeTxt.SetText(adLifeFormat);
    }

    private void ShowRevivalWithCoins()
    {
        adLifePanel.SetActive(true);
        adLifeEndPanel.SetActive(false);

        adIcon.SetActive(false);
        adLifeTxt.SetTextFormat(adLifeFormat, gameplayScriptable.numberOfCoinsRevivals);
    }

    public void FinalizeGameplay()
    {
        adLifePanel.SetActive(false);
        adLifeEndPanel.SetActive(true);

        PostProcessingController.Instance.SetVolumeHealth(0f);

        AnalyticsManager.Log_LevelEnd();
    }

    public void WatchAdForLife()
    {
#if Platform_Mobile
        if (AdsManager.RewardedLoaded)
        {
            ShowAdForLife();
        }
        else
        {
            UseCoinsForLife();
        }
#else
        UseCoinsForLife();
#endif
    }

#if Platform_Mobile
    private void ShowAdForLife()
    {
        AdsManager.OnRewardedAdCompleted += OnAdViewed;
        AdsManager.ShowRewarded();
    }
#endif

    private void UseCoinsForLife()
    {
        UpCoins(-(int)gameplayScriptable.numberOfCoinsRevivals);
        OnAdViewed(true);
    }

#if Platform_Mobile
    private void OnDestroy()
    {
        AdsManager.OnRewardedAdCompleted -= OnAdViewed;
    }
#endif

    private void OnAdViewed(bool rewarded)
    {
#if Platform_Mobile
        AdsManager.OnRewardedAdCompleted -= OnAdViewed;
#endif
        if (rewarded)
        {
            adRevivals--;

            switch (roundsController.levelType)
            {
                case RoundsController.LevelType.Normal:
                    playerController.SetHealth((int)(gameplayScriptable.playerHealth * 0.5f));
                    break;

                case RoundsController.LevelType.Infinite:
                    playerController.SetHealth((int)(gameplayScriptable.playerHealthInfinite * 0.5f));
                    break;
            }

            playerController.UpdateHealthUi();
            playerController.gameObject.SetActive(true);
            playerController.ImmuneBlink().Forget();

            audioManager.SetMasterVolume(1f);

            uiManager.SetUi(UiType.End, false);
            uiManager.SetUi(UiType.Gameplay, true);

            Time.timeScale = prevTimeScale;
            leftForNextGroup = prevLeftForNextGroup;

            BulletsPool.Instance.ResetBullets();

            isRevived = true;

            anim_count.SetTrigger(AnimationParameters.Init);
        }
    }

    public void Pause()
    {
        if (hasEnded)
        {
            return;
        }

        if (isPlaying)
        {
            audioManager.SetMasterVolume(0.25f);

            uiManager.SetUi(UiType.Pause, true);

            currentTimeScale = Time.timeScale;
            Time.timeScale = 0;

            isPlaying = false;
        }
        else
        {
            audioManager.SetMasterVolume(1f);

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