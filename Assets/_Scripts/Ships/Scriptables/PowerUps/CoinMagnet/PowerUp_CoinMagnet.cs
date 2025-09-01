using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PowerUp_CoinMagnet", menuName = "Gameplay/PowerUps/CoinMagnet", order = 16)]
public class PowerUp_CoinMagnet : PowerUpBase
{
    [Space]
    [SerializeField] private float range = 2f;
    [SerializeField] private float attractionSpeed = 1f;
    [SerializeField] private GameObject prefab;

    private ParticleSystem particles;
    private ParticleSystem.EmissionModule emissionModule;
    private ParticleSystem.ShapeModule shapeModule;

    private int collectables = 0;
    private ContactFilter2D contactFilter = new();
    private readonly List<Collider2D> colliders = new();

    public PowerUp_CoinMagnet() : base()
    {
        type = PowerUpsManager.PowerUpType.CoinMagnet;
        useDuration = true;
        durationRange = new Vector2(10f, 10f);
    }

    public override void OnActivate()
    {
        contactFilter.useTriggers = true;
        contactFilter.useLayerMask = true;
        contactFilter.layerMask = Layers.Collectable;

        particles = Instantiate(prefab, PlayerController.Instance.transform).GetComponent<ParticleSystem>();

        emissionModule = particles.emission;
        emissionModule.rateOverTime = 100 * range;
        shapeModule = particles.shape;
        shapeModule.radius = range;
    }

    public override void OnDeactivate()
    {
        particles.Stop(false, ParticleSystemStopBehavior.StopEmitting);
    }

    public override void OnEnemyDamage(ShipBaseController enemy)
    {

    }

    public override void OnEnemyDeath(ShipBaseController enemy)
    {

    }

    public override void OnGameUpdate()
    {
        collectables = Physics2D.OverlapCircle(PlayerController.Instance.transform.position, range, contactFilter, colliders);

        for (int i = 0; i < collectables; i++)
        {
            colliders[i].transform.position = Vector2.MoveTowards(colliders[i].transform.position, PlayerController.Instance.transform.position, attractionSpeed * Time.deltaTime);
        }
    }

    public override void OnPlayerDamage()
    {

    }

    public override void OnPlayerShoot()
    {

    }
}