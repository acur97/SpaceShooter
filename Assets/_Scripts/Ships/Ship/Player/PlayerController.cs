using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : ShipBaseController
{
    public static PlayerController Instance { get; private set; }

    [Header("Explosion Feedback")]
    [SerializeField] private float forceTime = 0.1f;
    [SerializeField] private float force = 1;

    [Space]
    [SerializeField] private ControlsManager controls;
    [SerializeField] private Slider healthBar;

    private Vector3 inputMove;
    private bool hold = false;

    private readonly int _ColorCapsule = Shader.PropertyToID("_ColorCapsule");
    private readonly int _Color = Shader.PropertyToID("_Color");
    private const string _Bullet = "Bullet";
    private const string _Collectable = "Collectable";

    private void Awake()
    {
        Instance = this;

        healthBar.maxValue = _properties.health;
        healthBar.value = _properties.health;
        healthNormalized = (float)health / _properties.health;
        SetHealth(_properties.health);

        SetColor();
    }

    public void SetColor()
    {
        renderer.material.SetColor(_ColorCapsule, _properties.color);
        renderer.material.SetColor(_Color, ConvertColor(_properties.color));

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
            inputMove = new Vector2(controls.move.x * _properties.speed * Time.deltaTime, controls.move.y * _properties.speed * Time.deltaTime);

            transform.localPosition = new Vector2(
                Mathf.Clamp(transform.localPosition.x + inputMove.x, -GameManager.PlayerLimits.x, GameManager.PlayerLimits.x),
                Mathf.Clamp(transform.localPosition.y + inputMove.y, -GameManager.PlayerLimits.y, GameManager.PlayerLimits.y));

            if (controls.fireDown)
            {
                hold = true;
                timer = _properties.coolDown;
            }
            if (controls.fireUp)
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
    }

    private void Shoot()
    {
        BulletsPool.Instance.InitBullet(shootRoot, _properties.bulletSpeed, false, Bullet.TypeBullet.player);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(_Bullet) && collision.GetComponent<Bullet>().bulletType == Bullet.TypeBullet.enemy)
        {
            collision.gameObject.SetActive(false);

            PostProcessingController.Instance.VolumePunch();
            VfxPool.Instance.InitVfx(transform.position);

            CollisionForce(forceTime, new Vector2(transform.position.x - collision.transform.position.x, -force * 0.02f)).Forget();

            if (health < 0)
            {
                return;
            }

            health--;
            healthNormalized = (float)health / _properties.health;
            healthBar.value = health;
            PostProcessingController.Instance.SetVolumeHealth(healthNormalized.Remap(0, 1, PostProcessingController.Instance.maxVignette, 0));

            if (health <= 0)
            {
                GameManager.Instance.EndLevel();
            }
        }
        else if (collision.CompareTag(_Collectable))
        {
            collision.gameObject.SetActive(false);

            GameManager.Instance.UpScore(GameManager.Instance.scoreCoin);
        }
    }

    private async UniTaskVoid CollisionForce(float _time, Vector3 direction)
    {
        float time = _time;

        while (time > 0)
        {
            transform.localPosition += force * Time.deltaTime * direction;
            await UniTask.Yield();

            time -= Time.deltaTime;
        }
    }
}