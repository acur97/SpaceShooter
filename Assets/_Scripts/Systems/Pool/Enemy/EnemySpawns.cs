using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class EnemySpawns : PoolBaseController
{
    public static EnemySpawns Instance;

    [ReadOnly] public EnemyController[] enemys;

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

            if (_group.minTimeBetweenSpawn > 0 || _group.maxTimeBetweenSpawn > 0 || _group.spawnType != Enums.SpawnType.AllAtOnce)
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
                    case Enums.SpawnType.Random:
                        enemys[i].transform.position = new Vector2(random, GameManager.BoundsLimits.y);
                        break;

                    case Enums.SpawnType.RandomPoint:
                        enemys[i].transform.position = new Vector2(groupRandom, GameManager.BoundsLimits.y);
                        break;

                    case Enums.SpawnType.Center:
                        enemys[i].transform.position = new Vector2(0, GameManager.BoundsLimits.y);
                        break;

                    case Enums.SpawnType.AllAtOnce:
                        enemys[i].transform.position = new Vector2(random, GameManager.BoundsLimits.y);
                        break;

                    case Enums.SpawnType.Specific:
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