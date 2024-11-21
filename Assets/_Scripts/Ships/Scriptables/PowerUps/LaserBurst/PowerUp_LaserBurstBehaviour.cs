using Cysharp.Threading.Tasks;
using UnityEngine;

public class PowerUp_LaserBurstBehaviour : MonoBehaviour
{
    [SerializeField] private LineRenderer line;
    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] private ParticleSystemRenderer particlesRenderer;
    [SerializeField] private ParticleSystem particles;
    [SerializeField] private ParticleSystem[] extraParticles;
    [SerializeField] private AudioClip boomSound;

    private uint damage = 0;
    private ParticleSystem.MainModule mainModule;
    private ParticleSystem.MinMaxCurve startSpeedCurve;
    private readonly int _Color = Shader.PropertyToID("_Color");
    private const string _Enemy = "Enemy";

    private Vector3 laserPoint = Vector3.zero;
    private float range = 0;

    public async UniTaskVoid Init(float startDelay, uint _damage, Color color, float shakeDuration)
    {
        damage = _damage;

        line.enabled = false;
        boxCollider.enabled = false;

        mainModule = particles.main;
        mainModule.duration = startDelay;

        line.material.SetColor(_Color, color);
        particlesRenderer.material.SetColor(_Color, color);

        startSpeedCurve = mainModule.startSpeed;
        startSpeedCurve.mode = ParticleSystemCurveMode.Curve;
        mainModule.startSpeed = startSpeedCurve;

        particles.Play(false);

        await UniTask.WaitForSeconds(startDelay);

        PostProcessingController.Instance.ImpactFrame().Forget();

        AudioManager.Instance.PlaySound(boomSound);

        startSpeedCurve.mode = ParticleSystemCurveMode.Constant;
        startSpeedCurve.constant = 1f;
        mainModule.startSpeed = startSpeedCurve;

        line.SetPosition(1, laserPoint);

        line.enabled = true;
        boxCollider.enabled = true;

        PostProcessingController.Instance.ScreenShake(shakeDuration - startDelay).Forget();

        for (int i = 0; i < extraParticles.Length; i++)
        {
            extraParticles[i].Play();
        }

        while (range <= 20)
        {
            await UniTask.Yield();

            range += Time.deltaTime * 50f;
            laserPoint.y = range;

            line.SetPosition(1, laserPoint);
        }
    }

    public async UniTaskVoid Deactivate()
    {
        mainModule.simulationSpeed = 0.5f;
        particles.Stop(false, ParticleSystemStopBehavior.StopEmitting);

        for (int i = 0; i < extraParticles.Length; i++)
        {
            extraParticles[i].Stop(false, ParticleSystemStopBehavior.StopEmitting);
        }

        range = line.startWidth;

        while (range > 0)
        {
            range -= Time.deltaTime * 1.5f;

            if (range <= 0)
            {
                range = 0;
            }

            line.startWidth = range;

            await UniTask.Yield();
        }

        Destroy(gameObject);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag(_Enemy) && collision.TryGetComponent(out EnemyController enemyController))
        {
            PostProcessingController.Instance.VolumePunch();
            VfxPool.Instance.InitVfx(collision.transform.position);

            enemyController.DoDamage((int)damage);
        }
    }
}