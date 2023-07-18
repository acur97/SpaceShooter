using System.Collections.Generic;
using UnityEngine;

public class VfxPool : MonoBehaviour
{
    public static VfxPool Instance { get; private set; }

    [SerializeField] private int size = 10;
    [SerializeField] private GameObject prefab;
    [SerializeField] private List<GameObject> vfxs = new();

    private void Awake()
    {
        Instance = this;

        for (int i = 0; i < size; i++)
        {
            GameObject ob = Instantiate(prefab, transform);
            vfxs.Add(ob);
            ob.SetActive(false);
        }
    }

    public void InitVfx(Transform _position)
    {
        for (int i = 0; i < size; i++)
        {
            if (!vfxs[i].activeSelf)
            {
                vfxs[i].transform.localPosition = new Vector2(_position.position.x + Random.Range(-0.2f, 0.2f), _position.position.y + Random.Range(-0.2f, 0.2f));
                vfxs[i].SetActive(true);
                break;
            }
            else
            {
                continue;
            }
        }
    }
}