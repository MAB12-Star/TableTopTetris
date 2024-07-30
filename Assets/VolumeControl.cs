using UnityEngine;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour
{
    public Slider volumeSlider;

    public void Start()
    {
        // Set the slider's value to the current music volume
        volumeSlider.value = AudioManager1.Instance.musicSource.volume;

        // Add a listener to the slider to handle volume changes
        volumeSlider.onValueChanged.AddListener(delegate { OnVolumeChange(); });
    }

    public void OnVolumeChange()
    {
        // Update the music volume based on the slider's value
        AudioManager1.Instance.SetVolume(volumeSlider.value);
    }
}
