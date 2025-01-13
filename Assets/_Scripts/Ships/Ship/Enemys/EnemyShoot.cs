using UnityEngine;

public class EnemyShoot : MonoBehaviour
{
    public void Shoot(Transform root1, Transform root2, Transform root3, ShipScriptable properties)
    {
        switch (properties.attack)
        {
            case Enums.Attack.Continuous:
                Continuous(properties, root2);
                break;

            case Enums.Attack.ContinuousDouble:
                ContinuousDouble(properties, root1, root3);
                break;
        }
    }

    private void Continuous(ShipScriptable properties, Transform root)
    {
        BulletsPool.Instance.InitBullet(root, properties, Enums.TypeBullet.enemy);
    }

    private void ContinuousDouble(ShipScriptable properties, Transform root1, Transform root2)
    {
        BulletsPool.Instance.InitBullet(root1, root2, properties, Enums.TypeBullet.enemy);
    }
}