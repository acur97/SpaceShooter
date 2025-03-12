using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

public class PostProcessingController : MonoBehaviour
{
    public static PostProcessingController Instance;

    [Header("Volume Control")]
    [SerializeField] private Volume volume;
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

    private Camera cam;
    private Transform camT;
    [Header("Camera Control")]
    [SerializeField] private AnimationCurve shakeCurve;
    private float shakeDuration = 0f;
    private Vector3 cameraOrigin = new(0f, 0f, -1f);
    private Vector3 shake;

    [Header("Renderer Control")]
    [SerializeField] private ScriptableRendererFeature[] impactFrameFeature;
    [SerializeField] private Material impactFrameMaterial;
    [SerializeField] private float impactFrameSpacing = 0.1f;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        Instance = null;
    }

    public void Init()
    {
        Instance = this;

#if Platform_Mobile && UNITY_ANDROID && !UNITY_EDITOR
        Application.targetFrameRate = (int)Screen.currentResolution.refreshRateRatio.value;
#endif
        //QualitySettings.maxQueuedFrames = 1;

        SetImpactFrameFeature(false);

        cam = Camera.main;
        camT = cam.transform;

        ResetCameraPosition();

        profile = volume.profile;
        profile.TryGet(out colorAdjustments);
        profile.TryGet(out chromaticAberration);
        profile.TryGet(out vignette);
    }

    private void SetImpactFrameFeature(bool on)
    {
        for (int i = 0; i < impactFrameFeature.Length; i++)
        {
            impactFrameFeature[i].SetActive(on);
        }
    }

    private void ResetCameraPosition()
    {
        if (camT == null)
        {
            return;
        }

        camT.localPosition = cameraOrigin;
    }

    public void VolumePunch()
    {
        LeanTween.cancel(tweenPostExposure1);
        LeanTween.cancel(tweenPostExposure2);
        tweenPostExposure1 = LeanTween.value(colorAdjustments.postExposure.value, exposureRange.y, volumeSpeed)
            .setOnUpdate(OnUpdatePostExposure)
            .setOnComplete(OnCompletePostExposure).id;

        LeanTween.cancel(tweenChromaticAberration1);
        LeanTween.cancel(tweenChromaticAberration2);
        tweenChromaticAberration1 = LeanTween.value(chromaticAberration.intensity.value, chromaticAberrationRange.y, volumeSpeed)
            .setOnUpdate(OnUpdateChromaticAberration)
            .setOnComplete(OnCompleteChromaticAberration).id;

        AudioManager.Instance.PlaySound(Enums.AudioType.Boom, 0.2f);
    }

    private void OnUpdatePostExposure(float value)
    {
        colorAdjustments.saturation.value = value;
    }

    private void OnCompletePostExposure()
    {
        tweenPostExposure2 = LeanTween.value(colorAdjustments.postExposure.value, exposureRange.x, volumeSpeed).setOnUpdate(OnUpdatePostExposure).id;
    }

    private void OnUpdateChromaticAberration(float value)
    {
        chromaticAberration.intensity.value = value;
    }

    private void OnCompleteChromaticAberration()
    {
        tweenChromaticAberration2 = LeanTween.value(chromaticAberration.intensity.value, chromaticAberrationRange.x, volumeSpeed).setOnUpdate(OnUpdateChromaticAberration).id;
    }

    public void SetVolumeHealth(float val)
    {
        vignette.intensity.value = val;
    }

    public async UniTaskVoid ScreenShake(float duration = 1f, float shakeAmount = 0.03f)
    {
        shakeDuration = duration;

        while (shakeDuration > 0 && camT != null)
        {
            shake = cameraOrigin + shakeAmount * shakeCurve.Evaluate(shakeDuration.Remap(0, duration, 1, 0)) * Random.insideUnitSphere;
            shake.z = -1f;
            camT.localPosition = shake;

            shakeDuration -= Time.deltaTime;

            await UniTask.Yield();
        }

        ResetCameraPosition();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            ImpactFrame().Forget();
        }
    }

    public async UniTaskVoid ImpactFrame(bool lastFrame = true, Action onComplete = null)
    {
        impactFrameMaterial.SetInt(MaterialProperties.Invert, 0);
        impactFrameMaterial.SetVector(MaterialProperties.Frecuency, new Vector2(5, 100));
        impactFrameMaterial.SetVector(MaterialProperties.Offset, cam.WorldToViewportPoint(PlayerController.Instance.transform.position));

        SetImpactFrameFeature(true);

        await UniTask.WaitForSeconds(impactFrameSpacing);

        impactFrameMaterial.SetInt(MaterialProperties.Invert, 1);

        await UniTask.WaitForSeconds(impactFrameSpacing);

        impactFrameMaterial.SetVector(MaterialProperties.Frecuency, new Vector2(-5, -100));

        await UniTask.WaitForSeconds(impactFrameSpacing);

        if (lastFrame)
        {
            impactFrameMaterial.SetInt(MaterialProperties.Invert, 0);

            await UniTask.WaitForSeconds(impactFrameSpacing);
        }

        SetImpactFrameFeature(false);

        onComplete?.Invoke();
    }

    private void OnDestroy()
    {
        ResetCameraPosition();
    }
}