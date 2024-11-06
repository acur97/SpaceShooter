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

    public bool copy = false;

    private readonly int _ColorCapsule = Shader.PropertyToID("_ColorCapsule");
    private readonly int _Color = Shader.PropertyToID("_Color");
    private const string _Bullet = "Bullet";
    private const string _Enemy = "Enemy";
    private const string _Collectable = "Collectable";

    public void Init(bool _copy = false)
    {
        copy = _copy;

        if (controls == null)
        {
            controls = Instance.controls;
        }

        if (!copy)
        {
            Instance = this;

            healthBar.maxValue = _properties.health;
            healthBar.value = _properties.health;
            healthNormalized = (float)health / _properties.health;

            movement.Init(this, controls);
            shoot.Init(this, controls);
        }

        SetHealth(_properties.health);
        SetColor();
    }

    public void SetColor()
    {
        if (!copy)
        {
            renderer.material.SetColor(_ColorCapsule, _properties.color);
            renderer.material.SetColor(_Color, ConvertColor(_properties.color));
        }
        else
        {
            renderer.material = _properties.material;
            renderer.sprite = _properties.sprite;
        }

        module = engine1.main;
        module.startColor = _properties.color;
        module = engine2.main;
        module.startColor = _properties.color;
    }

    private Color ConvertColor(Color col)
    {
        Color.RGBToHSV(col, out float h, out float s, out float v);
        return Color.HSVToRGB(h, s * 0.25f, v);
    }

    public void SetHealth(int value)
    {
        health = value;
    }

    private void Update()
    {
        if (GameManager.Instance.isPlaying)
        {
            movement.OnUpdate();
            shoot.OnUpdate();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(_Bullet) && collision.GetComponent<Bullet>().bulletType == Bullet.TypeBullet.enemy)
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
        else if (collision.CompareTag(_Collectable))
        {
            collision.gameObject.SetActive(false);

            GameManager.Instance.UpScore(GameManager.Instance.gameplayScriptable.coinValue);
        }
        else if (collision.CompareTag(_Enemy) && collision.TryGetComponent(out EnemyController enemyController) && enemyController._properties.enemyCollision)
        {
            PostProcessingController.Instance.VolumePunch();
            VfxPool.Instance.InitVfx(collision.transform.position);

            enemyController.DoDamage();

            DoDamage();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag(_Enemy) && collision.TryGetComponent(out EnemyController enemyController) && enemyController._properties.enemyCollision)
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
        PostProcessingController.Instance.SetVolumeHealth(healthNormalized.Remap(0, 1, PostProcessingController.Instance.maxVignette, 0));

        if (health <= 0)
        {
            if (!copy)
            {
                GameManager.Instance.EndLevel();
            }
            else
            {
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

        healthNormalized = (float)health / _properties.health;
        healthBar.value = health;
    }

    private async UniTaskVoid CollisionForce(float _time, Vector3 direction)
    {
        float time = _time;

        while (time > 0 && this != null)
        {
            transform.localPosition += gameplayScriptable.force * Time.deltaTime * direction;
            movement.ClampPosition();
            await UniTask.Yield();

            time -= Time.deltaTime;
        }
    }
}