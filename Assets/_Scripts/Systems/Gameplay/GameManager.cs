using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int leftForNextGroup = 0;

    [Header("Post Processing")]
    [SerializeField] private Volume volume;
    [SerializeField] private float volumeSpeed = 0.1f;
    private VolumeProfile profile;
    private ColorAdjustments colorAdjustments;
    private ChromaticAberration chromaticAberration;
    private int tween1;
    private int tween2;

    private void Awake()
    {
        Instance = this;

        profile = volume.profile;
        profile.TryGet(out colorAdjustments);
        profile.TryGet(out chromaticAberration);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            leftForNextGroup = 0;
        }

        if (leftForNextGroup == 0)
        {
            RoundsController.Instance.StartGroup();
        }
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
    }

    public void EndLevel()
    {
        Debug.LogWarning("Acabo el nivel");
        leftForNextGroup = -1;
    }
}