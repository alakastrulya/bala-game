using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    public string menuSceneName = "Main Menu";
    public GameObject pausePanel;
    public GameObject pauseButton;
    public static bool isPaused = false;

    public void Play()
    {
        SceneManager.LoadScene("Physics Game 1");
    }

    public void Quit()
    {
        Application.Quit();
        Debug.Log("Quitting the game");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
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

    void Start()
    {
        Time.timeScale = 1f;
        isPaused = false;

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
    }

    void Update()
    {
        if (pausePanel != null && Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Pause()
    {
        isPaused = true;
        if (pausePanel != null) pausePanel.SetActive(true);
        if (pauseButton != null) pauseButton.SetActive(false);

        Time.timeScale = 0f;

        if (BackgroundMusic.instance != null)
        {
            BackgroundMusic.instance.GetComponent<AudioSource>()?.Pause();
        }
    }

    public void Resume()
    {
        isPaused = false;
        if (pausePanel != null) pausePanel.SetActive(false);
        if (pauseButton != null) pauseButton.SetActive(true);

        Time.timeScale = 1f;

        if (BackgroundMusic.instance != null)
        {
            BackgroundMusic.instance.GetComponent<AudioSource>()?.UnPause();
        }
    }

    public void RestartLevel()
    {
        Debug.Log("Game Over / Restart");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}