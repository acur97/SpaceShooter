using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 1;
    public enum TypeBullet { player, enemy }
    public TypeBullet bulletType;
    public new SpriteRenderer renderer;

    private void Update()
    {
        transform.localPosition = new Vector2(transform.localPosition.x, transform.localPosition.y + (speed * Time.deltaTime));

        if (transform.position.x >= 5.1f ||
            transform.position.x <= -5.1f ||
            transform.position.y >= 3f ||
            transform.position.y <= -3f)
        {
            gameObject.SetActive(false);
            transform.SetLocalPositionAndRotation(Vector2.zero, Quaternion.identity);
        }
    }
}