using UnityEngine;

public class ShipBaseController : MonoBehaviour
{
    [Header("Properties")]
    public ShipScriptable _properties;
    [HideInInspector] public int health = 0;
    [HideInInspector] public float healthNormalized = 1f;

    [Space]
    public new SpriteRenderer renderer;
    public ParticleSystem engine1;
    public ParticleSystem engine2;
    [HideInInspector] public ParticleSystem.MainModule module;

    [Header("Firing")]
    public Transform shootRoot1;
    public Transform shootRoot2;
    public Transform shootRoot3;
    public Transform shootRoot4;
    public Transform shootRoot5;
    [HideInInspector] public float timer = -1;
    [HideInInspector] public float timer2 = -1;
    [HideInInspector] public bool timer2Up = false;
}