using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    private PlayerController controller;
    private ControlsManager controls;

    public bool shootPowerUp = false;

    private bool inited = false;
    private bool hold = false;

    public void Init(PlayerController _controller, ControlsManager _controls)
    {
        inited = true;

        controller = _controller;
        controls = _controls;
    }

    public void OnUpdate()
    {
        if (!inited)
        {
            return;
        }

        if (controls.fireDown)
        {
            hold = true;
            controller.timer = controller._properties.coolDown;
        }
        if (controls.fireUp)
        {
            hold = false;
            controller.timer = -1;
        }

        if (hold && controller.timer >= 0)
        {
            if (controller.timer == controller._properties.coolDown)
            {
                Shoot();
            }

            controller.timer -= Time.deltaTime;

            if (controller.timer <= 0)
            {
                controller.timer = controller._properties.coolDown;
            }
        }
    }

    private void Shoot()
    {
        PowerUpsManager.Player_Shoot?.Invoke();

        if (shootPowerUp)
        {
            return;
        }

        BulletsPool.Instance.InitBullet(controller.shootRoot, controller._properties.bulletSpeed, false, Bullet.TypeBullet.player);
    }
}