using UnityEngine;

public class EnemyController : ShipBaseController
{
    private Vector2 startPosition;
    private float customFloat = 0;
    private bool customBool = false;

    private const string _Bullet = "Bullet";

    private void Awake()
    {
        PoolsUpdateManager.PoolUpdate += OnUpdate;
    }

    private void OnEnable()
    {
        renderer.sprite = _properties.sprite;
        health = _properties.health;

        module = engine1.main;
        module.startColor = _properties.color;
        module = engine2.main;
        module.startColor = _properties.color;

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

        switch (_properties.behaviour)
        {
            case ShipScriptable.Behaviour.linear:
                transform.position += _properties.speed * Time.deltaTime * transform.up;
                break;

            case ShipScriptable.Behaviour.direct:
                if (transform.position.y > GameManager.EnemyLine)
                {
                    transform.position += _properties.speed * Time.deltaTime * transform.up;
                }
                break;

            case ShipScriptable.Behaviour.waves:
                transform.position += _properties.speed * Time.deltaTime * transform.up;
                transform.localPosition = new Vector2(startPosition.x + Mathf.Sin(Time.time * GameManager.HorizontalInvertedMultiplier) * _properties.behaviourMathfSin * GameManager.HorizontalMultiplier, transform.localPosition.y);
                break;

            case ShipScriptable.Behaviour.wavesDirect:
                if (transform.position.y > GameManager.EnemyLine)
                {
                    transform.position += _properties.speed * Time.deltaTime * transform.up;
                }

                transform.localPosition = new Vector2(startPosition.x + Mathf.Sin(Time.time * GameManager.HorizontalInvertedMultiplier) * _properties.behaviourMathfSin * GameManager.HorizontalMultiplier, transform.localPosition.y);
                break;

            case ShipScriptable.Behaviour.diagonal:
                if (transform.localEulerAngles.z == 135)
                {
                    transform.position += _properties.speed * Time.deltaTime * -transform.right;
                }
                else
                {
                    transform.position -= _properties.speed * Time.deltaTime * -transform.right;
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

        if (transform.position.y <= -GameManager.BoundsLimits.y || transform.position.y >= GameManager.BoundsLimits.y)
        {
            Dead();
        }

        if (transform.position.y >= -GameManager.PlayerLimits.y)
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

            health--;
            if (health == 0)
            {
                Dead();
            }
        }
    }

    public void Dead()
    {
        gameObject.SetActive(false);
        transform.SetLocalPositionAndRotation(Vector2.zero, Quaternion.identity);

        GameManager.Instance.leftForNextGroup--;
        GameManager.Instance.UpScore(GameManager.Instance.scoreEnemy);
    }
}