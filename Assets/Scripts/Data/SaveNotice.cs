using System.Collections;
using UnityEngine;
using TMPro;

public class SaveNotice : MonoBehaviour
{
    [SerializeField] TMP_Text message;
    [SerializeField] float showTime = 4f;

    void Start()
    {
        message.gameObject.SetActive(false);
        if (SaveSystem.Data != null && SaveSystem.LoadFailed) StartCoroutine(Show());
    }

    IEnumerator Show()
    {
        SaveSystem.AcknowledgeLoadFailed();
        message.text = "Save file was damaged.\nProgress was reset.";
        message.gameObject.SetActive(true);
        yield return new WaitForSeconds(showTime);
        message.gameObject.SetActive(false);
    }
}
