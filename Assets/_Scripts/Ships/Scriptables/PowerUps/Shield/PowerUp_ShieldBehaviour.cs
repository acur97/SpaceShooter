using Cysharp.Threading.Tasks;
using UnityEngine;

public class PowerUp_ShieldBehaviour : MonoBehaviour
{
    [SerializeField] private CircleCollider2D circle;
    [SerializeField] private ParticleSystem mainParticle;
    private ParticleSystem.ShapeModule shapeModule;
    [SerializeField] private ParticleSystem[] particles;

    private bool active = false;
    private int bulletAbsortion = 0;

    public void Init(int absortion, float range)
    {
        bulletAbsortion = absortion;
        active = true;

        shapeModule = mainParticle.shape;
        shapeModule.radius = range;
        circle.radius = range;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!active)
        {
            return;
        }

        if (collision.CompareTag(Tags.Bullet) && collision.GetComponent<Bullet>().bulletBase.bulletType == Enums.TypeBullet.enemy)
        {
            collision.gameObject.SetActive(false);

            VfxPool.Instance.InitVfx(collision.transform.position);
            AudioManager.Instance.PlaySound(Enums.AudioType.Boom, 0.2f);

            bulletAbsortion--;

            if (bulletAbsortion <= 0)
            {
                Disable().Forget();
            }
        }
    }

    public async UniTaskVoid Disable()
    {
        active = false;

        for (int i = 0; i < particles.Length; i++)
        {
            particles[i].Stop(false, ParticleSystemStopBehavior.StopEmitting);
        }
        for (int i = 0; i < particles.Length; i++)
        {
            await UniTask.WaitUntil(() => particles[i].isStopped);
        }

        Destroy(gameObject);
    }
}