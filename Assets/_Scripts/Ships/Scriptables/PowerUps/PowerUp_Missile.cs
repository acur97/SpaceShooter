using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using Object = UnityEngine.Object;

public class PowerUp_Missile : PowerUpBase
{
    private int count = 2;

    private GameObject missileObject;
    private Missile missile;
    private Missile missileInstance;

    private Action onUpdate;

    public PowerUp_Missile() : base()
    {
        type = PowerUpsManager.PowerUpType.Missile;
    }

    public override void OnActivate()
    {
        PlayerController.Instance.shoot.shootPowerUp = true;
        LoadPrefab().Forget();
    }

    private async UniTaskVoid LoadPrefab()
    {
        missileObject = Object.Instantiate(await Resources.LoadAsync("Power Bullets/Missile") as GameObject);
        missileObject.SetActive(false);

        missile = missileObject.AddComponent<Missile>();
    }

    public override void OnDeactivate()
    {
        PlayerController.Instance.shoot.shootPowerUp = false;
    }

    public override void OnEnemyDamage(ShipBaseController enemy)
    {

    }

    public override void OnEnemyDeath(ShipBaseController enemy)
    {

    }

    public override void OnGameUpdate()
    {
        if (missileObject == null)
        {
            return;
        }

        onUpdate?.Invoke();
    }

    public override void OnPlayerDamage()
    {

    }

    public override void OnPlayerShoot()
    {
        missileInstance = Object.Instantiate(missileObject).GetComponent<Missile>();
        onUpdate += missileInstance.OnUpdate;
        missileInstance.gameObject.SetActive(true);

        // El movimiento del misil no se mueve o no se suscribe o algo :´v

        count--;
        if (count == 0)
        {
            PowerUpsManager.Instance.RemovePowerUp(this);
        }
    }
}

public class Missile : MonoBehaviour
{
    public void OnUpdate()
    {
        if (!gameObject.activeSelf)
        {
            return;
        }

        transform.position = transform.forward * Time.deltaTime;
    }
}