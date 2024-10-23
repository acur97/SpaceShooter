using UnityEngine;

public abstract class PoolBaseController : MonoBehaviour
{
    public int size = 10;
    public GameObject prefab;
    public GameObject[] objects;

    protected virtual void Awake()
    {
        objects = new GameObject[size];

        for (int i = 0; i < size; i++)
        {
            objects[i] = Instantiate(prefab, transform);
        }
    }
}