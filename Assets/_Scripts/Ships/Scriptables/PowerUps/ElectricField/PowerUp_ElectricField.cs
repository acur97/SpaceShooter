using UnityEngine;

[CreateAssetMenu(fileName = "PowerUp_ElectricField", menuName = "Gameplay/PowerUps/ElectricField", order = 5)]
public class PowerUp_ElectricField : PowerUpBase
{
    [Space]
    [SerializeField] private GameObject prefab;
    [SerializeField] private uint enemyDamage = 5;

    private GameObject electricShieldObject;
    private PowerUp_ElectricFieldBehaviour playerElectricShield;

    public PowerUp_ElectricField() : base()
    {
        type = PowerUpsManager.PowerUpType.Shield;
        useDuration = true;
        durationRange = new Vector2(10f, 10f);
    }

    public override void OnActivate()
    {
        electricShieldObject = Instantiate(prefab, shipBase.transform);
        playerElectricShield = electricShieldObject.GetComponent<PowerUp_ElectricFieldBehaviour>();

        playerElectricShield.Init((int)enemyDamage);
    }

    public override void OnDeactivate()
    {
        playerElectricShield.Disable().Forget();
        playerElectricShield = null;
    }

    public override void OnEnemyDamage(ShipBaseController enemy)
    {

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