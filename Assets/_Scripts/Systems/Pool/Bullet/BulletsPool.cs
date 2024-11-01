using UnityEngine;

public class BulletsPool : PoolBaseController
{
    public static BulletsPool Instance;

    [SerializeField] private Bullet[] bullets;

    private readonly int _Color = Shader.PropertyToID("_Color");

    private new void Awake()
    {
        Instance = this;

        //objects = new GameObject[size];
        bullets = new Bullet[size];

        for (int i = 0; i < size; i++)
        {
            bullets[i] = Instantiate(prefab, transform).GetComponent<Bullet>();
            bullets[i].renderer.material.SetColor(_Color, Color.white);
        }
    }

    public void InitBullet(Transform _position, float _speed, bool _double, Bullet.TypeBullet type)
    {
        AudioManager.Instance.PlaySound(AudioManager.AudioType.Zap, 0.2f);

        if (_double)
        {
            if (_position.parent.localEulerAngles.z == 0)
            {
                Init(new Vector2(_position.position.x + 0.157f, _position.position.y), _position.rotation, _speed, type);
                Init(new Vector2(_position.position.x - 0.157f, _position.position.y), _position.rotation, _speed, type);
            }
            else
            {
                Init(new Vector2(_position.position.x, _position.position.y + 0.157f), _position.rotation, _speed, type);
                Init(new Vector2(_position.position.x, _position.position.y - 0.157f), _position.rotation, _speed, type);
            }
        }
        else
        {
            Init(_position.position, _position.rotation, _speed, type);
        }
    }

    private void Init(Vector2 _position, Quaternion _rotation, float _speed, Bullet.TypeBullet type)
    {
        for (int i = 0; i < size; i++)
        {
            if (!bullets[i].gameObject.activeSelf)
            {
                bullets[i].transform.SetPositionAndRotation(_position, _rotation);
                bullets[i].speed = _speed;
                bullets[i].bulletType = type;

                if (type == Bullet.TypeBullet.player)
                {
                    bullets[i].renderer.material.SetColor(_Color, PlayerController.Instance._properties.color);
                }
                else
                {
                    bullets[i].renderer.material.SetColor(_Color, SetColor(PlayerController.Instance._properties.color));
                }
                bullets[i].gameObject.SetActive(true);
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
}