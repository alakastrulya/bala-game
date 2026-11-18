using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    public int version = 1;
    public int unlockedLevels = 1;
    public List<int> bestScores = new List<int>();
    public float musicVolume = 1f;
}
