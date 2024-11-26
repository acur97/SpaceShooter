using UnityEngine;

public class BulletsPool : PoolBaseController
{
    public static BulletsPool Instance;

    public Bullet[] bullets;

    private new void Awake()
    {
        Instance = this;

        //objects = new GameObject[size];
        bullets = new Bullet[size];

        for (int i = 0; i < size; i++)
        {
            bullets[i] = Instantiate(prefab, transform).GetComponent<Bullet>();
            bullets[i].renderer.material.SetColor(Types.material_Color, Color.white);
        }
    }

    public void InitBullet(Transform _position, ShipScriptable _properties, Bullet.TypeBullet type)
    {
        AudioManager.Instance.PlaySound(AudioManager.AudioType.Zap, 0.2f);

        Init(_position.position, _position.rotation, _properties, type);
    }

    public void InitBullet(Transform _position1, Transform _position2, ShipScriptable _properties, Bullet.TypeBullet type)
    {
        AudioManager.Instance.PlaySound(AudioManager.AudioType.Zap, 0.2f);

        Init(_position1.position, _position1.rotation, _properties, type);
        Init(_position2.position, _position2.rotation, _properties, type);
    }

    private void Init(Vector2 _position, Quaternion _rotation, ShipScriptable _properties, Bullet.TypeBullet type)
    {
        for (int i = 0; i < size; i++)
        {
            if (!bullets[i].gameObject.activeSelf)
            {
                bullets[i].transform.SetPositionAndRotation(_position, _rotation);
                bullets[i].speed = _properties.bulletSpeed;
                bullets[i].bulletType = type;

                if (type == Bullet.TypeBullet.player)
                {
                    bullets[i].renderer.material.SetColor(Types.material_Color, PlayerController.Instance._properties.color);
                }
                else
                {
                    bullets[i].renderer.material.SetColor(Types.material_Color, SetColor(PlayerController.Instance._properties.color));
                }

                if (_properties._bulletTime)
                {
                    bullets[i].SetLifetime(_properties.bulletTime);
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