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
                    //agarrar desde el gamemanager
                    bulletsC[i].renderer.material.SetColor(_Color, Color.green);
                }
                else
                {
                    //el 180 invertido color del usuario
                    bulletsC[i].renderer.material.SetColor(_Color, Color.red);
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