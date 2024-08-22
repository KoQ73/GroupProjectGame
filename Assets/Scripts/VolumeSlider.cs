using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VolumeSlider : MonoBehaviour
{
    public Slider slider;
    public TMP_Text volumeText;

    void Start()
    {
        // Get the current volume from the AudioManager and set the slider value
        AudioManager audioManager = FindObjectOfType<AudioManager>();
        float currentVolume = audioManager.GetVolume();
        slider.value = currentVolume * 100f;  // Convert 0.0-1.0 range to 0-100 range

        // Update the volume text initially
        volumeText.text = slider.value.ToString("0");

        // Add a listener to call the method when the slider value changes
        slider.onValueChanged.AddListener(delegate { OnVolumeChange(); });
    }

    public void OnVolumeChange()
    {
        // Adjust the volume in the MusicManager
        FindObjectOfType<AudioManager>().SetVolume(slider.value);

        // Update the volume text to display the current slider value
        volumeText.text = slider.value.ToString("0");
    }
}
