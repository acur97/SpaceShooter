using UnityEngine;

public class VfxPool : PoolBaseController
{
    public static VfxPool Instance { get; private set; }

    protected override void Awake()
    {
        Instance = this;

        base.Awake();
    }

    public void InitVfx(Transform _position)
    {
        for (int i = 0; i < size; i++)
        {
            if (!objects[i].activeSelf)
            {
                objects[i].transform.localPosition = new Vector2(_position.position.x + Random.Range(-0.2f, 0.2f), _position.position.y + Random.Range(-0.2f, 0.2f));
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