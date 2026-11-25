using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Put on a Slider in Settings. Choose Music or Effects in the Inspector.
[RequireComponent(typeof(Slider))]
public class VolumeSlider : MonoBehaviour, IPointerUpHandler
{
    public enum Channel { Music, Effects }

    [SerializeField] Channel channel = Channel.Music;
    Slider slider;

    void Start()
    {
        slider = GetComponent<Slider>();
        slider.minValue = 0.0001f;   // 0.0001 = -80 dB, silence
        slider.maxValue = 1f;
        slider.SetValueWithoutNotify(channel == Channel.Music ? SaveSystem.Data.musicVolume : SaveSystem.Data.sfxVolume);
        slider.onValueChanged.AddListener(OnChanged);
    }

    void OnChanged(float value)
    {
        if (channel == Channel.Music) SaveSystem.SetMusicVolume(value);
        else SaveSystem.SetSfxVolume(value);
        AudioManager.ApplyVolumes();   // slider 0-1 -> decibels in the mixer
    }

    // save once when the finger is lifted, not every frame while dragging
    public void OnPointerUp(PointerEventData eventData)
    {
        SaveSystem.Save();
        if (channel == Channel.Effects) AudioManager.Play(Sfx.Coin);   // let the player hear the new level
    }
}
