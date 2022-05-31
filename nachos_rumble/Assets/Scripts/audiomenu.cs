using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class audiomenu : MonoBehaviour
{
    [SerializeField] private Slider volume;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private Toggle check;

    // Start is called before the first frame update
    void Start()
    {
        volume.value = 0.3f;
    }

    // Update is called once per frame
    void Update()
    {
        _audioSource.volume = volume.value;
        if (!check.isOn)
        {
            _audioSource.mute = true;
        }
        else
        {
            _audioSource.mute = false;
        }
    }
}
