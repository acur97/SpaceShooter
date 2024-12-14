using UnityEngine;
using UnityEngine.UI;

public class SliderGradient : MonoBehaviour
{
    [SerializeField] private Gradient gradient;
    [SerializeField] private Slider slider;
    [SerializeField] private Image image;

    public void OnChange(float _)
    {
        //image.color = gradient.Evaluate(value.Remap(slider.minValue, slider.maxValue, 1, 0));
        image.color = gradient.Evaluate(slider.normalizedValue);
    }
}