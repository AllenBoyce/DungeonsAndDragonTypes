using UnityEngine;
using UnityEngine.UI;
public class SaveOptionsButton : MonoBehaviour
{
    private Button _button;
    private SliderScript _musicSlider;
    private SliderScript _sfxSlider;
    void Start()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(SaveOptions);
        _musicSlider = GameObject.Find("MusicSlider").GetComponent<SliderScript>();
        _sfxSlider = GameObject.Find("SFXSlider").GetComponent<SliderScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SaveOptions()
    {
        PlayerPrefs.SetFloat("MusicVolume", _musicSlider.GetValue());
        PlayerPrefs.SetFloat("SFXVolume", _sfxSlider.GetValue());
        PlayerPrefs.Save();
    }
}
