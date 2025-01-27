using Cysharp.Threading.Tasks;
using UnityEngine;

public class PowerUp_OrbitalLaserBehaviour : MonoBehaviour
{
    [SerializeField] private int damage = 1000;
    [SerializeField] private GameObject circle1;
    [SerializeField] private Transform circle2;
    [SerializeField] private Animator circle1Anim;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private AudioClip boomSound;

    private float counter = 4.5f;

    private void OnEnable()
    {
        BlindCircle().Forget();
    }

    private async UniTaskVoid BlindCircle()
    {
        circle2.localScale = Vector3.zero;

        await UniTask.WaitUntil(() => circle1Anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);

        circle1.SetActive(false);

        PostProcessingController.Instance.ImpactFrame(false).Forget();
        PostProcessingController.Instance.ScreenShake(counter, 0.1f).Forget();
        PostProcessingController.Instance.VolumePunch();

        AudioManager.Instance.PlaySound(boomSound, 2f);

        while (circle2.localScale.x < 150f)
        {
            await UniTask.Yield();

            circle2.localScale += Time.deltaTime * 225f * Vector3.one;
        }

        Sparkles().Forget();

        for (int i = 0; i < EnemySpawns.Instance.enemys.Length; i++)
        {
            if (EnemySpawns.Instance.enemys[i].gameObject.activeSelf)
            {
                EnemySpawns.Instance.enemys[i].DoDamage(damage);
            }
        }

        for (int i = 0; i < BulletsPool.Instance.bullets.Length; i++)
        {
            if (BulletsPool.Instance.bullets[i].gameObject.activeSelf)
            {
                BulletsPool.Instance.bullets[i].gameObject.SetActive(false);
            }
        }

        for (int i = 0; i < CollectablesPool.Instance.objects.Length; i++)
        {
            if (CollectablesPool.Instance.objects[i].activeSelf)
            {
                CollectablesPool.Instance.objects[i].SetActive(false);
            }
        }

        while (sprite.color.a > 0f && sprite != null)
        {
            await UniTask.Yield();

            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, sprite.color.a - Time.deltaTime * 0.225f);
        }
    }

    private async UniTaskVoid Sparkles()
    {
        while (counter > 0 && this != null)
        {
            await UniTask.Yield();

            counter -= Time.deltaTime;

            VfxPool.Instance.InitVfx(new Vector2(
                Random.Range(GameManager.BulletLimits.z, GameManager.BulletLimits.w),
                Random.Range(GameManager.BulletLimits.y, GameManager.BulletLimits.x)));
        }

        Destroy(gameObject);
    }
}