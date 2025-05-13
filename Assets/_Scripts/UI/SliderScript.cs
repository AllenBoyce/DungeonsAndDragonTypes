using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class SliderScript : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private string _playerPrefsKey;
    void Start()
    {
        _slider.value = PlayerPrefs.GetFloat(_playerPrefsKey, 1.0f);
        _text.text = (_slider.value * 100).ToString("0.0");
        _slider.onValueChanged.AddListener((value) => {
            _text.text = (value * 100).ToString("0.0");
        });
    }

    public float GetValue()
    {
        return _slider.value;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
