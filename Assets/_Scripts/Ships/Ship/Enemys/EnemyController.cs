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

        movement._timeToContinue = _properties.timeToContinue;

        transform.localEulerAngles = new Vector3(0, 0, 180);

        movement.Init(_properties.behaviour);
        shoot.Init(_properties, shootRoot);

        timer = _properties.coolDown;
        movement.customFloat = 0;
        movement.customBool = false;
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

        movement.Move(_properties);

        if (transform.position.y <= -GameManager.BoundsLimits.y || transform.position.y >= GameManager.BoundsLimits.y)
        {
            Dead();
        }


        if (transform.position.y >= -GameManager.PlayerLimits.y && _properties.attack != ShipScriptable.Attack.none)
        {
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                shoot.Shoot();

                timer = _properties.coolDown;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(_Bullet) && collision.GetComponent<Bullet>().bulletType == Bullet.TypeBullet.player)
        {
            collision.gameObject.SetActive(false);

            PostProcessingController.Instance.VolumePunch();
            VfxPool.Instance.InitVfx(transform.position);

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

        GameManager.Instance.leftForNextGroup--;
        GameManager.Instance.UpScore(GameManager.Instance.scoreEnemy);
    }
}