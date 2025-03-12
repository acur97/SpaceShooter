using UnityEngine;

public class CollectablesPool : PoolBaseController
{
    public static CollectablesPool Instance;

    [Space]
    [SerializeField] private float speed = 1;
    [SerializeField] private float minProv = 1;
    [SerializeField] private float maxProv = 5;

    private float timer = 0;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        Instance = null;
    }

    protected override void Awake()
    {
        Instance = this;

        base.Awake();
    }

    private void Start()
    {
        timer = Random.Range(minProv, maxProv);
    }

    private void Update()
    {
        if (GameManager.Instance.isPlaying)
        {
            timer -= Time.deltaTime;

            if (timer < 0)
            {
                timer = Random.Range(minProv, maxProv);

                InitCoin();
            }

            for (int i = 0; i < size; i++)
            {
                if (objects[i].activeSelf)
                {
                    objects[i].transform.position -= speed * Time.deltaTime * transform.up;

                    if (objects[i].transform.position.y <= GameManager.BoundsLimits.y)
                    {
                        objects[i].SetActive(false);
                    }
                }
                else
                {
                    continue;
                }
            }
        }
    }

    private void InitCoin()
    {
        for (int i = 0; i < size; i++)
        {
            if (!objects[i].activeSelf)
            {
                objects[i].transform.localPosition = new Vector2(Random.Range(GameManager.PlayerLimits.z, GameManager.PlayerLimits.w), GameManager.BoundsLimits.x);
                objects[i].SetActive(true);
                break;
            }
            else
            {
                continue;
            }
        }
    }
}