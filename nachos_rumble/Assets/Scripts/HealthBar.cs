using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HealthBar : MonoBehaviour
{
    public Slider Slider;
    public Gradient _gradient;
    public Image fill;
    public void SetMaxHealth(float health)
    {
        Slider.maxValue = health;
        Slider.value = health;

        fill.color = _gradient.Evaluate(1f);
    }

    public void SetHealth(float health)
    {
        Slider.value = health;

        fill.color = _gradient.Evaluate(Slider.normalizedValue);
    }
}
