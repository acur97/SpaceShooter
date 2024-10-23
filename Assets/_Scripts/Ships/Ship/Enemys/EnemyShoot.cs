using UnityEngine;

public class EnemyShoot : MonoBehaviour
{
    public void Shoot(Transform root, ShipScriptable properties)
    {
        switch (properties.attack)
        {
            case ShipScriptable.Attack.continuous:
                Continuous(properties, root);
                break;

            case ShipScriptable.Attack.continuousDouble:
                ContinuousDouble(properties, root);
                break;

            case ShipScriptable.Attack.none:
                break;
        }
    }

    private void Continuous(ShipScriptable properties, Transform root)
    {
        BulletsPool.Instance.InitBullet(root, properties.bulletSpeed, false, Bullet.TypeBullet.enemy);
    }

    private void ContinuousDouble(ShipScriptable properties, Transform root)
    {
        BulletsPool.Instance.InitBullet(root, properties.bulletSpeed, true, Bullet.TypeBullet.enemy);
    }
}