using UnityEngine;

public class PowerUpsPool : PoolBaseController
{
    public static PowerUpsPool Instance;

    [ReadOnly, SerializeField] private PowerUpCollectable[] powerUps;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        Instance = null;
    }

    private new void Awake()
    {
        Instance = this;

        //objects = new GameObject[size];
        powerUps = new PowerUpCollectable[size];

        for (int i = 0; i < size; i++)
        {
            powerUps[i] = Instantiate(prefab, transform).GetComponent<PowerUpCollectable>();
        }
    }

    public void InitPowerUp(PowerUpBase powerUp)
    {
        for (int i = 0; i < size; i++)
        {
            if (!powerUps[i].gameObject.activeSelf)
            {
                powerUps[i].Init(powerUp);
                break;
            }
        }
    }
}