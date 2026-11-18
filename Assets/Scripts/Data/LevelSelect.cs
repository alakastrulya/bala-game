using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelSelect : MonoBehaviour
{
    [SerializeField] GameObject levelsPanel;
    [SerializeField] GameObject confirmPanel;
    [SerializeField] TMP_Text confirmText;
    [SerializeField] Button[] levelButtons;
    [SerializeField] GameObject[] lockIcons;
    [SerializeField] TMP_Text[] bestTexts;

    void Start()
    {
        levelsPanel.SetActive(false);
        confirmPanel.SetActive(false);
    }

    public void Open()
    {
        Refresh();
        levelsPanel.SetActive(true);
    }

    public void Close()
    {
        levelsPanel.SetActive(false);
    }

    public void LoadLevel(int index)
    {
        if (!SaveSystem.IsUnlocked(index)) return;
        LevelCatalog.Get.Load(index);
    }

    public void AskReset()
    {
        int opened = Mathf.Min(SaveSystem.Data.unlockedLevels, levelButtons.Length);
        bool anyScore = SaveSystem.Data.bestScores.Exists(s => s > 0);

        if (confirmText != null)
        {
            if (opened <= 1 && !anyScore) confirmText.text = "There is no progress yet.";
            else confirmText.text = "Levels 2-" + opened + " will be locked again\nand all best scores will be deleted.\nThis cannot be undone.";
        }
        confirmPanel.SetActive(true);
    }

    public void CancelReset()
    {
        confirmPanel.SetActive(false);
    }

    public void ConfirmReset()
    {
        SaveSystem.ResetProgress();
        confirmPanel.SetActive(false);
        Refresh();
    }

    void Refresh()
    {
        for (int i = 0; i < levelButtons.Length; i++)
        {
            bool unlocked = SaveSystem.IsUnlocked(i);
            levelButtons[i].interactable = unlocked;

            if (i < lockIcons.Length && lockIcons[i] != null) lockIcons[i].SetActive(!unlocked);

            if (i < bestTexts.Length && bestTexts[i] != null)
            {
                int best = SaveSystem.BestScore(i);
                bestTexts[i].text = best > 0 ? "Best: " + best : "";
            }
        }
    }
}
