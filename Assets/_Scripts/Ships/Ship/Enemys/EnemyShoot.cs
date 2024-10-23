using UnityEngine;

public class EnemyShoot : MonoBehaviour
{
    private ShipScriptable properties;
    private Transform root;

    public void Init(ShipScriptable properties, Transform root)
    {
        this.properties = properties;
        this.root = root;
    }

    public void Shoot()
    {
        switch (properties.attack)
        {
            case ShipScriptable.Attack.continuous:
                BulletsPool.Instance.InitBullet(root, properties.bulletSpeed, false, Bullet.TypeBullet.enemy);
                break;

            case ShipScriptable.Attack.continuousDouble:
                BulletsPool.Instance.InitBullet(root, properties.bulletSpeed, true, Bullet.TypeBullet.enemy);
                break;

            case ShipScriptable.Attack.none:
                break;
        }
    }
}