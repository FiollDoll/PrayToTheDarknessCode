using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class dialogsManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textName, textDialog;
    [SerializeField] private Image iconImage, bigPicture;
    [SerializeField] private Image noViewPanel;

    private bool animatingText;
    public dialog[] dialogs = new dialog[0];
    private dialog activatedDialog;
    private int totalStep;
    public GameObject dialogMenu;
    [SerializeField] private allScripts scripts;

    private void Start() => dialogMenu.GetComponent<RectTransform>().DOScale(new Vector3(0f, 1f, 0f), 0.001f);

    public void ActivateDialog(string name) // Старт диалога
    {
        foreach (dialog totalDialog in dialogs)
        {
            if (totalDialog.nameDialog == name && !totalDialog.readed)
            {
                dialogMenu.gameObject.SetActive(true);
                if (totalDialog.bigPicture != null)
                {
                    noViewPanel.DOFade(100f, 0.5f).SetEase(Ease.InQuart);
                    bigPicture.sprite = totalDialog.bigPicture;
                    bigPicture.DOFade(100f, 1f).SetEase(Ease.InQuart);
                }
                Sequence sequence = DOTween.Sequence();
                sequence.Append(dialogMenu.GetComponent<RectTransform>().DOScale(new Vector3(1f, 1f, 1f), 0.4f).SetEase(Ease.InQuart));
                if (totalDialog.bigPicture != null)
                    sequence.Append(noViewPanel.DOFade(0f, 0.5f).SetEase(Ease.OutQuart));
                sequence.Insert(0, transform.DOScale(new Vector3(3, 3, 3), sequence.Duration()));

                if (!totalDialog.moreRead)
                    totalDialog.readed = true;
                scripts.player.canMove = false;
                activatedDialog = totalDialog;
                textName.text = activatedDialog.steps[0].name;
                StartCoroutine(SetText(activatedDialog.steps[0].text));
                iconImage.sprite = activatedDialog.steps[0].icon;
                break;
            }
        }
    }

    private void DialogMoveNext()
    {
        if ((totalStep + 1) == activatedDialog.steps.Length)
        {
            totalStep = 0;
            Sequence sequence = DOTween.Sequence();
            if (activatedDialog.bigPicture != null)
            {
                sequence.Append(noViewPanel.DOFade(100f, 0.5f).SetEase(Ease.InQuart));
                sequence.Append(dialogMenu.GetComponent<RectTransform>().DOScale(new Vector3(0f, 1f, 0f), 0.1f));
                sequence.Append(bigPicture.DOFade(0f, 0.1f).SetEase(Ease.OutQuart));
                sequence.Append(noViewPanel.DOFade(0f, 0.5f).SetEase(Ease.OutQuart));
            }
            else
                sequence.Append(dialogMenu.GetComponent<RectTransform>().DOScale(new Vector3(0f, 1f, 0f), 0.5f));

            if (activatedDialog.startNewDialogAfterEnd == null)
            {
                sequence.Insert(0, transform.DOScale(new Vector3(1, 1, 1), sequence.Duration()));
                sequence.OnComplete(() =>
                {
                    dialogMenu.gameObject.SetActive(false);
                });
                scripts.player.canMove = true;
            }
            else
                ActivateDialog(activatedDialog.startNewDialogAfterEnd.nameDialog);

        }
        else
        {
            totalStep++;
            textName.text = activatedDialog.steps[totalStep].name;
            StartCoroutine(SetText(activatedDialog.steps[totalStep].text));
            iconImage.sprite = activatedDialog.steps[totalStep].icon;
        }
    }

    private IEnumerator SetText(string text)
    {
        textDialog.text = "";
        animatingText = true;
        char[] textChar = text.ToCharArray();
        foreach (char tChar in textChar)
        {
            if (animatingText)
            {
                textDialog.text += tChar;
                yield return new WaitForSeconds(0.05f);
            }
        }
        animatingText = false;
    }

    public void DialogManage()
    {
        if (animatingText)
        {
            StopCoroutine("SetText");
            animatingText = false;
            textDialog.text = activatedDialog.steps[totalStep].text;
        }
        else
            DialogMoveNext();
    }

    private void Update()
    {
        if (activatedDialog != null)
        {
            if (Input.GetKeyDown(KeyCode.Space))
                DialogManage();
        }
    }
}

[System.Serializable]
public class dialog
{
    public string nameDialog;
    public dialogStep[] steps = new dialogStep[0];
    public dialog startNewDialogAfterEnd;
    public Sprite bigPicture;
    public bool readed, moreRead;
}

[System.Serializable]
public class dialogStep
{
    [HideInInspector]
    public string name
    {
        get
        {
            if (PlayerPrefs.GetString("language") == "ru")
                return ruName;
            else
                return enName;
        }
    }
    public string ruName, enName;
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
    public Sprite icon;
}
