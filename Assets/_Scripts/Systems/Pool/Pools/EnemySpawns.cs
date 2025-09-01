using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class EnemySpawns : PoolBaseController
{
    public static EnemySpawns Instance;

    [ReadOnly] public EnemyController[] enemys;

    private CancellationToken cancellationToken;
    private float _groupRandom = 0;
    private float random = 0;
    private float newRandom = 0;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        Instance = null;
    }

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

        _groupRandom = Random.Range(GameManager.PlayerLimits.z, GameManager.PlayerLimits.w);

        for (int i = 0; i < _group.count; i++)
        {
            _group.ship.spawnIndex = i;
            await UniTask.Yield();

            newRandom = Random.Range(GameManager.PlayerLimits.z, GameManager.PlayerLimits.w);

            if (Mathf.Abs(newRandom - random) <= 0.2f)
            {
                newRandom -= Mathf.Sign(newRandom) * 0.4f;
            }

            random = newRandom;

            if (_group.spawnType == Enums.SpawnType.Uniform)
            {
                InitEnemy(_group.ship, _group.spawnType, random, _groupRandom, GameManager.PlayerLimits.z + (i / (float)(_group.count - 1)) * (GameManager.PlayerLimits.w - GameManager.PlayerLimits.z));
            }
            else
            {
                InitEnemy(_group.ship, _group.spawnType, random, _groupRandom, _group.customFloat);
            }

            await UniTask.WaitForSeconds(Random.Range(_group.minTimeBetweenSpawn, _group.maxTimeBetweenSpawn), cancellationToken: cancellationToken);
        }
    }

    public void InitEnemy(ShipScriptable properties, Enums.SpawnType spawn, float random = 0, float groupRandom = 0, float customValue = 0)
    {
        for (int i = 0; i < size; i++)
        {
            if (!enemys[i].gameObject.activeSelf)
            {
                switch (spawn)
                {
                    case Enums.SpawnType.Random:
                        enemys[i].transform.position = new Vector2(random, GameManager.BoundsLimits.x);
                        break;

                    case Enums.SpawnType.RandomPoint:
                        enemys[i].transform.position = new Vector2(groupRandom, GameManager.BoundsLimits.x);
                        break;

                    case Enums.SpawnType.Center:
                        enemys[i].transform.position = new Vector2(0, GameManager.BoundsLimits.x);
                        break;

                    case Enums.SpawnType.Uniform:
                        enemys[i].transform.position = new Vector2(customValue, GameManager.BoundsLimits.x);
                        break;

                    case Enums.SpawnType.Specific:
                        enemys[i].transform.position = new Vector2(customValue, GameManager.BoundsLimits.x);
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