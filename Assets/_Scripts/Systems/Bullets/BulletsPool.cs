using System.Collections.Generic;
using UnityEngine;

public class BulletsPool : MonoBehaviour
{
    public static BulletsPool Instance { get; private set; }

    [SerializeField] private int size = 100;
    [SerializeField] private GameObject prefab;
    [SerializeField] private List<GameObject> bullets = new();
    [SerializeField] private List<Bullet> bulletsC = new();

    private readonly int _Color = Shader.PropertyToID("_Color");

    private void Awake()
    {
        Instance = this;

        for (int i = 0; i < size; i++)
        {
            GameObject ob = Instantiate(prefab, transform);
            bulletsC.Add(ob.GetComponent<Bullet>());
            bullets.Add(ob);
            ob.SetActive(false);
        }
    }

    public void InitBullet(Transform _position, float _speed, bool _double, Bullet.TypeBullet type)
    {
        if (_double)
        {
            Init2(new Vector2(_position.position.x + 0.157f, _position.position.y), _position.rotation, _speed, type);
            Init2(new Vector2(_position.position.x - 0.157f, _position.position.y), _position.rotation, _speed, type);
        }
        else
        {
            Init2(_position.position, _position.rotation, _speed, type);
        }
    }

    private void Init2(Vector2 _position, Quaternion _rotation, float _speed, Bullet.TypeBullet type)
    {
        for (int i = 0; i < size; i++)
        {
            if (!bullets[i].activeSelf)
            {
                bullets[i].transform.SetPositionAndRotation(_position, _rotation);
                bulletsC[i].speed = _speed;
                bulletsC[i].bulletType = type;

                if (type == Bullet.TypeBullet.player)
                {
                    bulletsC[i].renderer.material.SetColor(_Color, PlayerController.Instance._properties.color);
                }
                else
                {
                    Color.RGBToHSV(PlayerController.Instance._properties.color, out float h, out float s, out float v);
                    h += 0.5f;
                    if (h > 1)
                    {
                        h--;
                    }
                    Color color = Color.HSVToRGB(h, s, v);
                    bulletsC[i].renderer.material.SetColor(_Color, color);
                }
                bullets[i].SetActive(true);
                break;
            }
            else
            {
                continue;
            }
        }
    }
}