using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : ShipBaseController
{
    public static PlayerController Instance;

    [Space]
    [SerializeField] private GameplayScriptable gameplayScriptable;

    [Space]
    [SerializeField] private Slider healthBar;

    [Header("Controllers")]
    public ControlsManager controls;
    public PlayerMovement movement;
    public PlayerShoot shoot;
    public Collider2D _collider;

    [ReadOnly] public bool copy = false;

    private int maxHealth = 0;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        Instance = null;
    }

    public void Init(ShipScriptable properties, bool _copy = false)
    {
        copy = _copy;

        if (controls == null)
        {
            controls = Instance.controls;
        }

        if (!copy)
        {
            Instance = this;

            movement.Init(this, controls);
            shoot.Init(this, controls);
        }

        SetHealth(properties.health);
        SetColor(properties);
    }

    public void SetHealthUi(int _health)
    {
        maxHealth = _health;

        healthBar.maxValue = maxHealth;
        healthBar.value = maxHealth;
        healthNormalized = (float)health / maxHealth;
    }

    public void SetColor(ShipScriptable shipBaseController)
    {
        _properties = shipBaseController;

        renderer.material.SetFloat(MaterialProperties.Hue, shipBaseController.hue);
        renderer.sprite = shipBaseController.sprite;
        renderer.transform.localScale = shipBaseController.spriteScale;

        module = engine1.main;
        module.startColor = shipBaseController.color;
        module = engine2.main;
        module.startColor = shipBaseController.color;
    }

    public void SetHealth(int value)
    {
        health = value;
    }

    private void Update()
    {
        if (gameObject.activeSelf)
        {
            if (!GameManager.Instance.hasEnded)
            {
                movement.OnUpdate();
            }

            if (GameManager.Instance.isPlaying)
            {
                shoot.OnUpdate();

                if (controls.power && GameManager.Instance.gameplayScriptable.selectedPowerUp != null)
                {
                    PowerUpsManager.Instance.AddPowerUp(GameManager.Instance.gameplayScriptable.selectedPowerUp);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Tags.Bullet_Enemy))
        {
            collision.gameObject.SetActive(false);

            PostProcessingController.Instance.VolumePunch();
            VfxPool.Instance.InitVfx(collision.transform.position);

            if (!copy)
            {
                CollisionForce(gameplayScriptable.forceTime, new Vector2(transform.position.x - collision.transform.position.x, -gameplayScriptable.force * 0.02f)).Forget();
            }

            DoDamage();
        }
        else if (collision.CompareTag(Tags.Collectable))
        {
            collision.gameObject.SetActive(false);

            GameManager.Instance.UpCoins(GameManager.Instance.gameplayScriptable.coinValue);
        }
        else if (collision.CompareTag(Tags.Enemy) && collision.TryGetComponent(out EnemyController enemyController) && enemyController._properties.enemyCollision)
        {
            PostProcessingController.Instance.VolumePunch();
            VfxPool.Instance.InitVfx(collision.transform.position);

            DoDamage(enemyController.health);
            enemyController.DoDamage();
        }
        else if (collision.CompareTag(Tags.PowerUp) && collision.TryGetComponent(out PowerUpCollectable powerUp))
        {
            PowerUpsManager.Instance.SelectPowerUp(powerUp.powerUp, false);
            powerUp.PlayPause(false);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag(Tags.Enemy) && collision.TryGetComponent(out EnemyController enemyController) && enemyController._properties.enemyCollision)
        {
            transform.position += new Vector3(
                (transform.position.x - collision.transform.position.x) * 0.5f,
                (transform.position.y - collision.transform.position.y) * 0.5f);
        }
    }

    public void DoDamage(int damage = 1)
    {
        PowerUpsManager.Player_Damage?.Invoke();

        if (health < 0)
        {
            return;
        }

        health -= damage;
        UpdateHealthUi();

        if (health <= 0)
        {
            if (!copy)
            {
                gameObject.SetActive(false);
                _collider.enabled = false;
                GameManager.Instance.EndLevel(true);
                PostProcessingController.Instance.ScreenShake(0.5f).Forget();
            }
            else
            {
                PowerUpsManager.Instance.RemovePowerUp(PowerUpsManager.Instance.currentPowerUp);
                Destroy(gameObject);
            }
        }
    }

    public void UpdateHealthUi()
    {
        if (copy)
        {
            return;
        }

        healthNormalized = (float)health / maxHealth;
        healthBar.value = health;

        PostProcessingController.Instance.SetVolumeHealth(healthNormalized.Remap(0, 1, PostProcessingController.Instance.maxVignette, 0));
    }

    private async UniTaskVoid CollisionForce(float _time, Vector3 direction)
    {
        float time = _time;

        while (gameObject.activeSelf && time > 0 && this != null)
        {
            transform.localPosition += gameplayScriptable.force * Time.deltaTime * direction;
            await UniTask.Yield();

            time -= Time.deltaTime;
        }
    }

    public async UniTaskVoid ImmuneBlink()
    {
        while (!GameManager.Instance.isPlaying)
        {
            renderer.enabled = !renderer.enabled;
            await UniTask.DelayFrame(2);
        }

        renderer.enabled = true;
    }
}