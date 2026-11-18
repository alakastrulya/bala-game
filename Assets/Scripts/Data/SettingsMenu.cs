using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] GameObject settingsPanel;

    void Start()
    {
        settingsPanel.SetActive(false);
    }

    public void Open()
    {
        settingsPanel.SetActive(true);
    }

    public void Close()
    {
        settingsPanel.SetActive(false);
        SaveSystem.Save();
    }
}
