using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private ShipScriptable properties;

    private bool _boolToContinue = false;
    private float _timeToContinue;
    private float customFloat = 0;
    private bool customBool = false;
    private Vector2 startPosition;
    private float line;
    private int index = 0;

    public void Init(ShipScriptable _properties)
    {
        properties = _properties;

        _boolToContinue = properties._timeToContinue;
        _timeToContinue = properties.timeToContinue;

        customFloat = 0;
        customBool = false;
        index = properties.spawnIndex;

        line = Mathf.Lerp(GameManager.PlayerLimits.x, GameManager.PlayerLimits.y, (properties.directLine + 1) / 2);

        switch (properties.behaviour)
        {
            case Enums.Behaviour.Waves:
                LimitX();
                startPosition = transform.localPosition;
                break;

            case Enums.Behaviour.Diagonal:
                if (transform.localPosition.x > 0)
                {
                    transform.localEulerAngles = new Vector3(0, 0, 225);
                }
                else
                {
                    transform.localEulerAngles = new Vector3(0, 0, 135);
                }
                break;

            case Enums.Behaviour.Wave8:
                transform.localPosition = new Vector2(0, GameManager.BoundsLimits.x);
                startPosition = transform.localPosition;
                break;

            case Enums.Behaviour.Borders:
                if (properties.spawnIndex % 2 == 0)
                {
                    transform.localPosition = new Vector2(GameManager.PlayerLimits.w + 0.25f, transform.localPosition.y);
                    transform.localEulerAngles = new Vector3(0, 0, 90);
                }
                else
                {
                    transform.localPosition = new Vector2(GameManager.PlayerLimits.z - 0.25f, transform.localPosition.y);
                    transform.localEulerAngles = new Vector3(0, 0, 270);
                }
                break;

            default:
                break;
        }
    }

    private void LimitX()
    {
        if (transform.localPosition.x > GameManager.InnerLimits.w)
        {
            transform.localPosition = new Vector2(GameManager.InnerLimits.w, transform.localPosition.y);
        }
        else if (transform.localPosition.x < GameManager.InnerLimits.z)
        {
            transform.localPosition = new Vector2(-GameManager.InnerLimits.z, transform.localPosition.y);
        }
    }

    public void Move()
    {
        if (_boolToContinue && _timeToContinue > 0)
        {
            _timeToContinue -= Time.deltaTime;

            if (_timeToContinue < 0)
            {
                _timeToContinue = 0;
            }
        }

        switch (properties.behaviour)
        {
            case Enums.Behaviour.Linear:
                Linear();
                break;

            case Enums.Behaviour.Waves:
                Waves();
                break;

            case Enums.Behaviour.Diagonal:
                Diagonal();
                break;

            case Enums.Behaviour.Wave8:
                Wave8();
                break;

            case Enums.Behaviour.Borders:
                Borders();
                break;

            case Enums.Behaviour.Chase:
                Chase();
                break;
        }
    }

    private bool StopIfDirect(bool directBool)
    {
        if (!directBool)
        {
            return false;
        }
        else if (transform.position.y <= line && _timeToContinue != 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void Linear()
    {
        if (StopIfDirect(properties.directBehaviour))
        {
            return;
        }

        transform.position += properties.speed * Time.deltaTime * transform.up;
    }

    private void Waves()
    {
        transform.localPosition = new Vector2(
            startPosition.x + Mathf.Sin(Time.time + (properties.behaviourMathfInverted && index % 2 != 0 ? 3 : 0) * GameManager.HorizontalInvertedMultiplier) * properties.behaviourMathfSin * GameManager.HorizontalMultiplier,
            transform.localPosition.y);

        if (StopIfDirect(properties.directBehaviour))
        {
            return;
        }

        transform.position += properties.speed * Time.deltaTime * transform.up;
    }

    private void Diagonal()
    {
        if (StopIfDirect(properties.directBehaviour))
        {
            return;
        }

        if (transform.localEulerAngles.z == 135)
        {
            transform.position += properties.speed * Time.deltaTime * -transform.right;
        }
        else
        {
            transform.position -= properties.speed * Time.deltaTime * -transform.right;
        }
    }

    private void Wave8()
    {
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
            customFloat += Time.deltaTime / 4;
        }
    }

    private void Borders()
    {
        if (StopIfDirect(properties.directBehaviour))
        {
            return;
        }

        if (transform.localEulerAngles.z == 90)
        {
            transform.position += properties.speed * Time.deltaTime * -transform.right;
        }
        else
        {
            transform.position += properties.speed * Time.deltaTime * transform.right;
        }
    }

    private void Chase()
    {
        if (transform.position.y < PlayerController.Instance.transform.position.y)
        {
            return;
        }

        if (transform.position.x >= PlayerController.Instance.transform.position.x)
        {
            transform.position = new Vector2(transform.position.x - (properties.speed * Time.deltaTime * properties.behaviourMathfSin), transform.position.y);
        }
        else
        {
            transform.position = new Vector2(transform.position.x + (properties.speed * Time.deltaTime * properties.behaviourMathfSin), transform.position.y);
        }

        if (StopIfDirect(properties.directBehaviour))
        {
            return;
        }

        transform.position += properties.speed * Time.deltaTime * transform.up;
    }
}