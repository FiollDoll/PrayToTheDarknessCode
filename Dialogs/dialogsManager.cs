using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using MyBox;

public class DialogsManager : MonoBehaviour, IMenuable
{
    [Header("All dialogs")] public Dialog[] dialogs = new Dialog[0];
    private Dictionary<string, Dialog> _dialogsDict = new Dictionary<string, Dialog>();

    [Header("Current dialog")] public int totalStep;
    private Dialog _activatedDialog;
    private StepBranch _selectedBranch;
    private DialogStep _selectedStep;

    [Header("Prefabs")] [SerializeField] private GameObject buttonChoicePrefab;

    [Foldout("Scene Objects", true)] [SerializeField]
    private GameObject dialogMenu;

    public GameObject menu
    {
        get => dialogMenu;
    }

    [SerializeField] private GameObject choicesContainer;

    [SerializeField] private TextMeshProUGUI textNameMain, textDialogMain, textDialogSub;
    [SerializeField] private GameObject mainDialogMenu, choiceDialogMenu, subDialogMenu;
    [SerializeField] private Image iconImage;
    [SerializeField] private Image bigPicture;
    [SerializeField] private AdaptiveScrollView adaptiveScrollViewChoice;
    private RectTransform _mainDialogMenuTransform, _choiceDialogMenuTransform;
    private RectTransform _iconImageTransform;


    private bool _animatingText, _canStepNext;
    private Image _noViewPanel;
    private AllScripts _scripts;

    private void Start()
    {
        foreach (Dialog dialog in dialogs)
        {
            _dialogsDict.TryAdd(dialog.nameDialog, dialog);
            dialog.UpdateBranchesDict();
        }

        _scripts = GameObject.Find("scripts").GetComponent<AllScripts>();
        _noViewPanel = _scripts.main.noViewPanel;
        _mainDialogMenuTransform = mainDialogMenu.GetComponent<RectTransform>();
        _choiceDialogMenuTransform = choiceDialogMenu.GetComponent<RectTransform>();
        _iconImageTransform = iconImage.GetComponent<RectTransform>();
    }

    // ReSharper disable Unity.PerformanceAnalysis
    /// <summary>
    /// Поиск диалога
    /// </summary>
    /// <param name="nameDialog">Имя диалога, который ищем</param>
    /// <returns>Возвращает диалог</returns>
    private Dialog GetDialog(string nameDialog)
    {
        return _dialogsDict[nameDialog];
    }

    /// <summary>
    /// Проверка - пора ли активировать выборы и есть ли они вообще?
    /// </summary>
    /// <returns></returns>
    private bool CheckCanChoice()
    {
        return _selectedBranch.choices.Length != 0 && totalStep + 1 == _selectedBranch.dialogSteps.Length;
    }

    public void ManageActivationMenu()
    {
        
    }
    
