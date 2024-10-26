using UnityEngine;

public class EnemyController : ShipBaseController
{
    private const string _Bullet = "Bullet";

    [Header("Behaviour")]
    [SerializeField] private EnemyMovement movement;
    [SerializeField] private EnemyShoot shoot;

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

        movement.Init(_properties.behaviour, _properties._timeToContinue, _properties.timeToContinue, _properties.spawnIndex);

        timer = _properties.coolDown;
        timer2 = _properties.spaceCooldown;
        timer2Up = false;
    }

    private void OnDestroy()
    {
        PoolsUpdateManager.PoolUpdate -= OnUpdate;
    }

    private void OnUpdate()
    {
        if (!gameObject.activeSelf || !GameManager.Instance.isPlaying)
        {
            return;
        }

        movement.Move(_properties);

        if (transform.position.x >= GameManager.BoundsLimits.x ||
            transform.position.x <= -GameManager.BoundsLimits.x ||
            transform.position.y >= GameManager.BoundsLimits.y ||
            transform.position.y <= -GameManager.BoundsLimits.y)
        //if (transform.position.y <= -GameManager.BoundsLimits.y || transform.position.y >= GameManager.BoundsLimits.y)
        {
            Dead(true);
            return;
        }

        if (transform.position.y >= -GameManager.PlayerLimits.y && _properties.attack != ShipScriptable.Attack.none)
        {
            timer -= Time.deltaTime;

            if (_properties._spaceCoolDown && _properties.spaceCooldown > 0)
            {
                if (timer2Up)
                {
                    timer2 += Time.deltaTime;

                    if (timer2 > _properties.spaceCooldown)
                    {
                        timer2 = _properties.spaceCooldown;
                        timer2Up = false;
                    }

                    TimerToShoot();
                }
                else
                {
                    timer2 -= Time.deltaTime;

                    if (timer2 < 0)
                    {
                        timer2 = 0;
                        timer2Up = true;
                    }
                }
            }
            else
            {
                TimerToShoot();
            }
        }
    }

    private void TimerToShoot()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            shoot.Shoot(shootRoot, _properties);

            timer = _properties.coolDown;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(_Bullet) && collision.GetComponent<Bullet>().bulletType == Bullet.TypeBullet.player)
        {
            collision.gameObject.SetActive(false);

            PostProcessingController.Instance.VolumePunch();
            VfxPool.Instance.InitVfx(transform.position);

            DoDamage();
        }
    }

    public void DoDamage()
    {
        health--;

        if (health == 0)
        {
            Dead();
        }
    }

    public void Dead(bool outOfBounds = false)
    {
        gameObject.SetActive(false);

        GameManager.Instance.leftForNextGroup--;

        if (outOfBounds)
        {
            return;
        }

        GameManager.Instance.UpScore(GameManager.Instance.scoreEnemy);
    }
}