//using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class PowerUp_MissileBehaviour : MonoBehaviour
{
    [SerializeField] private AimConstraint aim;

    private int damage = 0;
    private float initialSpeed;
    private float speed = 0f;
    private float speedUp = 0f;
    //private LayerMask defaultLayer;
    //private Collider2D[] objects;
    private ConstraintSource source;

    public void Init(int _damage, float _speed, float _speedUp)
    {
        damage = _damage;
        initialSpeed = _speed;
        speed = _speed;
        speedUp = _speedUp;

        //defaultLayer = LayerMask.GetMask("Default");

        SetAimSource();
    }

    private void SetAimSource()
    {
        speed = initialSpeed;

        if (aim.sourceCount > 0)
        {
            aim.RemoveSource(0);
        }

        //objects = Physics2D.OverlapCircleAll(transform.position, 25f, defaultLayer);

        //for (int i = 0; i < objects.Length; i++)
        //{
        //    if (objects[i].gameObject.activeSelf && objects[i].CompareTag(_Enemy))
        //    {
        //        Debug.LogWarning($"source selected: {i} - {objects[i].name}", objects[i].gameObject);
        //        source.sourceTransform = objects[i].transform;
        //        source.weight = 1f;

        //        aim.AddSource(source);
        //        break;
        //    }
        //}

        int strongestEnemyIndex = -1;

        for (int i = EnemySpawns.Instance.enemys.Length - 1; i >= 0; i--)
        {
            if (!EnemySpawns.Instance.enemys[i].gameObject.activeSelf)
            {
                continue;
            }

            if (strongestEnemyIndex == -1)
            {
                strongestEnemyIndex = i;
                continue;
            }

            if (EnemySpawns.Instance.enemys[i]._properties.health > EnemySpawns.Instance.enemys[strongestEnemyIndex]._properties.health)
            {
                strongestEnemyIndex = i;
            }
        }

        if (strongestEnemyIndex == -1)
        {
            return;
        }

        source.sourceTransform = EnemySpawns.Instance.enemys[strongestEnemyIndex].transform;
        source.weight = 1f;

        aim.AddSource(source);
    }

    public void OnUpdate()
    {
        if (!gameObject.activeSelf)
        {
            return;
        }

        if (source.sourceTransform == null ||
            !source.sourceTransform.gameObject.activeSelf)
        {
            SetAimSource();
            return;
        }

        transform.position += speed * Time.deltaTime * transform.up;
        speed += speedUp;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Types.tag_Enemy) && collision.TryGetComponent(out EnemyController enemyController))
        {
            PostProcessingController.Instance.VolumePunch();
            VfxPool.Instance.InitVfx(transform.position);
            VfxPool.Instance.InitVfx(collision.transform.position);

            enemyController.DoDamage(damage);
            PoolsUpdateManager.PoolUpdate -= OnUpdate;
            Destroy(gameObject);
        }
    }
}