using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public bool isPlaying = false;
    public int leftForNextGroup = 0;

    [Header("Post Processing")]
    [SerializeField] private Volume volume;
    [SerializeField] private float volumeSpeed = 0.1f;
    private VolumeProfile profile;
    private ColorAdjustments colorAdjustments;
    private ChromaticAberration chromaticAberration;
    private int tween1;
    private int tween2;

    [Header("Score")]
    public int scoreCoin = 5;
    public int scoreEnemy = 8;

    [Space]
    [SerializeField] private TextMeshProUGUI scoreText;
    private const string preScore = "Score: ";
    private int score = 0;
    [SerializeField] private TextMeshProUGUI endScore;
    private const string postcore = "Final score:\n ";

    [Header("UI")]
    [SerializeField] private GameObject canvasSelect;
    [SerializeField] private GameObject canvasGameplay;
    [SerializeField] private GameObject canvasPause;
    [SerializeField] private GameObject canvasEnd;

    [Space]
    [SerializeField, ColorUsage(true, true)] private Color[] colors;

    [Header("Sounds")]
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip clipBoom;
    [SerializeField] private AudioClip clipCoin;
    [SerializeField] private AudioClip clipStart;
    [SerializeField] private AudioClip clipEnd;

    private const string _Cancel = "Cancel";

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

        //Time.timeScale = 0;
    }

    public void GodMode(bool on)
    {
        PlayerController.Instance.SetHealth(on ? 1000 : 1);
    }

    public void StartGame()
    {
        isPlaying = true;
        Time.timeScale = 1;
        canvasGameplay.SetActive(true);

        LeanTween.alphaCanvas(canvasSelect.GetComponent<CanvasGroup>(), 0, 1).setOnComplete(() =>
        {
            canvasSelect.SetActive(false);
        });

        RoundsController.Instance.StartRound();

        PlaySound(clipStart);
    }

    private void Update()
    {
        if (isPlaying)
        {
            if (Input.GetButtonDown(_Cancel))
            {
                Pause();
            }

            if (Input.GetKeyDown(KeyCode.P))
            {
                leftForNextGroup = 0;
            }

            if (leftForNextGroup == 0)
            {
                RoundsController.Instance.StartGroup();
            }
        }
    }

    public void PlaySound(AudioClip clip)
    {
        source.PlayOneShot(clip);
    }

    public void VolumePunch()
    {
        LeanTween.cancel(tween1);
        tween1 = LeanTween.value(colorAdjustments.postExposure.value, 50, volumeSpeed).setOnUpdate((float value) =>
        {
            colorAdjustments.saturation.value = value;
        }).setOnComplete(() =>
        {
            LeanTween.value(colorAdjustments.postExposure.value, 25, volumeSpeed).setOnUpdate((float value) =>
            {
                colorAdjustments.saturation.value = value;
            });
        }).id;

        LeanTween.cancel(tween2);
        tween2 = LeanTween.value(chromaticAberration.intensity.value, 0.1f, volumeSpeed).setOnUpdate((float value) =>
        {
            chromaticAberration.intensity.value = value;
        }).setOnComplete(() =>
        {
            LeanTween.value(chromaticAberration.intensity.value, 0.05f, volumeSpeed).setOnUpdate((float value) =>
            {
                chromaticAberration.intensity.value = value;
            });
        }).id;

        PlaySound(clipBoom);
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

    public void SelectColor(int value)
    {
        PlayerController.Instance._properties.color = colors[value];
        PlayerController.Instance.SetColor();
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

    public void Retry()
    {
        SceneManager.LoadScene(0);
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void EndLevel()
    {
        if (isPlaying)
        {
            isPlaying = false;
            canvasGameplay.SetActive(false);
            canvasEnd.SetActive(true);

            endScore.SetText(postcore + score);
            PlaySound(clipEnd);

            Time.timeScale = 0.5f;

            leftForNextGroup = -1;
        }
    }
}