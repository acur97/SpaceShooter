using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private const string _Horizontal = "Horizontal";
    private const string _Vertical = "Vertical";
    private readonly string _Fire = "Fire1";

    [Header("Properties")]
    [SerializeField] private ShipScriptable _properties;

    [Space]
    [SerializeField] private new SpriteRenderer renderer;
    [SerializeField] private ParticleSystem engine1;
    [SerializeField] private ParticleSystem engine2;
    private ParticleSystem.MainModule module;

    [Header("Control")]
    [SerializeField] private Vector2 limits;

    private Vector3 inputMove;

    [Header("Firing")]
    [SerializeField] private Transform shootRoot;
    private bool hold = false;
    private float timer = -1;

    private void Awake()
    {
        renderer.material.SetColor("_ColorCapsule", _properties.color);

        module = engine1.main;
        module.startColor = _properties.color;
        module = engine2.main;
        module.startColor = _properties.color;
    }

    private void Update()
    {
        //move
        inputMove = new Vector2(
            Input.GetAxis(_Horizontal) * _properties.speed * Time.deltaTime,
            Input.GetAxis(_Vertical) * _properties.speed * Time.deltaTime);

        transform.localPosition = new Vector2(
            Mathf.Clamp(transform.localPosition.x + inputMove.x, -limits.x, limits.x),
            Mathf.Clamp(transform.localPosition.y + inputMove.y, -limits.y, limits.y));

        //shoot
        if (Input.GetButtonDown(_Fire))
        {
            hold = true;
            timer = _properties.coolDown;
        }
        if (Input.GetButtonUp(_Fire))
        {
            hold = false;
            timer = -1;
        }

        if (hold && timer >= 0)
        {
            if (timer == _properties.coolDown)
            {
                Shoot();
            }

            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                timer = _properties.coolDown;
            }
        }
    }

    private void Shoot()
    {
        BulletsPool.Instance.InitBullet(shootRoot, _properties.bulletSpeed, Bullet.TypeBullet.player);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet") && collision.GetComponent<Bullet>().bulletType == Bullet.TypeBullet.enemy)
        {
            collision.gameObject.SetActive(false);
            Debug.LogWarning("Muerto papu");
        }
        else if (collision.CompareTag("Collectable"))
        {
            collision.gameObject.SetActive(false);
            Debug.LogWarning("colectable");
        }
        else if (CompareTag("Enemy"))
        {
            collision.GetComponent<EnemyController>().Dead();
            Debug.LogWarning("Re Muerto papu");
        }
    }
}