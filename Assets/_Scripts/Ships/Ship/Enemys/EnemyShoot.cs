using UnityEngine;

public class EnemyShoot : MonoBehaviour
{
    public void Shoot(Transform root1, Transform root2, Transform root3, ShipScriptable properties)
    {
        switch (properties.attack)
        {
            case ShipScriptable.Attack.continuous:
                Continuous(properties, root2);
                break;

            case ShipScriptable.Attack.continuousDouble:
                ContinuousDouble(properties, root1, root3);
                break;

            case ShipScriptable.Attack.none:
                break;
        }
    }

    private void Continuous(ShipScriptable properties, Transform root)
    {
        BulletsPool.Instance.InitBullet(root, properties, Bullet.TypeBullet.enemy);
    }

    private void ContinuousDouble(ShipScriptable properties, Transform root1, Transform root2)
    {
        BulletsPool.Instance.InitBullet(root1, root2, properties, Bullet.TypeBullet.enemy);
    }
}