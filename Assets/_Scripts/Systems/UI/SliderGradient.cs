using UnityEngine;
using UnityEngine.UI;

public class SliderGradient : MonoBehaviour
{
    [SerializeField] private Gradient gradient;
    [SerializeField] private Slider slider;
    [SerializeField] private Image image;

    public void OnChange(float _)
    {
        image.color = gradient.Evaluate(slider.normalizedValue);
    }
}