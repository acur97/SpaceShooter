using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Properties")]
    public ShipScriptable _properties;
    private int _health = 0;
    private Vector2 startPosition;
    private float customFloat = 0;
    private bool customBool = false;

    [Space]
    [SerializeField] private new SpriteRenderer renderer;
    [SerializeField] private ParticleSystem engine1;
    [SerializeField] private ParticleSystem engine2;
    private ParticleSystem.MainModule module;

    [Header("Firing")]
    [SerializeField] private Transform shootRoot;
    private float timer = 0;

    private const string _Bullet = "Bullet";

    private void OnEnable()
    {
        renderer.sprite = _properties.sprite;

        module = engine1.main;
        module.startColor = _properties.color;
        module = engine2.main;
        module.startColor = _properties.color;

        _health = _properties.health;

        transform.localEulerAngles = new Vector3(0, 0, 180);
        switch (_properties.behaviour)
        {
            case ShipScriptable.Behaviour.linear:
                break;

            case ShipScriptable.Behaviour.direct:
                break;

            case ShipScriptable.Behaviour.waves:
                if (transform.localPosition.x > 3)
                {
                    transform.localPosition = new Vector3(3, transform.localPosition.y);
                }
                else if (transform.localPosition.x < -3)
                {
                    transform.localPosition = new Vector3(-3, transform.localPosition.y);
                }
                startPosition = transform.localPosition;
                break;

            case ShipScriptable.Behaviour.wavesDirect:
                if (transform.localPosition.x > 3)
                {
                    transform.localPosition = new Vector3(3, transform.localPosition.y);
                }
                else if (transform.localPosition.x < -3)
                {
                    transform.localPosition = new Vector3(-3, transform.localPosition.y);
                }
                startPosition = transform.localPosition;
                break;

            case ShipScriptable.Behaviour.diagonal:
                if (transform.localPosition.x > 0)
                {
                    transform.localEulerAngles = new Vector3(0, 0, 225);
                }
                else
                {
                    transform.localEulerAngles = new Vector3(0, 0, 135);
                }
                break;

            case ShipScriptable.Behaviour.wave8:
                transform.localPosition = new Vector3(0, 3.3f);
                startPosition = transform.localPosition;
                break;

            default:
                break;
        }

        timer = _properties.coolDown;
        customFloat = 0;
        customBool = false;
    }

    private void Update()
    {
        switch (_properties.behaviour)
        {
            case ShipScriptable.Behaviour.linear:
                transform.position += _properties.speed * Time.deltaTime * transform.up;

                if (transform.position.y <= -3.3f)
                {
                    Dead();
                }
                break;

            case ShipScriptable.Behaviour.direct:
                if (transform.position.y > 1.5f)
                {
                    transform.position += _properties.speed * Time.deltaTime * transform.up;
                }
                break;

            case ShipScriptable.Behaviour.waves:
                transform.position += _properties.speed * Time.deltaTime * transform.up;
                transform.localPosition = new Vector2(startPosition.x + Mathf.Sin(Time.time) * _properties.behaviourMathfSin, transform.localPosition.y);
                break;

            case ShipScriptable.Behaviour.wavesDirect:
                if (transform.position.y > 1.5f)
                {
                    transform.position += _properties.speed * Time.deltaTime * transform.up;
                }

                transform.localPosition = new Vector2(startPosition.x + Mathf.Sin(Time.time) * _properties.behaviourMathfSin, transform.localPosition.y);
                break;

            case ShipScriptable.Behaviour.diagonal:
                if (transform.localEulerAngles.z == 225)
                {
                    transform.position += _properties.speed * Time.deltaTime * -transform.right;
                }
                else
                {
                    transform.position += _properties.speed * Time.deltaTime * transform.right;
                }

                if (transform.position.y <= -3.3f)
                {
                    Dead();
                }
                break;

            case ShipScriptable.Behaviour.wave8:
                if (!customBool && transform.position.y > 1)
                {
                    transform.position += _properties.speed * Time.deltaTime * transform.up;
                    transform.localPosition = new Vector2(startPosition.x + Mathf.Sin(Time.time) * _properties.behaviourMathfSin, transform.localPosition.y);
                }
                else
                {
                    customBool = true;
                    transform.localPosition = new Vector2(
                        startPosition.x + Mathf.Sin(Time.time) * _properties.behaviourMathfSin,
                        Mathf.Lerp(transform.localPosition.y, 0.5f + Mathf.Sin(Time.time * 1.5f) * _properties.behaviourMathfSin / 2, customFloat));
                }

                if (customBool && customFloat < 1 && customFloat >= 0)
                {
                    customFloat += (Time.deltaTime / 4);
                }

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

                case ShipScriptable.Attack.continuousDouble:
                    timer -= Time.deltaTime;

                    if (timer <= 0)
                    {
                        Shoot(true);

                        timer = _properties.coolDown;
                    }
                    break;

                case ShipScriptable.Attack.none:
                    break;

                default:
                    break;
            }
        }
    }

    private void Shoot(bool _double = false)
    {
        BulletsPool.Instance.InitBullet(shootRoot, _properties.bulletSpeed, _double, Bullet.TypeBullet.enemy);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(_Bullet) && collision.GetComponent<Bullet>().bulletType == Bullet.TypeBullet.player)
        {
            collision.gameObject.SetActive(false);

            GameManager.Instance.VolumePunch();
            VfxPool.Instance.InitVfx(transform);

            _health--;
            if (_health <= 0)
            {
                Dead();
            }
        }
    }

    public void Dead()
    {
        GameManager.Instance.UpScore(GameManager.Instance.scoreEnemy);
        GameManager.Instance.leftForNextGroup--;
        transform.SetLocalPositionAndRotation(Vector2.zero, Quaternion.identity);
        gameObject.SetActive(false);
    }
}