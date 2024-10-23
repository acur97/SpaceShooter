using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private float _timeToContinue;
    private float customFloat = 0;
    private bool customBool = false;
    private Vector2 startPosition;

    public void Init(ShipScriptable.Behaviour properties, float timeToContinue, int customInt)
    {
        _timeToContinue = timeToContinue;

        switch (properties)
        {
            case ShipScriptable.Behaviour.linear:
                break;

            case ShipScriptable.Behaviour.direct:
                break;

            case ShipScriptable.Behaviour.waves:
                LimitX();
                startPosition = transform.localPosition;
                break;

            case ShipScriptable.Behaviour.wavesDirect:
                LimitX();
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
                transform.localPosition = new Vector2(0, GameManager.BoundsLimits.y);
                startPosition = transform.localPosition;
                break;

            case ShipScriptable.Behaviour.borders:
                if (customInt % 2 == 0)
                {
                    transform.localPosition = new Vector2(GameManager.PlayerLimits.x + 0.25f, transform.localPosition.y);
                    transform.localEulerAngles = new Vector3(0, 0, 90);
                }
                else
                {
                    transform.localPosition = new Vector2(-GameManager.PlayerLimits.x - 0.25f, transform.localPosition.y);
                    transform.localEulerAngles = new Vector3(0, 0, 270);
                }
                break;
        }

        customFloat = 0;
        customBool = false;
    }

    private void LimitX()
    {
        if (transform.localPosition.x > GameManager.InnerLimits.x)
        {
            transform.localPosition = new Vector2(GameManager.InnerLimits.x, transform.localPosition.y);
        }
        else if (transform.localPosition.x < -GameManager.InnerLimits.x)
        {
            transform.localPosition = new Vector2(-GameManager.InnerLimits.x, transform.localPosition.y);
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
                Linear(properties);
                break;

            case ShipScriptable.Behaviour.direct:
                Direct(properties);
                break;

            case ShipScriptable.Behaviour.waves:
                Waves(properties);
                break;

            case ShipScriptable.Behaviour.wavesDirect:
                WavesDirect(properties);
                break;

            case ShipScriptable.Behaviour.diagonal:
                Diagonal(properties);
                break;

            case ShipScriptable.Behaviour.wave8:
                Wave8(properties);
                break;

            case ShipScriptable.Behaviour.borders:
                Borders(properties);
                break;
        }
    }

    private void Linear(ShipScriptable properties)
    {
        transform.position += properties.speed * Time.deltaTime * transform.up;
    }

    private void Direct(ShipScriptable properties)
    {
        if (transform.position.y > GameManager.EnemyLine || _timeToContinue == 0)
        {
            transform.position += properties.speed * Time.deltaTime * transform.up;
        }
    }

    private void Waves(ShipScriptable properties)
    {
        transform.position += properties.speed * Time.deltaTime * transform.up;

        transform.localPosition = new Vector2(
            startPosition.x + Mathf.Sin(Time.time * GameManager.HorizontalInvertedMultiplier) * properties.behaviourMathfSin * GameManager.HorizontalMultiplier,
            transform.localPosition.y);
    }

    private void WavesDirect(ShipScriptable properties)
    {
        if (transform.position.y > GameManager.EnemyLine || _timeToContinue == 0)
        {
            transform.position += properties.speed * Time.deltaTime * transform.up;
        }

        transform.localPosition = new Vector2(
            startPosition.x + Mathf.Sin(Time.time * GameManager.HorizontalInvertedMultiplier) * properties.behaviourMathfSin * GameManager.HorizontalMultiplier,
            transform.localPosition.y);
    }

    private void Diagonal(ShipScriptable properties)
    {
        if (transform.localEulerAngles.z == 135)
        {
            transform.position += properties.speed * Time.deltaTime * -transform.right;
        }
        else
        {
            transform.position -= properties.speed * Time.deltaTime * -transform.right;
        }
    }

    private void Wave8(ShipScriptable properties)
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
            customFloat += (Time.deltaTime / 4);
        }
    }

    private void Borders(ShipScriptable properties)
    {
        if (transform.localEulerAngles.z == 90)
        {
            transform.position += properties.speed * Time.deltaTime * -transform.right;
        }
        else
        {
            transform.position += properties.speed * Time.deltaTime * transform.right;
        }
    }
}