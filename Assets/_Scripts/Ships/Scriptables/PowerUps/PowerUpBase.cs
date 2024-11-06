using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class PowerUpBase : ScriptableObject
{
    [HideInInspector] public ShipBaseController shipBase;
    public PowerUpsManager.PowerUpType type;
    public uint cost = 1;
    public bool useDuration = false;
    [ReadOnly] public float duration = -1f;
    public Vector2 durationRange = new(-1f, -1f);

    //public PowerUpBase()
    //{
    //    shipBase = PlayerController.Instance;
    //}

    public abstract void OnActivate();

    public async UniTaskVoid CountDuration()
    {
        duration = Random.Range(durationRange.x, durationRange.y);

        while (duration > 0 && shipBase != null)
        {
            duration -= Time.deltaTime;

            if (duration <= 0)
            {
                PowerUpsManager.Instance.RemovePowerUp(this);
                break;
            }

            await UniTask.Yield();
        }
    }

    public abstract void OnDeactivate();

    public abstract void OnPlayerShoot();

    public abstract void OnPlayerDamage();

    public abstract void OnEnemyDamage(ShipBaseController enemy);

    public abstract void OnEnemyDeath(ShipBaseController enemy);

    public abstract void OnGameUpdate();

    public virtual void Dispose()
    {
        shipBase = null;
    }
}