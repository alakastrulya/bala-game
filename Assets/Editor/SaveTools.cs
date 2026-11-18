using System.IO;
using UnityEditor;
using UnityEngine;

public static class SaveTools
{
    [MenuItem("Tools/Save/Open Save Folder")]
    static void OpenFolder()
    {
        EditorUtility.RevealInFinder(SaveSystem.FilePath);
    }

    [MenuItem("Tools/Save/Delete Save File")]
    static void DeleteSave()
    {
        if (File.Exists(SaveSystem.FilePath)) File.Delete(SaveSystem.FilePath);
        Debug.Log("Save file deleted");
    }
}
