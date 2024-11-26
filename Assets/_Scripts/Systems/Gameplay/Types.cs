using UnityEngine;

public class Types
{
    // tags
    public const string tag_Enemy = "Enemy";
    public const string tag_Bullet = "Bullet";
    public const string tag_Collectable = "Collectable";

    // layers
    public const int layer_Player = 6;
    public const int layer_Enemy = 7;
    public const int layer_Bullet = 8;
    public const int layer_Collectable = 9;
    public const int layer_PowerUp = 10;

    // material properties
    public static readonly int material_Color = Shader.PropertyToID("_Color");

    // Ui
    public const string ui_Free = "free";
    public const string ui_Owned = "owned";
}