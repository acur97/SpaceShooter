using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawns : MonoBehaviour
{
    public static EnemySpawns Instance { get; private set; }

    [SerializeField] private int size = 100;
    [SerializeField] private GameObject prefab;
    [SerializeField] private List<GameObject> enemys = new();
    [SerializeField] private List<EnemyController> bulletsC = new();
    private float groupRandom = 0;

    private void Awake()
    {
        Instance = this;

        for (int i = 0; i < size; i++)
        {
            GameObject ob = Instantiate(prefab, transform);
            bulletsC.Add(ob.GetComponent<EnemyController>());
            enemys.Add(ob);
            ob.SetActive(false);
        }
    }

    public IEnumerator InstantiateEnemys(Group _group)
    {
        if (_group.timeToStart > 0)
        {
            yield return new WaitForSeconds(_group.timeToStart);
        }

        Debug.LogWarning("Grupo con enemigos tipo: " + _group.ship.name);
        Debug.LogWarning(_group.spawnType);

        groupRandom = Random.Range(-4.5f, 4.5f);
        for (int i = 0; i < _group.count; i++)
        {
            InitEnemy(_group.ship, _group, Random.Range(-4.5f, 4.5f));

            if (_group.minTimeBetweenSpawn > 0 || _group.maxTimeBetweenSpawn > 0 || _group.spawnType != Group.SpawnType.allAtOnce)
            {
                yield return new WaitForSeconds(Random.Range(_group.minTimeBetweenSpawn, _group.maxTimeBetweenSpawn));
            }
        }
    }

    public void InitEnemy(ShipScriptable properties, Group group, float random)
    {
        for (int i = 0; i < (group.spawnType == Group.SpawnType.only ? 1 : size); i++)
        {
            if (!enemys[i].activeSelf)
            {
                switch (group.spawnType)
                {
                    case Group.SpawnType.random:
                        enemys[i].transform.position = new Vector2(random, 3.3f);
                        break;

                    case Group.SpawnType.row:
                        enemys[i].transform.position = new Vector2(groupRandom, 3.3f);
                        break;

                    case Group.SpawnType.only:
                        enemys[i].transform.position = new Vector2(random, 3.3f);
                        break;

                    case Group.SpawnType.allAtOnce:
                        enemys[i].transform.position = new Vector2(random, 3.3f);
                        break;

                    default:
                        break;
                }

                bulletsC[i]._properties = properties;
                enemys[i].SetActive(true);
                break;
            }
            else
            {
                continue;
            }
        }
    }
}