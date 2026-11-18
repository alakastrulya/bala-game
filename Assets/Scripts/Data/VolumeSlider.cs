using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class VolumeSlider : MonoBehaviour, IPointerUpHandler
{
    Slider slider;

    void Start()
    {
        slider = GetComponent<Slider>();
        slider.SetValueWithoutNotify(SaveSystem.Data.musicVolume);
        slider.onValueChanged.AddListener(OnChanged);
    }

    void OnChanged(float value)
    {
        if (BackgroundMusic.instance != null) BackgroundMusic.instance.SetVolume(value);
        SaveSystem.SetMusicVolume(value);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        SaveSystem.Save();
    }
}
