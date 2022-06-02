using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class audiomenu : MonoBehaviour
{
    [SerializeField] private Slider volume;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private Toggle check;

    public void OnSliderValueChanged()
    {
        _audioSource.volume = volume.value;
    }

    public void OnToggleValueChanged()
    {
        _audioSource.mute = !check.isOn;
    }
}
