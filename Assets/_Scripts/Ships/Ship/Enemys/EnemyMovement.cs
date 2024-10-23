using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [HideInInspector] public float customFloat = 0;
    [HideInInspector] public bool customBool = false;
    [HideInInspector] public float _timeToContinue;

    private Vector2 startPosition;

    public void Init(ShipScriptable.Behaviour properties)
    {
        switch (properties)
        {
            case ShipScriptable.Behaviour.linear:
                break;

            case ShipScriptable.Behaviour.direct:
                break;

            case ShipScriptable.Behaviour.waves:
                if (transform.localPosition.x > 3)
                {
                    transform.localPosition = new Vector2(3, transform.localPosition.y);
                }
                else if (transform.localPosition.x < -3)
                {
                    transform.localPosition = new Vector2(-3, transform.localPosition.y);
                }
                startPosition = transform.localPosition;
                break;

            case ShipScriptable.Behaviour.wavesDirect:
                if (transform.localPosition.x > 3)
                {
                    transform.localPosition = new Vector2(3, transform.localPosition.y);
                }
                else if (transform.localPosition.x < -3)
                {
                    transform.localPosition = new Vector2(-3, transform.localPosition.y);
                }
                startPosition = transform.localPosition;
                break;

            case ShipScriptable.Behaviour.diagonal:
                if (transform.localPosition.x > 0)
                {
                    transform.localEulerAngles = new Vector3(0, 0, 225);
                }
                else
                {
                    transform.localEulerAngles = new Vector3(0, 0, 135);
                }
                break;

            case ShipScriptable.Behaviour.wave8:
                transform.localPosition = new Vector2(0, 3.3f);
                startPosition = transform.localPosition;
                break;
        }
    }

    public void Move(ShipScriptable properties)
    {
        if (_timeToContinue > 0)
        {
            _timeToContinue -= Time.deltaTime;

            if (_timeToContinue < 0)
            {
                _timeToContinue = 0;
            }
        }

        switch (properties.behaviour)
        {
            case ShipScriptable.Behaviour.linear:
                transform.position += properties.speed * Time.deltaTime * transform.up;
                break;

            case ShipScriptable.Behaviour.direct:
                if (transform.position.y > GameManager.EnemyLine || _timeToContinue == 0)
                {
                    transform.position += properties.speed * Time.deltaTime * transform.up;
                }
                break;

            case ShipScriptable.Behaviour.waves:
                transform.position += properties.speed * Time.deltaTime * transform.up;
                transform.localPosition = new Vector2(startPosition.x + Mathf.Sin(Time.time * GameManager.HorizontalInvertedMultiplier) * properties.behaviourMathfSin * GameManager.HorizontalMultiplier, transform.localPosition.y);
                break;

            case ShipScriptable.Behaviour.wavesDirect:
                if (transform.position.y > GameManager.EnemyLine || _timeToContinue == 0)
                {
                    transform.position += properties.speed * Time.deltaTime * transform.up;
                }

                transform.localPosition = new Vector2(startPosition.x + Mathf.Sin(Time.time * GameManager.HorizontalInvertedMultiplier) * properties.behaviourMathfSin * GameManager.HorizontalMultiplier, transform.localPosition.y);
                break;

            case ShipScriptable.Behaviour.diagonal:
                if (transform.localEulerAngles.z == 135)
                {
                    transform.position += properties.speed * Time.deltaTime * -transform.right;
                }
                else
                {
                    transform.position -= properties.speed * Time.deltaTime * -transform.right;
                }
                break;

            case ShipScriptable.Behaviour.wave8:
                if (!customBool && transform.position.y > 1)
                {
                    transform.position += properties.speed * Time.deltaTime * transform.up;
                    transform.localPosition = new Vector2(startPosition.x + Mathf.Sin(Time.time) * properties.behaviourMathfSin, transform.localPosition.y);
                }
                else
                {
                    customBool = true;
                    transform.localPosition = new Vector2(
                        startPosition.x + Mathf.Sin(Time.time) * properties.behaviourMathfSin,
                        Mathf.Lerp(transform.localPosition.y, 0.5f + Mathf.Sin(Time.time * 1.5f) * properties.behaviourMathfSin / 2, customFloat));
                }

                if (customBool && customFloat < 1 && customFloat >= 0)
                {
                    customFloat += (Time.deltaTime / 4);
                }
                break;

            default:
                break;
        }
    }
}