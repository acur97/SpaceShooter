using UnityEngine;

[CreateAssetMenu(fileName = "PowerUp_LifeSteal", menuName = "Gameplay/PowerUps/LifeSteal", order = 9)]
public class PowerUp_LifeSteal : PowerUpBase
{
    [Space]
    [SerializeField] private GameObject prefab;
    [SerializeField, Range(0f, 1f)] private float maxLifeSteal = 0.1f;

    private int maxPercent;
    private int percent = 0;
    private uint lifeToAdd;

    public PowerUp_LifeSteal() : base()
    {
        type = PowerUpsManager.PowerUpType.LifeSteal;
        useDuration = true;
        durationRange = new Vector2(10f, 10f);
    }

    public override void OnActivate()
    {
        switch (RoundsController.Instance.levelType)
        {
            case RoundsController.LevelType.Normal:
                lifeToAdd = GameManager.Instance.gameplayScriptable.playerHealth;
                break;

            case RoundsController.LevelType.Infinite:
                lifeToAdd = GameManager.Instance.gameplayScriptable.playerHealthInfinite;
                break;
        }

        maxPercent = (int)(lifeToAdd * maxLifeSteal);
    }

    public override void OnDeactivate()
    {

    }

    public override void OnEnemyDamage(ShipBaseController enemy)
    {
        if (shipBase.health < lifeToAdd)
        {
            shipBase.health++;
            percent++;

            PlayerController.Instance.UpdateHealthUi();
            Object.Instantiate(prefab, PlayerController.Instance.transform);

            if (percent >= maxPercent)
            {
                PowerUpsManager.Instance.RemovePowerUp(this);
            }
        }
    }

    public override void OnEnemyDeath(ShipBaseController enemy)
    {

    }

    public override void OnGameUpdate()
    {

    }

    public override void OnPlayerDamage()
    {

    }

    public override void OnPlayerShoot()
    {

    }
}