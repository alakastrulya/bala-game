using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class WinMenu : MonoBehaviour
{
    public GameObject winPanel;
    public GameObject levelCompletePanel;
    public string menuSceneName = "Main Menu";
    public GameObject pauseButton;
    public TMP_Text levelCompleteResult;
    public TMP_Text winResult;

    bool HasNextDataLevel => LevelLoader.Instance != null && LevelLoader.Instance.HasNextLevel;
    bool HasNextScene => SceneManager.GetActiveScene().buildIndex + 1 < SceneManager.sceneCountInBuildSettings;

    public void Finish()
    {
        Finish(-1, 0, false);
    }

    public void FinishWithScore(int score)
    {
        if (LevelTimer.Instance != null) LevelTimer.Instance.Stop();

        int index = LevelCatalog.Get != null ? LevelCatalog.Get.CurrentIndex() : -1;
        int oldBest = index >= 0 ? SaveSystem.BestScore(index) : 0;
        LevelCatalog.CompleteCurrent(score);
        int best = index >= 0 ? SaveSystem.BestScore(index) : score;

        Finish(score, best, score > oldBest);
    }

    public void Finish(int score, int best, bool newBest)
    {
        string result = "";
        if (score >= 0)
        {
            result = "Score: " + score + "\nBest: " + best;
            if (newBest) result += "\nNEW BEST!";
        }
        if (levelCompleteResult != null) levelCompleteResult.text = result;
        if (winResult != null) winResult.text = result;

        if (HasNextDataLevel || HasNextScene) ShowLevelComplete();
        else ShowWin();
    }

    public void ShowLevelComplete()
    {
        if (levelCompletePanel == null)
        {
            NextLevel();
            return;
        }

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
