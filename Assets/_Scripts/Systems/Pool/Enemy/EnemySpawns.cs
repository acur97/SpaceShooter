using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class EnemySpawns : PoolBaseController
{
    public static EnemySpawns Instance;

    public EnemyController[] enemys;

    private CancellationToken cancellationToken;

    private new void Awake()
    {
        Instance = this;
        cancellationToken = this.GetCancellationTokenOnDestroy();

        //objects = new GameObject[size];
        enemys = new EnemyController[size];

        for (int i = 0; i < size; i++)
        {
            enemys[i] = Instantiate(prefab, transform).GetComponent<EnemyController>();
        }
    }

    public async UniTaskVoid InstantiateEnemys(Group _group)
    {
        if (_group.timeToStart > 0)
        {
            await UniTask.WaitForSeconds(_group.timeToStart, cancellationToken: cancellationToken);
        }

        //Debug.LogWarning($"Group with enemies type: {_group.ship.name}");
        //Debug.LogWarning(_group.spawnType);

        float _groupRandom = Random.Range(-GameManager.PlayerLimits.x, GameManager.PlayerLimits.x);
        for (int i = 0; i < _group.count; i++)
        {
            _group.ship.spawnIndex = i;
            InitEnemy(_group.ship, _group, Random.Range(-GameManager.PlayerLimits.x, GameManager.PlayerLimits.x), _groupRandom);

            if (_group.minTimeBetweenSpawn > 0 || _group.maxTimeBetweenSpawn > 0 || _group.spawnType != Group.SpawnType.allAtOnce)
            {
                await UniTask.WaitForSeconds(Random.Range(_group.minTimeBetweenSpawn, _group.maxTimeBetweenSpawn), cancellationToken: cancellationToken);
            }
        }
    }

    public void InitEnemy(ShipScriptable properties, Group group, float random, float groupRandom)
    {
        for (int i = 0; i < size; i++)
        {
            if (!enemys[i].gameObject.activeSelf)
            {
                switch (group.spawnType)
                {
                    case Group.SpawnType.random:
                        enemys[i].transform.position = new Vector2(random, GameManager.BoundsLimits.y);
                        break;

                    case Group.SpawnType.randomPoint:
                        enemys[i].transform.position = new Vector2(groupRandom, GameManager.BoundsLimits.y);
                        break;

                    case Group.SpawnType.center:
                        enemys[i].transform.position = new Vector2(0, GameManager.BoundsLimits.y);
                        break;

                    case Group.SpawnType.allAtOnce:
                        enemys[i].transform.position = new Vector2(random, GameManager.BoundsLimits.y);
                        break;

                    case Group.SpawnType.specific:
                        enemys[i].transform.position = new Vector2(group.customFloat, GameManager.BoundsLimits.y);
                        break;
                }

                enemys[i]._properties = properties;
                enemys[i].gameObject.SetActive(true);
                break;
            }
            else
            {
                continue;
            }
        }
    }
}