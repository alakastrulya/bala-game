using UnityEngine;

public class SaveManager : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Create()
    {
        GameObject go = new GameObject("SaveManager");
        DontDestroyOnLoad(go);
        go.AddComponent<SaveManager>();
        Debug.Log("Save file: " + SaveSystem.FilePath);
    }

    void OnApplicationPause(bool paused)
    {
        if (paused) SaveSystem.Save();
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus) SaveSystem.Save();
    }

    void OnApplicationQuit()
    {
        SaveSystem.Save();
    }
}
