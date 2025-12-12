
using UnityEngine;
using UnityEngine.UI;
using static TMPro.TMP_Dropdown;
using System;

public class SettingsManager : MonoBehaviour
{
    private Toggle FullScreenToggle;
    private TMPro.TMP_Dropdown  ResolutionSetting;
    private Slider VolumeSlider;

    int width=1280;
    int height=720;


	
	void Start ()
    {
        

        this.FullScreenToggle = GameObject.FindAnyObjectByType<Toggle>();
        this.ResolutionSetting = GameObject.FindAnyObjectByType<TMPro.TMP_Dropdown >();
        this.VolumeSlider = GameObject.FindAnyObjectByType<Slider>();
    }

    void Update()
    {

        
    }


    public void OnFullscreenToggle()
    {
        
        Debug.Log("Fullscreen toggle: " + this.FullScreenToggle.isOn);
       
    }

    public void OnResolutionSettingsChanged()
    {
        int index = this.ResolutionSetting.value;
        OptionData[] options = this.ResolutionSetting.options.ToArray();
        Debug.Log("Resolution Settings: " + options[index].text);
        string[] resolution = options[index].text.Split("x");
        width = Int32.Parse(resolution[0]);
        height = Int32.Parse(resolution[1]);

        
    } 

      public void OnVolumeSettingsChanged()
    {

        
        Debug.Log("Volume slider value: " + this.VolumeSlider.value);
        AudioListener.volume = this.VolumeSlider.value;
    }

    public void OnClickSave()
    {
         if (this.FullScreenToggle.isOn)
        {
            Screen.fullScreen = true;
        }
        else
        {
             Screen.fullScreen = false;
        }
        Screen.SetResolution(width,height,this.FullScreenToggle.isOn);
        AudioListener.volume = this.VolumeSlider.value;

        
    }

}
