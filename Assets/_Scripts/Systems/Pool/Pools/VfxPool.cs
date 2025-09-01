using UnityEngine;

public class VfxPool : PoolBaseController
{
    public static VfxPool Instance;

    [Space]
    [SerializeField] private float miniOffset = 0.2f;

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

    public void InitVfx(Vector2 _position)
    {
        for (int i = 0; i < size; i++)
        {
            if (!objects[i].activeSelf)
            {
                objects[i].transform.localPosition = new Vector2(_position.x + Random.Range(-miniOffset, miniOffset), _position.y + Random.Range(-miniOffset, miniOffset));
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