using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using MyBox;
using LastFramework;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class DialogsManager : MonoBehaviour, IMenuable
{
    public static DialogsManager Instance { get; private set; }

    // Все диалоги
    private readonly Dictionary<string, Dialog> _dialogsDict = new Dictionary<string, Dialog>();

    [Header("Current dialog")] public int totalStep;
    [HideInInspector] public Dialog activatedDialog;
    [HideInInspector] public StepBranch selectedBranch;
    [HideInInspector] public DialogStep selectedStep;
    private List<Npc> _npcInTotalDialog = new List<Npc>();
    [Header("Prefabs")] [SerializeField] private GameObject buttonChoicePrefab;
    [SerializeField] private Npc nothingNpc; // Костыль. Вырезать

    [Header("Preferences")] [SerializeField]
    private Color dontSelectedColor;

    [Foldout("Scene Objects", true)] [SerializeField]
    private GameObject dialogMenu;

    public GameObject menu => dialogMenu;

    [SerializeField] private GameObject choicesContainer;

    [SerializeField] private TextMeshProUGUI textNameMain, textDialogMain, textDialogSub, textDialogBigPicture;
    [SerializeField] private GameObject mainDialogMenu, choiceDialogMenu, subDialogMenu, bigPictureMenu;
    [SerializeField] private Image firstTalkerIcon, secondTalkerIcon, thirdTalkerIcon, iconImageChoice;
    [SerializeField] private Image bigPicture;
    [SerializeField] private AdaptiveScrollView adaptiveScrollViewChoice;
    private RectTransform _mainDialogMenuTransform, _choiceDialogMenuTransform;

    private bool _animatingText, _canStepNext;

    private void Awake()
    {
        Instance = this;
    }

    public void Initialize()
    {
        DialogsLoader dl = new DialogsLoader();
        List<Dialog> dialogs = dl.LoadDialogs();
        foreach (Dialog dialog in dialogs)
        {
            _dialogsDict.TryAdd(dialog.nameDialog, dialog);
            dialog.UpdateDialogDicts();
        }

        _mainDialogMenuTransform = mainDialogMenu.GetComponent<RectTransform>();
        _choiceDialogMenuTransform = choiceDialogMenu.GetComponent<RectTransform>();
        _mainDialogMenuTransform.DOPivotY(3f, 0.01f);
        _choiceDialogMenuTransform.DOPivotY(3f, 0.01f);
        activatedDialog = null;
        selectedBranch = null;
        selectedStep = null;
    }

    // ReSharper disable Unity.PerformanceAnalysis
    /// <summary>
    /// Поиск диалога
    /// </summary>
    /// <param name="nameDialog">Имя диалога, который ищем</param>
    /// <returns>Возвращает диалог</returns>
    private Dialog GetDialog(string nameDialog)
    {
        return _dialogsDict.GetValueOrDefault(nameDialog);
    }

    /// <summary>
    /// Проверка - пора ли активировать выборы и есть ли они вообще?
    /// </summary>
    /// <returns></returns>
    private bool CanChoice()
    {
        return selectedBranch.choices.Count != 0 && totalStep == selectedBranch.dialogSteps.Count;
    }

    public void ManageActivationMenu()
    {
    }

    /// <summary>
    /// Активация диалоговых меню
    /// </summary>
    private void ActivateDialogWindow()
    {
        if (!CanChoice()) // Потом уже управление менюшками
        {
            switch (activatedDialog.styleOfDialog)
            {
                case Dialog.DialogStyle.Main:
                    mainDialogMenu.gameObject.SetActive(true);
                    break;
                case Dialog.DialogStyle.BigPicture:
                    GameMenuManager.Instance.ActivateNoVision(1.2f,
                        () => { bigPictureMenu.gameObject.SetActive(true); });
                    break;
                case Dialog.DialogStyle.SubMain:
                    subDialogMenu.gameObject.SetActive(true);
                    break;
            }
        }
        else
            choiceDialogMenu.gameObject.SetActive(true);
    }

    /// <summary>
    ///  Активирует новый диалог
    /// </summary>
    /// <param name="nameDialog">Название диалога</param>
    public void ActivateDialog(string nameDialog) // Старт диалога
    {
        Dialog newDialog = GetDialog(nameDialog);
        if (newDialog == null) return;
        if (newDialog.read) return;

        if (activatedDialog != null)
        {
            totalStep = 0;
            StopAllCoroutines();
        }

        activatedDialog = newDialog;
        selectedBranch = activatedDialog.stepBranches[0];
        selectedStep = selectedBranch.dialogSteps[0];
        Player.Instance.canMove = activatedDialog.canMove;
        Interactions.Instance.lockInter = !activatedDialog.canInter;
        if (!activatedDialog.moreRead)
            activatedDialog.read = true;
        _canStepNext = false;

        dialogMenu.gameObject.SetActive(true);
        ActivateDialogWindow();
        CameraManager.Instance.CameraZoom(-5f, true);
        TextManager.EndCursedText(textDialogMain);

        StartCoroutine(ActivateUIMainMenuWithDelay(activatedDialog.mainPanelStartDelay));

        if (!CanChoice())
            DialogUpdateAction();
        else
            ActivateChoiceMenu();
    }

    /// <summary>
    /// Закрытие диалога
    /// </summary>
    public void DialogCLose()
    {
        _canStepNext = false;
        Interactions.Instance.lockInter = false;

        if (activatedDialog.styleOfDialog == Dialog.DialogStyle.BigPicture)
            GameMenuManager.Instance.ActivateNoVision(1.5f, DoActionToClose);
        else if (activatedDialog.darkAfterEnd)
            GameMenuManager.Instance.ActivateNoVision(1.2f, DoActionToClose);
        else
            DoActionToClose();
    }

    /// <summary>
    /// Выполнить действия для закрытия диалога
    /// </summary>
    private void DoActionToClose()
    {
        totalStep = 0;
        StopAllCoroutines();
        _choiceDialogMenuTransform.DOPivotY(3f, 0.3f);
        _mainDialogMenuTransform.DOPivotY(3f, 0.3f).OnComplete(() =>
        {
            CameraManager.Instance.CameraZoom(0, true);
            dialogMenu.gameObject.SetActive(false);
            mainDialogMenu.gameObject.SetActive(false);
            subDialogMenu.gameObject.SetActive(false);
            bigPictureMenu.gameObject.SetActive(false);
            _npcInTotalDialog = new List<Npc>();
            firstTalkerIcon.sprite = nothingNpc.GetStyleIcon(NpcIcon.IconMood.Standart);
            secondTalkerIcon.sprite = nothingNpc.GetStyleIcon(NpcIcon.IconMood.Standart);
            thirdTalkerIcon.sprite = nothingNpc.GetStyleIcon(NpcIcon.IconMood.Standart);

            //if (_activatedDialog.posAfterEnd)
            //Player.Instance.transform.localPosition = _activatedDialog.posAfterEnd.position;

            Player.Instance.canMove = true;
            TextManager.EndCursedText(textDialogMain);
            Player.Instance.virtualCamera.Follow = Player.Instance.transform;
            activatedDialog = null;
            _canStepNext = true;
        });
    }

    // ReSharper disable Unity.PerformanceAnalysis
    /// <summary>
    /// Активация + генерация меню с выборами
    /// </summary>
    /// <param name="setScale"></param>
    private void ActivateChoiceMenu(bool setScale = false)
    {
        choiceDialogMenu.gameObject.SetActive(true);
        iconImageChoice.sprite = Player.Instance.npcEntity.GetStyleIcon(NpcIcon.IconMood.Standart);
        adaptiveScrollViewChoice.UpdateContentSize();
        Player.Instance.virtualCamera.Follow = Player.Instance.transform;
        if (setScale)
            _choiceDialogMenuTransform.localScale = new Vector2(1f, 1f);
        foreach (Transform child in choicesContainer.transform)
            Destroy(child.gameObject);

        for (int i = 0; i < selectedBranch.choices.Count; i++)
        {
            var obj = Instantiate(buttonChoicePrefab, new Vector3(0, 0, 0), Quaternion.identity,
                choicesContainer.transform);
            obj.transform.Find("Text").GetComponent<TextMeshProUGUI>().text =
                selectedBranch.choices[i].textQuestion.text;
            int num = i;
            obj.GetComponent<Button>().onClick.AddListener(delegate { ActivateChoiceStep(num); });
            if (selectedBranch.choices[i].read)
                obj.GetComponent<Button>().interactable = false;
        }

        adaptiveScrollViewChoice.UpdateContentSize();
    }

    /// <summary>
    /// Активация выбора
    /// </summary>
    /// <param name="id"></param>
    private void ActivateChoiceStep(int id)
    {
        DialogChoice choice = selectedBranch.choices[id];
        if (choice.nameNewBranch == "") // Быстрое закрытие диалога
            DialogCLose();
        else
        {
            choiceDialogMenu.gameObject.SetActive(false);
            totalStep = 0;
            if (!choice.moreRead)
                choice.read = true;
            ChangeDialogBranch(choice.nameNewBranch);
        }
    }

    private void ChangeDialogBranch(string nameOfBranch)
    {
        StepBranch newBranch = activatedDialog.FindBranch(nameOfBranch);
        selectedBranch = newBranch;
        selectedStep = selectedBranch.dialogSteps[0];
        ActivateDialogWindow();

        if (!CanChoice())
            DialogUpdateAction();
        else
            ActivateChoiceMenu();
    }

    /// <summary>
    /// Продвижение диалога далее
    /// </summary>
    private void DialogMoveNext()
    {
        bool AddStep()
        {
            totalStep++;
            if (totalStep != selectedBranch.dialogSteps.Count)
            {
                selectedStep = selectedBranch.dialogSteps[totalStep];
                DialogUpdateAction();
                return true;
            }

            // Продолжение не найдено, выдаём false
            return false;
        }

        // Окончание обычного диалога
        if (!AddStep())
        {
            // Если выбора нет.
            if (!CanChoice())
                DialogCLose();
            else
                ActivateChoiceMenu();
        }
    }

    /// <summary>
    /// Обновить текст и выполнить иные действия
    /// </summary>
    private void DialogUpdateAction()
    {
        selectedStep.UpdateStep();

        if (selectedStep.totalNpcName != "nothing" && !_npcInTotalDialog.Contains(selectedStep.totalNpc))
            _npcInTotalDialog.Add(selectedStep.totalNpc);


        if (activatedDialog.styleOfDialog is Dialog.DialogStyle.Main)
        {
            textNameMain.text = selectedStep.totalNpc.nameOfNpc.text;
            SetIcon();
        }
        else if (activatedDialog.styleOfDialog is Dialog.DialogStyle.BigPicture)
        {
            textDialogBigPicture.text = selectedStep.totalNpc.nameOfNpc.text;
            if (selectedStep.bigPictureName != "") // Меняем
                bigPicture.sprite = selectedStep.GetBigPicture();
        }

        foreach (Npc totalNpc in NpcManager.Instance.AllNpc)
        {
            if (!totalNpc.canMeet) continue;
            if (totalNpc.nameOfNpc != selectedStep.totalNpc.nameOfNpc) continue;
            if (Player.Instance.familiarNpc.Contains(totalNpc)) continue;
            Player.Instance.familiarNpc.Add(totalNpc);
            break;
        }

        if (selectedStep.cursedText)
            TextManager.SetCursedText(textDialogMain, Random.Range(5, 40));
        else
            StartCoroutine(SetText());

        selectedStep.fastChanges.ActivateChanges();

        CutsceneManager.Instance.ActivateCutsceneStep(selectedStep.activateCutsceneStep);

        if (selectedStep.stepSpeech != "")
            AudioManager.Instance.PlaySpeech(selectedStep.GetSpeech());
        else
            AudioManager.Instance.StopSpeech();
    }

    private void SetIcon()
    {
        if (selectedStep.totalNpc.nameInWorld == "nothing") return;

        firstTalkerIcon.color = dontSelectedColor;
        secondTalkerIcon.color = dontSelectedColor;
        thirdTalkerIcon.color = dontSelectedColor;

        switch (_npcInTotalDialog.Count)
        {
            case > 0 when selectedStep.totalNpc == _npcInTotalDialog[0]:
                firstTalkerIcon.sprite = selectedStep.icon;
                firstTalkerIcon.GetComponent<RectTransform>().DOPunchAnchorPos(new Vector3(1, 1, 1), 5f, 3);
                firstTalkerIcon.color = Color.white;
                break;
            case > 1 when selectedStep.totalNpc == _npcInTotalDialog[1]:
                secondTalkerIcon.sprite = selectedStep.icon;
                secondTalkerIcon.GetComponent<RectTransform>().DOPunchAnchorPos(new Vector3(1, 1, 1), 5f, 3);
                secondTalkerIcon.color = Color.white;
                break;
            case > 2 when selectedStep.totalNpc == _npcInTotalDialog[2]:
                thirdTalkerIcon.sprite = selectedStep.icon;
                thirdTalkerIcon.GetComponent<RectTransform>().DOPunchAnchorPos(new Vector3(1, 1, 1), 5f, 3);
                thirdTalkerIcon.color = Color.white;
                break;
        }
    }

    /// <summary>
    /// Включение анимаций появления менюшек
    /// </summary>
    private void OpenPanels()
    {
        // TODO: переделать
        _mainDialogMenuTransform.DOPivotY(0.5f, 0.3f).SetEase(Ease.InQuart)
            .OnComplete(() => { _canStepNext = true; });
        _choiceDialogMenuTransform.DOPivotY(0.5f, 0.3f).SetEase(Ease.InQuart)
            .OnComplete(() => { _canStepNext = true; });
    }

    private void Update()
    {
        if (activatedDialog == null) return;
        if (selectedStep.delayAfterNext == 0)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (!_canStepNext) return;

                if (_animatingText)
                {
                    _animatingText = false;
                    StopAllCoroutines();
                    if (activatedDialog.styleOfDialog == Dialog.DialogStyle.Main)
                        textDialogMain.text = selectedStep.dialogText.text;
                    else if (activatedDialog.styleOfDialog == Dialog.DialogStyle.BigPicture)
                        textDialogBigPicture.text = selectedStep.dialogText.text;
                    else
                        textDialogSub.text = selectedStep.dialogText.text;
                }
                else
                    DialogMoveNext();
            }
        }
    }

    /// <summary>
    ///  Посимвольная установка текста
    /// </summary>
    /// <returns></returns>
    private IEnumerator SetText()
    {
        textDialogMain.text = "";
        textDialogBigPicture.text = "";
        textDialogSub.text = "";
        _animatingText = true;
        char[] textChar = selectedStep.dialogText.text.ToCharArray();
        foreach (char tChar in textChar)
        {
            if (!_animatingText && activatedDialog != null) continue;
            if (activatedDialog?.styleOfDialog == Dialog.DialogStyle.Main)
                textDialogMain.text += tChar;
            else if (activatedDialog?.styleOfDialog == Dialog.DialogStyle.BigPicture)
                textDialogBigPicture.text += tChar;
            else
                textDialogSub.text += tChar;
            yield return new WaitForSeconds(1f / textChar.Length);
        }

        _animatingText = false;
        if (selectedStep.delayAfterNext != 0)
        {
            yield return new WaitForSeconds(selectedStep.delayAfterNext);
            if (totalStep + 1 == activatedDialog?.stepBranches.Count) // Завершение
                DialogCLose();
            else
            {
                if (!CanChoice())
                    DialogUpdateAction();
                else
                    ActivateChoiceMenu();
            }
        }
    }

    private IEnumerator ActivateUIMainMenuWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        OpenPanels();
    }
}