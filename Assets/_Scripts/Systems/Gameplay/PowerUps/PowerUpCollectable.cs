using UnityEngine;

public class PowerUpCollectable : MonoBehaviour
{
    [ReadOnly] public PowerUpBase powerUp;

    [Space]
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Collider2D _collider;
    public ParticleSystem particles;

    [Space]
    [SerializeField] private float speed;
    [SerializeField] private float waveSpeed;
    [SerializeField] private float rotationMultiplier;

    private bool canMove = false;
    private Vector2 startPosition;
    private float wave;

    public void Init(PowerUpBase _powerUp)
    {
        powerUp = _powerUp;

        transform.localPosition = new Vector2(Random.Range(GameManager.PlayerLimits.z, GameManager.PlayerLimits.w), GameManager.BoundsLimits.x);
        startPosition = transform.localPosition;

        sprite.sprite = _powerUp.sprite;

        gameObject.SetActive(true);

        canMove = true;
    }

    private void Update()
    {
        if (!canMove)
        {
            return;
        }

        wave = Mathf.Sin(Time.time * GameManager.HorizontalInvertedMultiplier) * waveSpeed * GameManager.HorizontalMultiplier;

        transform.localEulerAngles = new Vector3(transform.localRotation.x, transform.localRotation.y, wave * rotationMultiplier);
        transform.position += speed * Time.deltaTime * -transform.up;
        transform.localPosition = new Vector2(startPosition.x + wave, transform.localPosition.y);

        if (transform.position.y <= GameManager.BoundsLimits.y)
        {
            gameObject.SetActive(false);
        }
    }

    public void Stop()
    {
        _collider.enabled = false;
        canMove = false;
        sprite.enabled = false;
        particles.Stop(false, ParticleSystemStopBehavior.StopEmitting);
    }
}