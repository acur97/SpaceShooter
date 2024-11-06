using Cysharp.Threading.Tasks;
using UnityEngine;

public class PowerUp_ShieldBehaviour : MonoBehaviour
{
    [SerializeField] private ParticleSystem particles;

    private bool active = false;
    private int bulletAbsortion = 0;

    private const string _Bullet = "Bullet";

    public void Init(int absortion)
    {
        bulletAbsortion = absortion;
        active = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!active)
        {
            return;
        }

        if (collision.CompareTag(_Bullet) && collision.GetComponent<Bullet>().bulletType == Bullet.TypeBullet.enemy)
        {
            collision.gameObject.SetActive(false);

            //PostProcessingController.Instance.VolumePunch();
            VfxPool.Instance.InitVfx(collision.transform.position);

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
        particles.Stop(false, ParticleSystemStopBehavior.StopEmitting);

        await UniTask.WaitUntil(() => particles.isStopped);

        Destroy(gameObject);
    }
}