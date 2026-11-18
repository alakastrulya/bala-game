using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelTimer : MonoBehaviour
{
    public static LevelTimer Instance { get; private set; }

    [SerializeField] TMP_Text timerText;

    float timeLeft;
    bool running;

    public int SecondsLeft => Mathf.CeilToInt(Mathf.Max(timeLeft, 0f));

    void Awake()
    {
        Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    void Start()
    {
        LevelEntry entry = LevelCatalog.Get != null ? LevelCatalog.Get.CurrentEntry() : null;
        if (entry == null || entry.timeLimit <= 0f)
        {
            if (timerText != null) timerText.gameObject.SetActive(false);
            return;
        }

        timeLeft = entry.timeLimit;
        running = true;
    }

    void Update()
    {
        if (!running) return;

        timeLeft -= Time.deltaTime;
        if (timerText != null) timerText.text = SecondsLeft.ToString();

        if (timeLeft <= 0f)
        {
            running = false;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void Stop()
    {
        running = false;
        if (timerText != null) timerText.gameObject.SetActive(false);
    }
}
