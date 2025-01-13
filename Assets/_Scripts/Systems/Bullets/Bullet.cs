using UnityEngine;

public class Bullet : MonoBehaviour
{
    public BulletBase bulletBase;

    private float lifetime = -1f;

    private void Awake()
    {
        PoolsUpdateManager.PoolUpdate += OnUpdate;
    }

    private void OnDestroy()
    {
        PoolsUpdateManager.PoolUpdate -= OnUpdate;
    }

    public void SetLifetime(float time)
    {
        lifetime = time;
    }

    private void OnDisable()
    {
        lifetime = -1f;
    }

    private void OnUpdate()
    {
        if (!gameObject.activeSelf || !GameManager.Instance.isPlaying)
        {
            return;
        }

        if (lifetime > 0)
        {
            lifetime -= Time.deltaTime;

            if (lifetime <= 0)
            {
                gameObject.SetActive(false);
                return;
            }
        }

        transform.position += bulletBase.speed * Time.deltaTime * transform.up;

        if (transform.position.x >= GameManager.BulletLimits.w ||
            transform.position.x <= GameManager.BulletLimits.z ||
            transform.position.y >= GameManager.BulletLimits.x ||
            transform.position.y <= GameManager.BulletLimits.y)
        {
            gameObject.SetActive(false);
        }
    }
}