    /// <summary>
    /// Активация диалоговых меню
    /// </summary>
    private void ActivateDialogWindow()
    {
        if (_activatedDialog.bigPicture) // Сначала затемнение
        {
            _scripts.main.ActivateNoVision(1.2f, () =>
            {
                bigPicture.gameObject.SetActive(true);
                bigPicture.sprite = _activatedDialog.bigPicture;
            });
        }

        if (!CheckCanChoice()) // Потом уже управление менюшками
        {
            if (_activatedDialog.styleOfDialog == Dialog.DialogStyle.Main)
                mainDialogMenu.gameObject.SetActive(true);
            else
                subDialogMenu.gameObject.SetActive(true);
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
        if (_activatedDialog != null)
        {
            _activatedDialog = new Dialog(); // Если уже назначен - очищаем
            StopAllCoroutines();
        }

        Dialog newDialog = GetDialog(nameDialog);
        if (newDialog == null) return;
        if (newDialog.read) return;

        _activatedDialog = newDialog;
        _selectedBranch = _activatedDialog.stepBranches[0];
        _selectedStep = _selectedBranch.dialogSteps[0];

        dialogMenu.gameObject.SetActive(true);
        _canStepNext = false;
        _scripts.interactions.lockInter = _activatedDialog.canInter;
        ActivateDialogWindow();

        if (!_activatedDialog.moreRead)
            _activatedDialog.read = true;
        _scripts.player.canMove = _activatedDialog.canMove;

        _scripts.main.EndCursedText(textDialogMain);

        if (_activatedDialog.mainPanelStartDelay == 0)
            OpenPanels();
        else
            StartCoroutine(ActivateUIMainMenuWithDelay(
                _activatedDialog.mainPanelStartDelay));

        if (!CheckCanChoice())
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
        _scripts.interactions.lockInter = false;
        if (_activatedDialog.activatedCutsceneStepAtEnd != -1)
            _scripts.cutsceneManager.ActivateCutsceneStep(_activatedDialog.activatedCutsceneStepAtEnd);

        if (_activatedDialog.bigPicture)
            _scripts.main.ActivateNoVision(1.5f, DoActionToClose);
        else if (_activatedDialog.darkAfterEnd)
            _scripts.main.ActivateNoVision(1.2f, DoActionToClose);
        else
            DoActionToClose();
    }

    /// <summary>
    /// Выполнить действия для закрытия диалога
    /// </summary>
    private void DoActionToClose()
    {
        totalStep = 0;
        dialogMenu.gameObject.SetActive(false);
        bigPicture.gameObject.SetActive(false);
        mainDialogMenu.gameObject.SetActive(false);
        subDialogMenu.gameObject.SetActive(false);
        if (_activatedDialog.posAfterEnd)
            _scripts.player.transform.localPosition = _activatedDialog.posAfterEnd.position;

        if (_activatedDialog.nextStepQuest)
            _scripts.questsSystem.NextStep();
        _scripts.player.canMove = true;
        _scripts.cutsceneManager.totalCutscene = new Cutscene();
        _scripts.main.EndCursedText(textDialogMain);
        _scripts.player.virtualCamera.Follow = _scripts.player.transform;
        if (_activatedDialog.noteAdd != "")
            _scripts.notebook.AddNote(_activatedDialog.noteAdd);
        string newDialog = _activatedDialog.startNewDialogAfterEnd;
        _activatedDialog = null;
        _canStepNext = true;
        if (newDialog != "")
            ActivateDialog(newDialog);
    }

    // ReSharper disable Unity.PerformanceAnalysis
    /// <summary>
    /// Активация + генерация меню с выборами
    /// </summary>
    /// <param name="setScale"></param>
    private void ActivateChoiceMenu(bool setScale = false)
    {
        choiceDialogMenu.gameObject.SetActive(true);
        adaptiveScrollViewChoice.UpdateContentSize();
        if (setScale)
            _choiceDialogMenuTransform.localScale = new Vector2(1f, 1f);
        foreach (Transform child in choicesContainer.transform)
            Destroy(child.gameObject);

        for (int i = 0; i < _selectedBranch.choices.Length; i++)
        {
            var obj = Instantiate(buttonChoicePrefab, new Vector3(0, 0, 0), Quaternion.identity,
                choicesContainer.transform);
            obj.transform.Find("Text").GetComponent<TextMeshProUGUI>().text =
                _selectedBranch.choices[i].textQuestion.text;
            int num = i;
            obj.GetComponent<Button>().onClick.AddListener(delegate { ActivateChoiceStep(num); });
            if (_selectedBranch.choices[i].readed)
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
        DialogChoice choice = _selectedBranch.choices[id];
        choiceDialogMenu.gameObject.SetActive(false);
        totalStep = 0;
        if (!choice.moreRead)
            choice.readed = true;
        ChangeDialogBranch(choice.nameNewBranch);
    }

    private void ChangeDialogBranch(string nameOfBranch)
    {
        StepBranch newBranch = _activatedDialog.FindBranch(nameOfBranch);
        _selectedBranch = newBranch;
        _selectedStep = _selectedBranch.dialogSteps[0];
        ActivateDialogWindow();

        if (!CheckCanChoice())
            DialogUpdateAction();
        else
            ActivateChoiceMenu();
    }

    /// <summary>
    /// Продвижение диалога далее
    /// </summary>
    private void DialogMoveNext()
    {
        void AddStep()
        {
            totalStep++;
            _selectedStep = _selectedBranch.dialogSteps[totalStep];
            DialogUpdateAction();
        }

        _selectedStep.totalNpc.animator?.SetBool("talk", false);

        // Окончание обычного диалога
        if (totalStep + 1 == _selectedBranch.dialogSteps.Length)
        {
            // Если выбора нет.
            if (!CheckCanChoice())
                DialogCLose();
            else
                ActivateChoiceMenu();
            return;
        }

        AddStep();
    }

    /// <summary>
    /// Обновить текст и выполнить иные действия
    /// </summary>
    private void DialogUpdateAction()
    {
        if (_activatedDialog.styleOfDialog == Dialog.DialogStyle.Main)
        {
            textNameMain.text = _selectedStep.totalNpc.nameOfNpc.text;
            iconImage.sprite = _selectedStep.icon;
            iconImage.SetNativeSize();
            _iconImageTransform.DOPunchAnchorPos(new Vector3(1, 1, 1), 5f, 3);
        }

        foreach (Npc totalNpc in _scripts.main.allNpc)
        {
            if (!totalNpc.canMeet) continue;
            if (totalNpc.nameOfNpc != _selectedStep.totalNpc.nameOfNpc) continue;
            if (_scripts.player.familiarNpc.Contains(totalNpc)) continue;
            _scripts.player.familiarNpc.Add(totalNpc);
            break;
        }

        _selectedStep.totalNpc.animator?.SetBool("talk", true);

        if (_selectedStep.cursedText)
            _scripts.main.SetCursedText(textDialogMain, Random.Range(5, 40));
        else
            StartCoroutine(SetText());

        _scripts.player.virtualCamera.Follow =
            _selectedStep.cameraTarget ? _selectedStep.cameraTarget : _scripts.player.transform;

        _scripts.questsSystem.ActivateQuest(_selectedStep.questStart, true);
        _selectedStep.totalNpc.relationshipWithPlayer += _selectedStep.changeRelationship;
        _scripts.cutsceneManager.ActivateCutsceneStep(_selectedStep.activatedCutsceneStep);
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
        if (_activatedDialog == null) return;
        if (_selectedStep.delayAfterNext == 0)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (!_canStepNext) return;

                if (_animatingText)
                {
                    _animatingText = false;
                    StopAllCoroutines();
                    if (_activatedDialog.styleOfDialog == Dialog.DialogStyle.Main)
                        textDialogMain.text = _selectedStep.dialogText.text;
                    else
                        textDialogSub.text = _selectedStep.dialogText.text;
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
        textDialogSub.text = "";
        _animatingText = true;
        char[] textChar = _selectedStep.dialogText.text.ToCharArray();
        foreach (char tChar in textChar)
        {
            if (!_animatingText) continue;
            if (_activatedDialog.styleOfDialog == Dialog.DialogStyle.Main)
                textDialogMain.text += tChar;
            else
                textDialogSub.text += tChar;
            yield return new WaitForSeconds(0.05f);
        }

        _animatingText = false;
        if (_selectedStep.delayAfterNext != 0)
        {
            yield return new WaitForSeconds(_selectedStep.delayAfterNext);
            if (totalStep + 1 == _activatedDialog.stepBranches.Length) // Завершение
                DialogCLose();
            else
            {
                if (!CheckCanChoice())
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