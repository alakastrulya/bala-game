using UnityEngine;
using UnityEngine.SceneManagement;

public class WinMenu : MonoBehaviour
{
    public GameObject winPanel;
    public GameObject levelCompletePanel;
    public string menuSceneName = "Main Menu";
    public GameObject pauseButton;

    bool HasNextDataLevel => LevelLoader.Instance != null && LevelLoader.Instance.HasNextLevel;
    bool HasNextScene => SceneManager.GetActiveScene().buildIndex + 1 < SceneManager.sceneCountInBuildSettings;

    public void Finish()
    {
        if (HasNextDataLevel || HasNextScene) ShowLevelComplete();
        else ShowWin();
    }

    public void ShowLevelComplete()
    {
        if (pauseButton != null) pauseButton.SetActive(false);
        levelCompletePanel.SetActive(true);
        Time.timeScale = 0f;
    }

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

    public void NextLevel()
    {
        Time.timeScale = 1f;
        GameSession.StartedFromMenu = true;

        if (HasNextDataLevel)
        {
            GameSession.LevelIndex++;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else if (HasNextScene)
        {
            GameSession.LevelIndex = 0;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else
        {
            BackToMenu();
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
