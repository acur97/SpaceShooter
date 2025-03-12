using UnityEngine;

public class BulletsPool : PoolBaseController
{
    public static BulletsPool Instance;

    [ReadOnly] public Bullet[] bullets;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        Instance = null;
    }

    private new void Awake()
    {
        Instance = this;

        //objects = new GameObject[size];
        bullets = new Bullet[size];

        for (int i = 0; i < size; i++)
        {
            bullets[i].transform = Instantiate(prefab, transform).transform;
            BulletData data = new()
            {
                renderer = bullets[i].transform.GetComponent<SpriteRenderer>(),
                lifetime = -1,
                speed = 0
            };
            bullets[i].data = data;
        }
    }

    public void InitBullet(Transform _position, ShipScriptable _properties, Enums.TypeBullet type)
    {
        AudioManager.Instance.PlaySound(Enums.AudioType.Zap, 0.2f);

        Init(_position.position, _position.rotation, _properties, type);
    }

    public void InitBullet(Transform _position1, Transform _position2, ShipScriptable _properties, Enums.TypeBullet type)
    {
        AudioManager.Instance.PlaySound(Enums.AudioType.Zap, 0.2f);

        Init(_position1.position, _position1.rotation, _properties, type);
        Init(_position2.position, _position2.rotation, _properties, type);
    }

    private void Init(Vector2 _position, Quaternion _rotation, ShipScriptable _properties, Enums.TypeBullet type)
    {
        for (int i = 0; i < size; i++)
        {
            if (!bullets[i].transform.gameObject.activeSelf)
            {
                switch (type)
                {
                    case Enums.TypeBullet.player:
                        bullets[i].transform.tag = Tags.Bullet_Player;
                        break;

                    case Enums.TypeBullet.enemy:
                        bullets[i].transform.tag = Tags.Bullet_Enemy;
                        break;
                }

                bullets[i].transform.SetPositionAndRotation(_position, _rotation);
                bullets[i].data.speed = _properties.bulletSpeed;

                if (type == Enums.TypeBullet.player)
                {
                    bullets[i].data.renderer.color = PlayerController.Instance._properties.color;
                }
                else
                {
                    bullets[i].data.renderer.color = SetColor(PlayerController.Instance._properties.color);
                }

                if (_properties._bulletTime)
                {
                    bullets[i].data.lifetime = _properties.bulletTime;
                }

                bullets[i].transform.gameObject.SetActive(true);
                break;
            }
            else
            {
                continue;
            }
        }
    }

    private Color SetColor(Color col)
    {
        Color.RGBToHSV(col, out float h, out float s, out float v);

        h += 0.5f;
        if (h > 1)
        {
            h--;
        }

        return Color.HSVToRGB(h, s, v);
    }

    private void Update()
    {
        if (!GameManager.Instance.isPlaying)
        {
            return;
        }

        for (int i = 0; i < size; i++)
        {
            if (!bullets[i].transform.gameObject.activeSelf)
            {
                continue;
            }

            if (bullets[i].data.lifetime > 0)
            {
                bullets[i].data.lifetime -= Time.deltaTime;

                if (bullets[i].data.lifetime <= 0)
                {
                    bullets[i].transform.gameObject.SetActive(false);
                    continue;
                }
            }

            bullets[i].transform.position += bullets[i].data.speed * Time.deltaTime * bullets[i].transform.up;

            if (bullets[i].transform.position.x >= GameManager.BulletLimits.w ||
                bullets[i].transform.position.x <= GameManager.BulletLimits.z ||
                bullets[i].transform.position.y >= GameManager.BulletLimits.x ||
                bullets[i].transform.position.y <= GameManager.BulletLimits.y)
            {
                bullets[i].transform.gameObject.SetActive(false);
                bullets[i].data.lifetime = -1;
            }
        }
    }
}