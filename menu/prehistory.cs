using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class prehistory : MonoBehaviour
{
    [SerializeField] private DialogMini[] dialogs = new DialogMini[0];
    [SerializeField] private GameObject prehistoryObj, trainingObj;
    [SerializeField] private TextMeshProUGUI textDialog;
    public bool prehistoryEnded = false; // Потом убрать true

    private void Update()
    {
        if (prehistoryEnded)
        {
            if (Input.anyKey)
                SceneManager.LoadScene("step1");
        }
    }

    public void ActivateText(int text) => StartCoroutine(SetText(text));

    private IEnumerator SetText(int text)
    {
        textDialog.text = "";
        char[] textChar = dialogs[text].text.ToCharArray();
        foreach (char tChar in textChar)
        {
            textDialog.text += tChar;
            yield return new WaitForSeconds(0.06f);
        }
    }
}

[System.Serializable]
public class DialogMini
{
        [HideInInspector]
    public string text
    {
        get
        {
            if (PlayerPrefs.GetString("language") == "ru")
                return ruText;
            else
                return enText;
        }
    }
    public string ruText, enText;
}
