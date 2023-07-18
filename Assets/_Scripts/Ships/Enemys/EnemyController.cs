using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Properties")]
    public ShipScriptable _properties;
    private int _health = 0;

    [Space]
    [SerializeField] private new SpriteRenderer renderer;
    [SerializeField] private ParticleSystem engine1;
    [SerializeField] private ParticleSystem engine2;
    private ParticleSystem.MainModule module;

    [Header("Firing")]
    [SerializeField] private Transform shootRoot;
    private float timer = 0;

    private void Awake()
    {
        renderer.sprite = _properties.sprite;

        module = engine1.main;
        module.startColor = _properties.color;
        module = engine2.main;
        module.startColor = _properties.color;

        _health = _properties.health;
    }

    private void OnEnable()
    {
        transform.localEulerAngles = new Vector3(0, 0, 180);
        switch (_properties.behaviour)
        {
            case ShipScriptable.Behaviour.linear:
                break;

            case ShipScriptable.Behaviour.direct:
                break;

            case ShipScriptable.Behaviour.waves:
                break;

            case ShipScriptable.Behaviour.diagonal:
                transform.localEulerAngles = new Vector3(0, 0, 225);
                break;

            case ShipScriptable.Behaviour.wave8:
                break;

            case ShipScriptable.Behaviour.custom:
                break;

            default:
                break;
        }

        timer = _properties.coolDown;
    }

    private void Update()
    {
        switch (_properties.behaviour)
        {
            case ShipScriptable.Behaviour.linear:
                transform.position += _properties.speed * Time.deltaTime * transform.up;

                if (//transform.position.x >= 5.1f ||
                    //transform.position.x <= -5.1f ||
                    transform.position.y <= -3.3f)
                {
                    Dead();
                }
                break;

            case ShipScriptable.Behaviour.direct:
                break;

            case ShipScriptable.Behaviour.waves:
                break;

            case ShipScriptable.Behaviour.diagonal:
                break;

            case ShipScriptable.Behaviour.wave8:
                break;

            case ShipScriptable.Behaviour.custom:
                break;

            default:
                break;
        }

        if (transform.position.y >= -2.25f)
        {
            switch (_properties.attack)
            {
                case ShipScriptable.Attack.continuous:
                    timer -= Time.deltaTime;

                    if (timer <= 0)
                    {
                        Shoot();

                        timer = _properties.coolDown;
                    }
                    break;

                case ShipScriptable.Attack.burst:
                    break;

                case ShipScriptable.Attack.none:
                    break;

                default:
                    break;
            }
        }
    }

    private void Shoot()
    {
        BulletsPool.Instance.InitBullet(shootRoot, _properties.bulletSpeed, Bullet.TypeBullet.enemy);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet") && collision.GetComponent<Bullet>().bulletType == Bullet.TypeBullet.player)
        {
            collision.gameObject.SetActive(false);

            _health--;
            if (_health <= 0)
            {
                Dead();
            }
        }
    }

    public void Dead()
    {
        GameManager.Instance.leftForNextGroup--;
        transform.SetLocalPositionAndRotation(Vector2.zero, Quaternion.identity);
        gameObject.SetActive(false);
    }
}