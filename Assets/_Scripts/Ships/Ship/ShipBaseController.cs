using UnityEngine;

public class ShipBaseController : MonoBehaviour
{
    [Header("Properties")]
    public ShipScriptable _properties;
    [HideInInspector] public int health = 0;

    [Space]
    public new SpriteRenderer renderer;
    public ParticleSystem engine1;
    public ParticleSystem engine2;
    [HideInInspector] public ParticleSystem.MainModule module;

    [Header("Firing")]
    public Transform shootRoot;
    [HideInInspector] public float timer = -1;
}