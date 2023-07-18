using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    [Header("Properties")]
    public ShipScriptable _properties;
    private int _health = 0;

    [Space]
    [SerializeField] private new SpriteRenderer renderer;
    [SerializeField] private ParticleSystem engine1;
    [SerializeField] private ParticleSystem engine2;
    private ParticleSystem.MainModule module;

    private Vector3 inputMove;

    [Header("Firing")]
    [SerializeField] private Transform shootRoot;
    private bool hold = false;
    private float timer = -1;

    private readonly int _ColorCapsule = Shader.PropertyToID("_ColorCapsule");
    private const string _Horizontal = "Horizontal";
    private const string _Vertical = "Vertical";
    private readonly string _Fire = "Fire1";
    private const string _Bullet = "Bullet";
    private const string _Collectable = "Collectable";

    private void Awake()
    {
        Instance = this;

        SetHealth(_properties.health);
        SetColor();
    }

    public void SetColor()
    {
        renderer.material.SetColor(_ColorCapsule, _properties.color);

        module = engine1.main;
        module.startColor = _properties.color;
        module = engine2.main;
        module.startColor = _properties.color;
    }

    public void SetHealth(int value)
    {
        _health = value;
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
                Mathf.Clamp(transform.localPosition.x + inputMove.x, -4.5f, 4.5f),
                Mathf.Clamp(transform.localPosition.y + inputMove.y, -2.5f, 2.5f));

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

            _health--;
            if (_health <= 0)
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