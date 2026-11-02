using UnityEngine;
using UnityEngine.SceneManagement;

public class WinMenu : MonoBehaviour
{
    public GameObject winPanel;
    public string menuSceneName = "Main Menu";
    public GameObject pauseButton;
    public void ShowWin()
    {
        if (pauseButton != null) pauseButton.SetActive(false);

        winPanel.SetActive(true);
        Time.timeScale = 0f;

        if (BackgroundMusic.instance != null)
        {
            BackgroundMusic.instance.PlayWinMusic();
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;

        if (BackgroundMusic.instance != null)
        {
            BackgroundMusic.instance.PlayDefaultMusic();
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void BackToMenu()
    {
        Time.timeScale = 1f;

        if (BackgroundMusic.instance != null)
        {
            BackgroundMusic.instance.PlayDefaultMusic();
        }

        SceneManager.LoadScene(menuSceneName);
    }
}