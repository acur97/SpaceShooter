using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessingController : MonoBehaviour
{
    public static PostProcessingController Instance;

    [SerializeField] private Volume volume;

    [Space]
    [SerializeField] private float volumeSpeed = 0.1f;
    [SerializeField] private Vector2 exposureRange;
    [SerializeField] private Vector2 chromaticAberrationRange;
    public float maxVignette = 0.25f;

    private VolumeProfile profile;
    private ColorAdjustments colorAdjustments;
    private ChromaticAberration chromaticAberration;
    private Vignette vignette;
    private int tweenPostExposure1;
    private int tweenPostExposure2;
    private int tweenChromaticAberration1;
    private int tweenChromaticAberration2;

    [Space]
    [SerializeField] private Transform cam;
    [SerializeField] private AnimationCurve shakeCurve;
    private float shakeDuration = 0f;
    private Vector3 cameraOrigin = new(0f, 0f, -1f);
    private Vector3 shake;

    public void Init()
    {
        Instance = this;

        ResetCameraPosition();

        profile = volume.profile;
        profile.TryGet(out colorAdjustments);
        profile.TryGet(out chromaticAberration);
        profile.TryGet(out vignette);
    }

    private void ResetCameraPosition()
    {
        if (cam == null)
        {
            return;
        }

        cam.localPosition = cameraOrigin;
    }

    public void VolumePunch()
    {
        LeanTween.cancel(tweenPostExposure1);
        LeanTween.cancel(tweenPostExposure2);
        tweenPostExposure1 = LeanTween.value(colorAdjustments.postExposure.value, exposureRange.y, volumeSpeed).setOnUpdate((float value) =>
        {
            colorAdjustments.saturation.value = value;
        }).setOnComplete(() =>
        {
            tweenPostExposure2 = LeanTween.value(colorAdjustments.postExposure.value, exposureRange.x, volumeSpeed).setOnUpdate((float value) =>
            {
                colorAdjustments.saturation.value = value;
            }).id;
        }).id;

        LeanTween.cancel(tweenChromaticAberration1);
        LeanTween.cancel(tweenChromaticAberration2);
        tweenChromaticAberration1 = LeanTween.value(chromaticAberration.intensity.value, chromaticAberrationRange.y, volumeSpeed).setOnUpdate((float value) =>
        {
            chromaticAberration.intensity.value = value;
        }).setOnComplete(() =>
        {
            tweenChromaticAberration2 = LeanTween.value(chromaticAberration.intensity.value, chromaticAberrationRange.x, volumeSpeed).setOnUpdate((float value) =>
            {
                chromaticAberration.intensity.value = value;
            }).id;
        }).id;

        AudioManager.Instance.PlaySound(AudioManager.AudioType.Boom, 0.25f);
    }

    public void SetVolumeHealth(float val)
    {
        vignette.intensity.value = val;
    }

    public async UniTaskVoid ScreenShake(float duration = 1f, float shakeAmount = 0.03f)
    {
        shakeDuration = duration;

        while (shakeDuration > 0)
        {
            shake = cameraOrigin + shakeAmount * shakeCurve.Evaluate(shakeDuration.Remap(0, duration, 1, 0)) * Random.insideUnitSphere;
            shake.z = -1f;
            cam.localPosition = shake;

            shakeDuration -= Time.deltaTime;

            await UniTask.Yield();
        }

        ResetCameraPosition();
    }

    private void OnDestroy()
    {
        ResetCameraPosition();
    }
}