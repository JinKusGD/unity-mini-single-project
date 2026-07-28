using UnityEngine;
using UnityEngine.UI;

public class OptionSlider : MonoBehaviour
{
    [SerializeField] private Slider _slider1;
    [SerializeField] private Slider _slider2;

    private void OnEnable()
    {
        _slider1.value = AudioManager.Instance.GetBgmVolume();
        _slider2.value = AudioManager.Instance.GetSFXVolume();
    }

    private void Update()
    {
        AudioManager.Instance.SetBgmVolume(_slider1.value);
        AudioManager.Instance.SetSFXVolume(_slider2.value);
    }
}
