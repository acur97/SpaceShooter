using UnityEngine;

public class EnemyController : ShipBaseController
{
    [Header("Behaviour")]
    [SerializeField] private EnemyMovement movement;
    [SerializeField] private EnemyShoot shoot;

    private bool dead = true;

    private void Awake()
    {
        PoolsUpdateManager.PoolUpdate += OnUpdate;
    }

    private void OnEnable()
    {
        dead = false;

        renderer.material.SetFloat(MaterialProperties.Hue, _properties.hue);
        renderer.sprite = _properties.sprite;
        renderer.transform.localScale = _properties.spriteScale;

        health = _properties.health;

        module = engine1.main;
        module.startColor = _properties.color;
        module = engine2.main;
        module.startColor = _properties.color;

        transform.localEulerAngles = new Vector3(0, 0, 180);

        movement.Init(_properties.behaviour, _properties._timeToContinue, _properties.timeToContinue, _properties.spawnIndex);
        shoot.Init(_properties, shootRoot1, shootRoot2, shootRoot3, shootRoot4, shootRoot5);

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

        if (transform.position.x >= GameManager.BoundsLimits.w ||
            transform.position.x <= GameManager.BoundsLimits.z ||
            transform.position.y >= GameManager.BoundsLimits.x ||
            transform.position.y <= GameManager.BoundsLimits.y)
        {
            Dead(true);
            return;
        }

        if (transform.position.y >= GameManager.PlayerLimits.y && _properties.attack != Enums.Attack.None)
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
            shoot.Shoot();

            timer = _properties.coolDown;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Tags.Bullet_Player))
        {
            collision.gameObject.SetActive(false);

            PostProcessingController.Instance.VolumePunch();
            VfxPool.Instance.InitVfx(collision.transform.position);

            DoDamage();
        }
    }

    public void DoDamage(int damage = 1)
    {
        PowerUpsManager.Enemy_Damage?.Invoke(this);

        health -= damage;

        if (!dead && health <= 0)
        {
            Dead();
        }
    }

    public void Dead(bool outOfBounds = false)
    {
        dead = true;
        gameObject.SetActive(false);

        if (_properties.countForGroup)
        {
            GameManager.Instance.leftForNextGroup--;
        }

        if (!outOfBounds)
        {
            PowerUpsManager.Enemy_Death?.Invoke(this);

            if (RoundsController.Instance.levelType == RoundsController.LevelType.Normal)
            {
                GameManager.Instance.UpScore(_properties.deathScore);
            }
        }
    }
}