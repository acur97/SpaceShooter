using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public bool hasStarted = false;
    public bool isPlaying = false;
    public int leftForNextGroup = 0;

    [Header("Post Processing")]
    [SerializeField] private Volume volume;
    [SerializeField] private float volumeSpeed = 0.1f;
    private VolumeProfile profile;
    private ColorAdjustments colorAdjustments;
    private ChromaticAberration chromaticAberration;
    private int tweenPostExposure1;
    private int tweenPostExposure2;
    private int tweenChromaticAberration1;
    private int tweenChromaticAberration2;

    [Header("Score")]
    public int scoreCoin = 5;
    public int scoreEnemy = 8;

    [Space]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI endScore;
    private int score = 0;
    private const string preScore = "Score: ";
    private const string postcore = "Final score:\n ";

    [Header("UI")]
    [SerializeField] private GameObject canvasSelect;
    [SerializeField] private GameObject canvasGameplay;
    [SerializeField] private GameObject canvasPause;
    [SerializeField] private GameObject canvasEnd;

    [Space]
    [SerializeField] private GameObject shadowBorders;

    [Header("Screen Properties")]
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

    [Space]
    [SerializeField] private RectTransform uiRect;
    [SerializeField] private Camera mainCamera;

    [Space]
    [SerializeField, ColorUsage(true, true)] private Color[] colors;

    [Header("Sounds")]
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip clipBoom;
    [SerializeField] private AudioClip clipCoin;
    [SerializeField] private AudioClip clipStart;
    [SerializeField] private AudioClip clipEnd;
    public AudioClip clipZap;

    private const string _Cancel = "Cancel";

    private void OnDrawGizmosSelected()
    {
        RectTransformUtility.ScreenPointToWorldPointInRectangle(uiRect, uiRect.position, mainCamera, out Vector3 canvasBorders);

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

    private void Awake()
    {
        Instance = this;

        profile = volume.profile;
        profile.TryGet(out colorAdjustments);
        profile.TryGet(out chromaticAberration);

        scoreText.SetText(preScore + score);

        canvasSelect.SetActive(true);
        canvasGameplay.SetActive(false);
        canvasPause.SetActive(false);
        canvasEnd.SetActive(false);

        Time.timeScale = 1;
    }

    public void GodMode(bool on)
    {
        PlayerController.Instance.SetHealth(on ? 1000000 : 1);
    }

    public void StartGame()
    {
        hasStarted = true;
        isPlaying = true;
        Time.timeScale = 1;
        canvasGameplay.SetActive(true);

        LeanTween.alphaCanvas(canvasSelect.GetComponent<CanvasGroup>(), 0, 1).setOnComplete(() =>
        {
            canvasSelect.SetActive(false);
        }).setIgnoreTimeScale(true);

        RoundsController.Instance.StartRound();

        PlaySound(clipStart, 2f);
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
                if (leftForNextGroup == 0)
                {
                    RoundsController.Instance.StartGroup();
                }

                //DebugMode
                if (Input.GetKeyDown(KeyCode.P))
                {
                    leftForNextGroup = 0;
                }
            }
        }
    }

    private void UpdateBorders()
    {
        RectTransformUtility.ScreenPointToWorldPointInRectangle(uiRect, uiRect.position, mainCamera, out Vector3 canvasBorders);

        playerLimits = new Vector2(-canvasBorders.x + playerLimit, -canvasBorders.y + playerLimit);
        bulletLimits = new Vector2(-canvasBorders.x + bulletLimit, -canvasBorders.y + bulletLimit);
        boundsLimits = new Vector2(-canvasBorders.x + boundsLimit, -canvasBorders.y + boundsLimit);
        enemyLineLimit = canvasBorders.y - (enemyLine * canvasBorders.y);
        horizontalMultiplier = mainCamera.pixelWidth / 1280f;
        horizontalInvertedMultiplier = 1280f / mainCamera.pixelWidth;

        // black gradient borders to fix wider aspect ratios
        shadowBorders.SetActive((mainCamera.pixelWidth / (float)mainCamera.pixelHeight) > 1.94f);
    }

    public void PlaySound(AudioClip clip, float volume = 1f)
    {
        source.PlayOneShot(clip, volume);
    }

    public void VolumePunch()
    {
        LeanTween.cancel(tweenPostExposure1);
        LeanTween.cancel(tweenPostExposure2);
        tweenPostExposure1 = LeanTween.value(colorAdjustments.postExposure.value, 50, volumeSpeed).setOnUpdate((float value) =>
        {
            colorAdjustments.saturation.value = value;
        }).setOnComplete(() =>
        {
            tweenPostExposure2 = LeanTween.value(colorAdjustments.postExposure.value, 25, volumeSpeed).setOnUpdate((float value) =>
            {
                colorAdjustments.saturation.value = value;
            }).id;
        }).id;

        LeanTween.cancel(tweenChromaticAberration1);
        LeanTween.cancel(tweenChromaticAberration2);
        tweenChromaticAberration1 = LeanTween.value(chromaticAberration.intensity.value, 0.1f, volumeSpeed).setOnUpdate((float value) =>
        {
            chromaticAberration.intensity.value = value;
        }).setOnComplete(() =>
        {
            tweenChromaticAberration2 = LeanTween.value(chromaticAberration.intensity.value, 0.05f, volumeSpeed).setOnUpdate((float value) =>
            {
                chromaticAberration.intensity.value = value;
            }).id;
        }).id;

        PlaySound(clipBoom, 0.25f);
    }

    public void UpScore(int value)
    {
        if (isPlaying)
        {
            score += value;
            scoreText.SetText(preScore + score);

            if (value == scoreCoin)
            {
                PlaySound(clipCoin);
            }
        }
    }

    public void EndLevel()
    {
        if (isPlaying)
        {
            isPlaying = false;
            hasStarted = false;
            canvasGameplay.SetActive(false);
            canvasEnd.SetActive(true);

            endScore.SetText(postcore + score);
            PlaySound(clipEnd, 2.5f);

            Time.timeScale = 0.5f;

            leftForNextGroup = -1;
        }
    }

    public void Pause()
    {
        if (!canvasPause.activeSelf)
        {
            canvasPause.SetActive(true);
            Time.timeScale = 0;
            isPlaying = false;
        }
        else
        {
            canvasPause.SetActive(false);
            Time.timeScale = 1;
            isPlaying = true;
        }
    }

    public void SelectColor(int value)
    {
        PlayerController.Instance._properties.color = colors[value];
        PlayerController.Instance.SetColor();
    }

    public void Retry()
    {
        SceneManager.LoadScene(0);
    }

    public void Exit()
    {
        Application.Quit();
    }
}