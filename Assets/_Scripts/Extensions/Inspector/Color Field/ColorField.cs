using UnityEngine;

public class ColorField : PropertyAttribute
{
    public Color color;

    public ColorField(float r, float g, float b)
    {
        color = new Color(r, g, b);
    }
}