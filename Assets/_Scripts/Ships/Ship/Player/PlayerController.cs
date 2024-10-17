using UnityEngine;
using UnityEngine.UI;

public class PlayerController : ShipBaseController
{
    public static PlayerController Instance { get; private set; }

    [SerializeField] private Slider healthBar;

    private Vector3 inputMove;
    private bool hold = false;

    private readonly int _ColorCapsule = Shader.PropertyToID("_ColorCapsule");
    private readonly int _Color = Shader.PropertyToID("_Color");
    private const string _Horizontal = "Horizontal";
    private const string _Vertical = "Vertical";
    private const string _Fire = "Fire1";
    private const string _Bullet = "Bullet";
    private const string _Collectable = "Collectable";

    private void Awake()
    {
        Instance = this;

        healthBar.maxValue = _properties.health;
        healthBar.value = _properties.health;
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
            //move
            inputMove = new Vector2(
                Input.GetAxis(_Horizontal) * _properties.speed * Time.deltaTime,
                Input.GetAxis(_Vertical) * _properties.speed * Time.deltaTime);

            transform.localPosition = new Vector2(
                Mathf.Clamp(transform.localPosition.x + inputMove.x, -GameManager.PlayerLimits.x, GameManager.PlayerLimits.x),
                Mathf.Clamp(transform.localPosition.y + inputMove.y, -GameManager.PlayerLimits.y, GameManager.PlayerLimits.y));

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

            GameManager.Instance.VolumePunch();
            VfxPool.Instance.InitVfx(transform);

            health--;
            healthBar.value = health;
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
}