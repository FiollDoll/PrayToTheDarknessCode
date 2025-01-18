using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class DialogsManager : MonoBehaviour
{
    [Header("Все диалоги")] public Dialog[] dialogs = new Dialog[0];
    private Dictionary<string, Dialog> _dialogsDict = new Dictionary<string, Dialog>(); 

    [Header("Текущий диалог")] public int totalStep;

    private Dialog _activatedDialog;
    private StepBranch _selectedBranch;
    private DialogStep _selectedStep;

    [Header("Настройки")] public GameObject dialogMenu;
    [SerializeField] private GameObject choicesContainer;

    public GameObject buttonChoicePrefab;

    private TextMeshProUGUI _textNameMain, _textDialogMain, _textDialogSub;
    private GameObject _mainDialogMenu, _choiceDialogMenu, _subDialogMenu;
    private RectTransform _mainDialogMenuTransform, _choiceDialogMenuTransform;
    private Image _iconImage;
    private RectTransform _iconImageTransform;
    private Image _bigPicture;
    private AdaptiveScrollView _adaptiveScrollViewChoice;
    private bool _animatingText, _canStepNext;


    private Image _noViewPanel;
    private AllScripts _scripts;

    public void Initialize()
    {
        foreach (Dialog dialog in dialogs)
            _dialogsDict.Add(dialog.nameDialog, dialog);
        
        _scripts = GameObject.Find("scripts").GetComponent<AllScripts>();
        _noViewPanel = _scripts.main.noViewPanel;

        _mainDialogMenu = dialogMenu.transform.Find("mainMenu").gameObject;
        _mainDialogMenuTransform = _mainDialogMenu.GetComponent<RectTransform>();
        _mainDialogMenuTransform.DOPivotY(4f, 0.3f);
        _choiceDialogMenu = dialogMenu.transform.Find("choiceMenu").gameObject;
        _choiceDialogMenuTransform = _choiceDialogMenu.GetComponent<RectTransform>();
        _choiceDialogMenuTransform.DOPivotY(4f, 0.3f);
        _adaptiveScrollViewChoice = _choiceDialogMenu.transform.Find("Scroll View").GetComponent<AdaptiveScrollView>();
        _subDialogMenu = dialogMenu.transform.Find("subMainMenu").gameObject;

        _textNameMain = _mainDialogMenu.transform.Find("TextName").GetComponent<TextMeshProUGUI>();
        _textDialogMain = _mainDialogMenu.transform.Find("TextDialog").GetComponent<TextMeshProUGUI>();
        _iconImage = _mainDialogMenu.transform.Find("Image").GetComponent<Image>();
        _iconImageTransform = _iconImage.GetComponent<RectTransform>();
        _textDialogSub = _subDialogMenu.transform.Find("Text").GetComponent<TextMeshProUGUI>();
        _bigPicture = dialogMenu.transform.Find("bigPicture").gameObject.GetComponent<Image>();
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

    /// <summary>
    /// Активация диалоговых меню
    /// </summary>
    private void ActivateDialogWindow()
    {
        if (_activatedDialog.bigPicture) // Сначала затемнение
        {
            _scripts.main.ActivateNoVision(1.2f, () =>
            {
                _bigPicture.gameObject.SetActive(true);
                _bigPicture.sprite = _activatedDialog.bigPicture;
            });
        }
        
        if (!CheckCanChoice()) // Потом уже управление менюшками
        {
            if (_activatedDialog.styleOfDialog == Dialog.DialogStyle.Main)
                _mainDialogMenu.gameObject.SetActive(true);
            else
                _subDialogMenu.gameObject.SetActive(true);
        }
        else
            _choiceDialogMenu.gameObject.SetActive(true);
    }

    /// <summary>
    ///  Активирует новый диалог
    /// </summary>
    /// <param name="nameDialog">Название диалога</param>
    public void ActivateDialog(string nameDialog) // Старт диалога
    {
        _activatedDialog = new Dialog(); // Если уже назначен - очищаем

        Dialog newDialog = GetDialog((nameDialog));
        if (newDialog == null) return;
        if (newDialog.readed) return;
        
        _activatedDialog = newDialog;
        _selectedBranch = _activatedDialog.stepBranches[0];
        _selectedStep = _selectedBranch.dialogSteps[0];
        
        dialogMenu.gameObject.SetActive(true);
        _canStepNext = false;
        _scripts.interactions.lockInter = _activatedDialog.canInter;
        ActivateDialogWindow();

        if (!_activatedDialog.moreRead)
            _activatedDialog.readed = true;
        _scripts.player.canMove = _activatedDialog.canMove;

        _scripts.main.EndCursedText(_textDialogMain);

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
        _bigPicture.gameObject.SetActive(false);
        _mainDialogMenu.gameObject.SetActive(false);
        _subDialogMenu.gameObject.SetActive(false);
        if (_activatedDialog.posAfterEnd)
            _scripts.player.transform.localPosition = _activatedDialog.posAfterEnd.position;

        if (_activatedDialog.nextStepQuest)
            _scripts.questsSystem.NextStep();
        _scripts.player.canMove = true;
        _scripts.main.EndCursedText(_textDialogMain);
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
        _choiceDialogMenu.gameObject.SetActive(true);
        _adaptiveScrollViewChoice.UpdateContentSize();
        if (setScale)
            _choiceDialogMenuTransform.localScale = new Vector2(1f, 1f);
        foreach (Transform child in choicesContainer.transform)
            Destroy(child.gameObject);

        for (int i = 0; i < _selectedBranch.choices.Length; i++)
        {
            var obj = Instantiate(buttonChoicePrefab, new Vector3(0, 0, 0), Quaternion.identity,
                choicesContainer.transform);
            obj.transform.Find("Text").GetComponent<TextMeshProUGUI>().text =
                _selectedBranch.choices[i].textQuestion;
            int num = i;
            obj.GetComponent<Button>().onClick.AddListener(delegate { ActivateChoiceStep(num); });
            if (_selectedBranch.choices[i].readed)
                obj.GetComponent<Button>().interactable = false;
        }

        _adaptiveScrollViewChoice.UpdateContentSize();
    }

    /// <summary>
    /// Активация выбора
    /// </summary>
    /// <param name="id"></param>
    private void ActivateChoiceStep(int id)
    {
        DialogChoice choice = _selectedBranch.choices[id];
        _choiceDialogMenu.gameObject.SetActive(false);
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
            _textNameMain.text = _selectedStep.totalNpc.nameOfNpc;
            _iconImage.sprite = _selectedStep.icon;
            _iconImage.SetNativeSize();
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
            _scripts.main.SetCursedText(_textDialogMain, Random.Range(5, 40));
        else
            StartCoroutine(SetText(_selectedStep.text));

        _scripts.player.virtualCamera.Follow =
            _selectedStep.cameraTarget ? _selectedStep.cameraTarget : _scripts.player.transform;

        if (_selectedStep.questStart != "")
            _scripts.questsSystem.ActivateQuest(_selectedStep.questStart, true);
        if (_selectedStep.activatedCutsceneStep != -1)
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
                        _textDialogMain.text = _selectedStep.text;
                    else
                        _textDialogSub.text = _selectedStep.text;
                }
                else
                    DialogMoveNext();
            }
        }
    }

    /// <summary>
    ///  Посимвольная установка текста
    /// </summary>
    /// <param name="text">Текст</param>
    /// <returns></returns>
    private IEnumerator SetText(string text)
    {
        _textDialogMain.text = "";
        _textDialogSub.text = "";
        _animatingText = true;
        char[] textChar = text.ToCharArray();
        foreach (char tChar in textChar)
        {
            if (!_animatingText) continue;
            if (_activatedDialog.styleOfDialog == Dialog.DialogStyle.Main)
                _textDialogMain.text += tChar;
            else
                _textDialogSub.text += tChar;
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