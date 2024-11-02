using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private PlayerController controller;
    private ControlsManager controls;

    public float movePowerUp = 1;

    private bool inited = false;
    private Vector3 inputMove;
    private Vector2 randomOffset;

    public void Init(PlayerController _controller, ControlsManager _controls, float _randomOffset = 0.6f)
    {
        inited = true;

        controller = _controller;
        controls = _controls;

        randomOffset = new Vector2(
            Random.Range(0, 2) == 0 ? -_randomOffset : _randomOffset,
            Random.Range(0, 2) == 0 ? -_randomOffset : _randomOffset);
    }

    public void OnUpdate()
    {
        if (!inited)
        {
            return;
        }

        if (!controller.copy)
        {
            inputMove = new Vector2(controls.move.x * controller._properties.speed * Time.deltaTime * movePowerUp, controls.move.y * controller._properties.speed * Time.deltaTime * movePowerUp);
        }
        else
        {
            transform.position = new Vector2(
                PlayerController.Instance.transform.position.x + randomOffset.x,
                PlayerController.Instance.transform.position.y + randomOffset.y);
        }

        ClampPosition();
    }

    public void ClampPosition()
    {
        transform.localPosition = new Vector2(
            Mathf.Clamp(transform.localPosition.x + inputMove.x, -GameManager.PlayerLimits.x, GameManager.PlayerLimits.x),
            Mathf.Clamp(transform.localPosition.y + inputMove.y, -GameManager.PlayerLimits.y, GameManager.PlayerLimits.y));
    }
}