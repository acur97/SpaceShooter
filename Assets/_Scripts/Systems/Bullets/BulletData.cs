using System;
using UnityEngine;

[Serializable]
public struct BulletData
{
    public SpriteRenderer renderer;
    public float lifetime;
    public float speed;
}

[Serializable]
public struct Bullet
{
    public Transform transform;
    public BulletData data;
}