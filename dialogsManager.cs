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

    [SerializeField] private Sprite nullSprite;
    private bool animatingText;
    public dialog[] dialogs = new dialog[0];
    private dialog activatedDialog;
    private int totalStep;
    public GameObject dialogMenu;
    [SerializeField] private allScripts scripts;

    private void Start() => dialogMenu.transform.Find("mainMenu").GetComponent<RectTransform>().DOScale(new Vector3(0f, 1f, 0f), 0.001f);

    public void ActivateDialog(string name) // Старт диалога
    {
        if (activatedDialog == null)
        {
            foreach (dialog totalDialog in dialogs)
            {
                if (totalDialog.nameDialog == name && !totalDialog.readed)
                {
                    dialogMenu.gameObject.SetActive(true);
                    if (totalDialog.steps[0].cameraTarget != null)
                        scripts.player.virtualCamera.Follow = totalDialog.steps[0].cameraTarget;

                    if (totalDialog.bigPicture != null)
                    {
                        Sequence sequence = DOTween.Sequence();
                        Tween stepSequence = noViewPanel.DOFade(100f, 0.5f).SetEase(Ease.InQuart);
                        stepSequence.OnComplete(() =>
                        {
                            bigPicture.sprite = totalDialog.bigPicture;
                            if (totalDialog.bigPictureSecond != null)
                                StartCoroutine(BigPictureAnimate(totalDialog));
                        });
                        sequence.Append(stepSequence);
                        sequence.Append(noViewPanel.DOFade(0f, 0.5f).SetEase(Ease.OutQuart));
                        sequence.Insert(0, transform.DOScale(new Vector3(1, 1, 1), sequence.Duration()));
                    }
                    else
                        bigPicture.sprite = nullSprite;
                    if (totalDialog.mainPanelStartDelay == 0)
                        dialogMenu.transform.Find("mainMenu").GetComponent<RectTransform>().DOScale(new Vector3(1f, 1f, 1f), 0.4f).SetEase(Ease.InQuart);
                    else
                        StartCoroutine(ActivateUIMainMenuWithDelay(totalDialog, totalDialog.mainPanelStartDelay));

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
    }

    private void DialogMoveNext()
    {
        if ((totalStep + 1) >= activatedDialog.steps.Length) // Окончание
        {
            if (activatedDialog.activatedCutsceneStepAtEnd != -1)
                scripts.cutsceneManager.ActivateCutsceneStep(activatedDialog.activatedCutsceneStepAtEnd);
            totalStep = 0;
            Sequence sequence = DOTween.Sequence();
            if (activatedDialog.bigPicture != null && !activatedDialog.disableFadeAtEnd)
            {
                sequence = sequence.Append(noViewPanel.DOFade(100f, 0.5f).SetEase(Ease.InQuart));
                sequence.Append(dialogMenu.transform.Find("mainMenu").GetComponent<RectTransform>().DOScale(new Vector3(0f, 1f, 0f), 0.1f));
                sequence.Append(bigPicture.DOFade(0f, 0.1f).SetEase(Ease.OutQuart));
                sequence.Append(noViewPanel.DOFade(0f, 0.5f).SetEase(Ease.OutQuart));
            }
            else
                sequence.Append(dialogMenu.transform.Find("mainMenu").GetComponent<RectTransform>().DOScale(new Vector3(0f, 1f, 0f), 0.5f));

            if (activatedDialog.darkAfterEnd && !activatedDialog.disableFadeAtEnd)
            {
                Tween extraSequence = noViewPanel.DOFade(100f, 0.7f).SetEase(Ease.InQuart);
                if (activatedDialog.posAfterEnd != null)
                {
                    extraSequence.OnComplete(() =>
                    {
                        scripts.player.transform.localPosition = activatedDialog.posAfterEnd.position;
                    });
                }
                sequence.Append(extraSequence);
                sequence.Append(noViewPanel.DOFade(0f, 0.7f).SetEase(Ease.OutQuart));
            }
            sequence.Insert(0, transform.DOScale(new Vector3(1, 1, 1), sequence.Duration()));
            sequence.OnComplete(() =>
            {
                dialogMenu.gameObject.SetActive(false);
                if (activatedDialog.noteAdd != "")
                    scripts.notebook.AddNote(activatedDialog.noteAdd);
                if (activatedDialog.startNewDialogAfterEnd != null)
                    ActivateDialog(activatedDialog.startNewDialogAfterEnd.nameDialog);
                scripts.player.canMove = true;
                scripts.player.virtualCamera.Follow = scripts.player.transform;
                activatedDialog = null;
            });
        }
        else if ((totalStep + 1) < activatedDialog.steps.Length)
        {
            totalStep++;
            textName.text = activatedDialog.steps[totalStep].name;
            StartCoroutine(SetText(activatedDialog.steps[totalStep].text));
            iconImage.sprite = activatedDialog.steps[totalStep].icon;
            if (activatedDialog.steps[totalStep].cameraTarget != null)
                scripts.player.virtualCamera.Follow = activatedDialog.steps[totalStep].cameraTarget;
            else
                scripts.player.virtualCamera.Follow = scripts.player.transform;
        }

        if (activatedDialog.steps[totalStep].activatedCutsceneStep != -1)
            scripts.cutsceneManager.ActivateCutsceneStep(activatedDialog.steps[totalStep].activatedCutsceneStep);
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

    private IEnumerator BigPictureAnimate(dialog totalDialog)
    {
        bigPicture.sprite = totalDialog.bigPicture;
        yield return new WaitForSeconds(0.35f);
        bigPicture.sprite = totalDialog.bigPictureSecond;
        yield return new WaitForSeconds(0.35f);
        if (totalDialog.bigPictureThird != null)
        {
            bigPicture.sprite = totalDialog.bigPictureThird;
            yield return new WaitForSeconds(0.35f);
        }
        StartCoroutine(BigPictureAnimate(totalDialog));
    }

    private IEnumerator ActivateUIMainMenuWithDelay(dialog totalDialog, float delay)
    {
        yield return new WaitForSeconds(delay);
        dialogMenu.transform.Find("mainMenu").GetComponent<RectTransform>().DOScale(new Vector3(1f, 1f, 1f), 0.4f).SetEase(Ease.InQuart);
    }

    private void Update()
    {
        if (activatedDialog != null)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (animatingText)
                {
                    animatingText = false;
                    StopAllCoroutines();
                    textDialog.text = activatedDialog.steps[totalStep].text;
                }
                else
                    DialogMoveNext();
            }
        }
    }
}

[System.Serializable]
public class dialog
{
    [Header("Main")]
    public string nameDialog;
    public dialogStep[] steps = new dialogStep[0];
    [Header("AfterEnd")]
    public dialog startNewDialogAfterEnd;
    public bool darkAfterEnd;
    [Tooltip("Работает только с darkAfterEnd")] public Transform posAfterEnd;
    [Header("Other")]
    public string noteAdd;
    public Sprite bigPicture, bigPictureSecond, bigPictureThird;
    public int activatedCutsceneStepAtEnd = -1;
    public bool disableFadeAtEnd;
    public float mainPanelStartDelay; // Задержка
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
    public int activatedCutsceneStep = -1;
    public Transform cameraTarget;
}
