using Cysharp.Threading.Tasks;
using UnityEngine;

public class PowerUp_ElectricFieldBehaviour : MonoBehaviour
{
    [SerializeField] private CircleCollider2D circle;
    [SerializeField] private ParticleSystem mainParticle;
    private ParticleSystem.ShapeModule shapeModule;

    private bool active = false;
    private int enemyDamage = 0;

    public void Init(int damage, float range)
    {
        enemyDamage = damage;
        active = true;

        shapeModule = mainParticle.shape;
        shapeModule.radius = range;
        circle.radius = range;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!active)
        {
            return;
        }

        if (collision.CompareTag(Tags.Enemy) && collision.TryGetComponent(out EnemyController enemyController) && enemyController._properties.enemyCollision)
        {
            PostProcessingController.Instance.VolumePunch();
            VfxPool.Instance.InitVfx(collision.transform.position);

            enemyController.DoDamage(enemyDamage);
        }
    }

    public async UniTaskVoid Disable()
    {
        active = false;
        mainParticle.Stop(false, ParticleSystemStopBehavior.StopEmitting);

        await UniTask.WaitUntil(() => mainParticle.isStopped);

        Destroy(gameObject);
    }
}