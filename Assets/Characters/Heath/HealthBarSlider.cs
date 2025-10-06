using UnityEngine;
using UnityEngine.UI;
public class HealthBarSlider : MonoBehaviour
{
    public Slider slider;
    public Gradient gradient;
    public Image fill;

    public void SetMaxHealth (int health)
    {
        slider.maxValue = health;
        slider.value = health;

        fill.color = gradient.Evaluate(1f);

    }
    public void SetHealth(int heath)
    {
        slider.value = heath;
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }
}
