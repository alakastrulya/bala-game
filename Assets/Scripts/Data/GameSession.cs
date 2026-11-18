using UnityEngine;

public static class GameSession
{
    public static int LevelIndex;
    public static bool StartedFromMenu;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetOnPlay()
    {
        LevelIndex = 0;
        StartedFromMenu = false;
    }
}