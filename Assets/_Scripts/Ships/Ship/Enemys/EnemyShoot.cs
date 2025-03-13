using UnityEngine;

public class EnemyShoot : MonoBehaviour
{
    [SerializeField] private Transform rotationRoot;

    private ShipScriptable properties;
    private Transform root1;
    private Transform root2;
    private Transform root3;
    private Transform root4;
    private Transform root5;

    public void Init(ShipScriptable _properties, Transform _root1, Transform _root2, Transform _root3, Transform _root4, Transform _root5)
    {
        properties = _properties;
        root1 = _root1;
        root2 = _root2;
        root3 = _root3;
        root4 = _root4;
        root5 = _root5;

        rotationRoot.localEulerAngles = new Vector3(0, 0, properties.startRotation);
    }

    public void Shoot()
    {
        switch (properties.attack)
        {
            case Enums.Attack.Unique:
                Unique(properties, root2);
                break;

            case Enums.Attack.Double:
                Double(properties, root1, root3);
                break;

            case Enums.Attack.None:
                break;

            case Enums.Attack.Triple:
                Triple(properties, root1, root2, root3);
                break;

            case Enums.Attack.Horizontal:
                Horizontal(properties, root2);
                break;

            case Enums.Attack.HorizontalDouble:
                HorizontalDouble(properties, root4, root5);
                break;

            case Enums.Attack.HorizontalTriple:
                HorizontalTriple(properties, root2, root4, root5);
                break;

            case Enums.Attack.Triangle:
                Triangle(properties, root1, root4, root3);
                break;

            case Enums.Attack.Xshaped:
                Xshaped(properties, root2);
                break;
        }
    }

    private void Update()
    {
        if (properties.rotationSpeed != 0)
        {
            rotationRoot.Rotate(Vector3.forward, properties.rotationSpeed * Time.deltaTime, Space.Self);
        }
    }

    private void Unique(ShipScriptable properties, Transform root)
    {
        BulletsPool.Instance.InitBullet(properties, Enums.TypeBullet.enemy,
            (root.position, root.rotation));
    }

    private void Double(ShipScriptable properties, Transform root1, Transform root3)
    {
        BulletsPool.Instance.InitBullet(properties, Enums.TypeBullet.enemy,
            (root1.position, root1.rotation),
            (root3.position, root3.rotation));
    }

    private void Triple(ShipScriptable properties, Transform root1, Transform root2, Transform root3)
    {
        BulletsPool.Instance.InitBullet(properties, Enums.TypeBullet.enemy,
            (root1.position, root1.rotation),
            (root2.position, root2.rotation),
            (root3.position, root3.rotation));
    }

    private void Horizontal(ShipScriptable properties, Transform root)
    {
        BulletsPool.Instance.InitBullet(properties, Enums.TypeBullet.enemy,
            (root.position, new Vector3(root.eulerAngles.x, root.eulerAngles.y, root.eulerAngles.z - 90)),
            (root.position, new Vector3(root.eulerAngles.x, root.eulerAngles.y, root.eulerAngles.z + 90)));
    }

    private void HorizontalDouble(ShipScriptable properties, Transform root4, Transform root5)
    {
        BulletsPool.Instance.InitBullet(properties, Enums.TypeBullet.enemy,
            (root4.position, new Vector3(root4.eulerAngles.x, root4.eulerAngles.y, root4.eulerAngles.z - 90)),
            (root4.position, new Vector3(root4.eulerAngles.x, root4.eulerAngles.y, root4.eulerAngles.z + 90)),
            (root5.position, new Vector3(root5.eulerAngles.x, root5.eulerAngles.y, root5.eulerAngles.z - 90)),
            (root5.position, new Vector3(root5.eulerAngles.x, root5.eulerAngles.y, root5.eulerAngles.z + 90)));
    }

    private void HorizontalTriple(ShipScriptable properties, Transform root2, Transform root4, Transform root5)
    {
        BulletsPool.Instance.InitBullet(properties, Enums.TypeBullet.enemy,
            (root4.position, new Vector3(root4.eulerAngles.x, root4.eulerAngles.y, root4.eulerAngles.z - 90)),
            (root4.position, new Vector3(root4.eulerAngles.x, root4.eulerAngles.y, root4.eulerAngles.z + 90)),
            (root2.position, new Vector3(root2.eulerAngles.x, root2.eulerAngles.y, root2.eulerAngles.z - 90)),
            (root2.position, new Vector3(root2.eulerAngles.x, root2.eulerAngles.y, root2.eulerAngles.z + 90)),
            (root5.position, new Vector3(root5.eulerAngles.x, root5.eulerAngles.y, root5.eulerAngles.z - 90)),
            (root5.position, new Vector3(root5.eulerAngles.x, root5.eulerAngles.y, root5.eulerAngles.z + 90)));
    }

    private void Triangle(ShipScriptable properties, Transform root1, Transform root4, Transform root3)
    {
        BulletsPool.Instance.InitBullet(properties, Enums.TypeBullet.enemy,
            (root1.position, new Vector3(root1.eulerAngles.x, root1.eulerAngles.y, root1.eulerAngles.z + 45)),
            (root4.position, new Vector3(root4.eulerAngles.x, root4.eulerAngles.y, root4.eulerAngles.z + 180)),
            (root3.position, new Vector3(root3.eulerAngles.x, root3.eulerAngles.y, root3.eulerAngles.z - 45)));
    }

    private void Xshaped(ShipScriptable properties, Transform root2)
    {
        BulletsPool.Instance.InitBullet(properties, Enums.TypeBullet.enemy,
            (root2.position, new Vector3(root2.eulerAngles.x, root2.eulerAngles.y, root2.eulerAngles.z - 45)),
            (root2.position, new Vector3(root2.eulerAngles.x, root2.eulerAngles.y, root2.eulerAngles.z + 45)),
            (root2.position, new Vector3(root2.eulerAngles.x, root2.eulerAngles.y, root2.eulerAngles.z - 135)),
            (root2.position, new Vector3(root2.eulerAngles.x, root2.eulerAngles.y, root2.eulerAngles.z + 135)));
    }
}