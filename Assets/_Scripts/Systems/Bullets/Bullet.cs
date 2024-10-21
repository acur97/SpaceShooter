using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 1;
    public enum TypeBullet { player, enemy }
    public TypeBullet bulletType;
    public new SpriteRenderer renderer;
    public new Collider2D collider;

    private void Awake()
    {
        PoolsUpdateManager.PoolUpdate += OnUpdate;
    }

    private void OnDestroy()
    {
        PoolsUpdateManager.PoolUpdate -= OnUpdate;
    }

    private void OnUpdate()
    {
        if (!gameObject.activeSelf)
        {
            return;
        }

        transform.position += speed * Time.deltaTime * transform.up;

        if (transform.position.x >= GameManager.BulletLimits.x ||
            transform.position.x <= -GameManager.BulletLimits.x ||
            transform.position.y >= GameManager.BulletLimits.y ||
            transform.position.y <= -GameManager.BulletLimits.y)
        {
            gameObject.SetActive(false);
        }
    }
}