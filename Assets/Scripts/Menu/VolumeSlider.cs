using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    public Slider slider;

    private void Start()
    {
        float volume = PlayerPrefs.GetFloat("Volume", 1);
        AudioListener.volume = volume;
        slider.value = volume;
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat("Volume", volume);
    }
    
}
