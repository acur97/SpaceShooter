using Cysharp.Threading.Tasks;
using UnityEngine;

public class PowerUp_ElectricFieldBehaviour : MonoBehaviour
{
    [SerializeField] private ParticleSystem particles;

    private bool active = false;
    private int enemyDamage = 0;

    private const string _Enemy = "Enemy";

    public void Init(int damage)
    {
        enemyDamage = damage;
        active = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!active)
        {
            return;
        }

        if (collision.CompareTag(_Enemy) && collision.TryGetComponent(out EnemyController enemyController) && enemyController._properties.enemyCollision)
        {
            //PostProcessingController.Instance.VolumePunch();
            VfxPool.Instance.InitVfx(collision.transform.position);

            enemyController.DoDamage(enemyDamage);
        }
    }

    public async UniTaskVoid Disable()
    {
        active = false;
        particles.Stop(false, ParticleSystemStopBehavior.StopEmitting);

        await UniTask.WaitUntil(() => particles.isStopped);

        Destroy(gameObject);
    }
}