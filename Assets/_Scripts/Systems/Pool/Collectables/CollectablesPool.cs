using UnityEngine;

public class CollectablesPool : PoolBaseController
{
    [Space]
    [SerializeField] private float speed = 1;
    [SerializeField] private float minProv = 1;
    [SerializeField] private float maxProv = 5;

    private float timer = 0;

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
                    if (objects[i].transform.position.y <= -3.3f)
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
                objects[i].transform.localPosition = new Vector2(Random.Range(-4.5f, 4.5f), 3.3f);
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