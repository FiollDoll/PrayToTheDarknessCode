using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class dialogsManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textName, textNameInChoice, textDialog;
    [SerializeField] private Image iconImage, bigPicture;
    [SerializeField] private Image noViewPanel;

    [SerializeField] private Sprite nullSprite;
    private string totalMode;
    private bool animatingText;
    public dialog[] dialogs = new dialog[0];
    private dialog activatedDialog;
    private dialogStepChoice selectedChoice;
    private dialogStep selectedStep;
    [SerializeField] private int totalStep;
    public GameObject dialogMenu, mainDialogMenu, choiceDialogMenu;
    public GameObject buttonChoicePrefab;
    private bool dialogEnding;
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
                    choiceDialogMenu.gameObject.SetActive(false);
                    mainDialogMenu.gameObject.SetActive(true);
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
                    // Вот здесь ещё подделать, если steps = 0, а choice нет
                    selectedStep = activatedDialog.steps[0];
                    textName.text = selectedStep.totalNpc.name;
                    if (GameObject.Find(selectedStep.totalNpc.nameInWorld) && selectedStep.animateTalking)
                        GameObject.Find(selectedStep.totalNpc.nameInWorld).GetComponent<Animator>().SetBool("talk", true);
                    StartCoroutine(SetText(selectedStep.text));
                    totalMode = "dialog";
                    iconImage.sprite = selectedStep.icon;
                    break;
                }
            }
        }
    }

    private void DialogCLose()
    {
        dialogEnding = true;
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
            dialogEnding = false;
        });
    }

    public void ActivateChoiceStep(int id)
    {
        totalMode = "choice";
        selectedChoice = activatedDialog.dialogsChoices[id];
        selectedChoice.readed = true;
        choiceDialogMenu.gameObject.SetActive(false);
        mainDialogMenu.gameObject.SetActive(true);
        totalStep = 0;
        selectedStep = activatedDialog.steps[0];
        textName.text = selectedChoice.steps[0].totalNpc.name;
        if (GameObject.Find(selectedChoice.steps[0].totalNpc.nameInWorld) && selectedChoice.steps[0].animateTalking)
            GameObject.Find(selectedChoice.steps[0].totalNpc.nameInWorld).GetComponent<Animator>().SetBool("talk", true);
        StartCoroutine(SetText(selectedChoice.steps[0].text));
    }

    private void DialogMoveNext()
    {
        void AddStep()
        {
            totalStep++;
            if (totalMode == "dialog")
                selectedStep = activatedDialog.steps[totalStep];
            else
                selectedStep = selectedChoice.steps[totalStep];
            textName.text = selectedStep.totalNpc.name;
            if (GameObject.Find(selectedStep.totalNpc.nameInWorld) && selectedStep.animateTalking)
                GameObject.Find(selectedStep.totalNpc.nameInWorld).GetComponent<Animator>().SetBool("talk", true);
            StartCoroutine(SetText(selectedStep.text));
            iconImage.sprite = selectedStep.icon;
            if (selectedStep.cameraTarget != null)
                scripts.player.virtualCamera.Follow = selectedStep.cameraTarget;
            else
                scripts.player.virtualCamera.Follow = scripts.player.transform;
            if (selectedStep.questStart != "")
                scripts.quests.ActivateQuest(selectedStep.questStart);
        }
        
        void ActivateChoiceMenu()
        {
            choiceDialogMenu.gameObject.SetActive(true);
            mainDialogMenu.gameObject.SetActive(false);
            foreach (Transform child in choiceDialogMenu.transform.Find("choices"))
                Destroy(child.gameObject);

            for (int i = 0; i < activatedDialog.dialogsChoices.Length; i++)
            {
                var obj = Instantiate(buttonChoicePrefab, new Vector3(0, 0, 0), Quaternion.identity, choiceDialogMenu.transform.Find("choices"));
                obj.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = activatedDialog.dialogsChoices[i].textQuestion;
                int num = i;
                obj.GetComponent<Button>().onClick.AddListener(delegate { ActivateChoiceStep(num); });
                if (activatedDialog.dialogsChoices[i].readed)
                    obj.GetComponent<Button>().interactable = false;
            }
        }

        if (GameObject.Find(selectedStep.totalNpc.nameInWorld) && selectedStep.animateTalking)
            GameObject.Find(selectedStep.totalNpc.nameInWorld).GetComponent<Animator>().SetBool("talk", false);
        if (totalMode == "dialog") // Окончание обычного диалога
        {
            if ((totalStep + 1) == activatedDialog.steps.Length)
            {
                if (activatedDialog.dialogsChoices.Length == 0)
                    DialogCLose();
                else
                    ActivateChoiceMenu();
            }
            else
                AddStep();
        }
        else if (totalMode == "choice")
        {
            if ((totalStep + 1) == selectedChoice.steps.Length)
            {
                if (selectedChoice.returnToStartChoices)
                    ActivateChoiceMenu();
                else
                    DialogCLose();
            }
            else
                AddStep();
        }

        if (selectedStep.activatedCutsceneStep != -1)
            scripts.cutsceneManager.ActivateCutsceneStep(selectedStep.activatedCutsceneStep);
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
                if (!dialogEnding)
                {
                    if (animatingText)
                    {
                        animatingText = false;
                        StopAllCoroutines();
                        textDialog.text = selectedStep.text;
                    }
                    else
                        DialogMoveNext();
                }
            }
        }
    }
}

[System.Serializable]
public class dialog
{
    [Header("Main")]
    public string nameDialog;
    [Tooltip("Обычные этапы диалога")] public dialogStep[] steps = new dialogStep[0];
    [Tooltip("Диалоги с выбором варианта. Начинается после steps")] public dialogStepChoice[] dialogsChoices = new dialogStepChoice[0];
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
    public NPC totalNpc;
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
    public bool animateTalking = true;
    public enum iconMood { standart, happy, angry, sad, scary, wonder, confusion }
    public iconMood iconMoodSelected;
    public Sprite icon
    {
        get
        {
            if (iconMoodSelected == iconMood.standart)
                return totalNpc.icon.standartIcon;
            else if (iconMoodSelected == iconMood.happy)
                return totalNpc.icon.happyIcon;
            else if (iconMoodSelected == iconMood.angry)
                return totalNpc.icon.angryIcon;
            else if (iconMoodSelected == iconMood.sad)
                return totalNpc.icon.sadIcon;
            else if (iconMoodSelected == iconMood.scary)
                return totalNpc.icon.scaryIcon;
            else if (iconMoodSelected == iconMood.wonder)
                return totalNpc.icon.wonderIcon;
            else if (iconMoodSelected == iconMood.confusion)
                return totalNpc.icon.confusionIcon;
            return null;
        }
    }
    public int activatedCutsceneStep = -1;
    public string questStart;
    public Transform cameraTarget;
}

[System.Serializable]
public class dialogStepChoice
{
    [HideInInspector]
    public string textQuestion
    {
        get
        {
            if (PlayerPrefs.GetString("language") == "ru")
                return ruTextQuestion;
            else
                return enTextQuestion;
        }
    }
    public string ruTextQuestion, enTextQuestion;
    public dialogStep[] steps = new dialogStep[0];
    public bool readed;
    public bool returnToStartChoices;
}
