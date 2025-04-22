using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using MyBox;
using LastFramework;
using Random = UnityEngine.Random;

public class DialogsManager : MonoBehaviour, IMenuable
{
    public static DialogsManager Instance { get; private set; }

    // Все диалоги
    private readonly Dictionary<string, Dialog> _dialogsDict = new Dictionary<string, Dialog>();

    [Header("Current dialog")] public int totalStep;
    private Dialog _activatedDialog;
    private StepBranch _selectedBranch;
    private DialogStep _selectedStep;

    [Header("Prefabs")] [SerializeField] private GameObject buttonChoicePrefab;
    [SerializeField] private Npc nothingNpc; // Костыль. Вырезать

    [Foldout("Scene Objects", true)] [SerializeField]
    private GameObject dialogMenu;

    public GameObject menu => dialogMenu;

    [SerializeField] private GameObject choicesContainer;

    [SerializeField] private TextMeshProUGUI textNameMain, textDialogMain, textDialogSub, textDialogBigPicture;
    [SerializeField] private GameObject mainDialogMenu, choiceDialogMenu, subDialogMenu, bigPictureMenu;
    [SerializeField] private Image iconImageMain, iconImageChoice;
    [SerializeField] private Image bigPicture;
    [SerializeField] private AdaptiveScrollView adaptiveScrollViewChoice;
    private RectTransform _mainDialogMenuTransform, _choiceDialogMenuTransform;
    private RectTransform _iconImageTransform;

    private bool _animatingText, _canStepNext;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        List<Dialog> dialogs = DialogsLoader.Instance.LoadDialogs();
        foreach (Dialog dialog in dialogs)
        {
            _dialogsDict.TryAdd(dialog.nameDialog, dialog);
            dialog.UpdateDialogDicts();
        }

        _mainDialogMenuTransform = mainDialogMenu.GetComponent<RectTransform>();
        _choiceDialogMenuTransform = choiceDialogMenu.GetComponent<RectTransform>();
        _iconImageTransform = iconImageMain.GetComponent<RectTransform>();
        _mainDialogMenuTransform.DOPivotY(3f, 0.01f);
        _choiceDialogMenuTransform.DOPivotY(3f, 0.01f);
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
        return _selectedBranch.choices.Count != 0 && totalStep == _selectedBranch.dialogSteps.Count;
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
            switch (_activatedDialog.styleOfDialog)
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
        if (_activatedDialog != null)
        {
            DialogCLose();
            StopAllCoroutines();
        }

        Dialog newDialog = GetDialog(nameDialog);
        if (newDialog == null) return;
        if (newDialog.read) return;

        _activatedDialog = newDialog;
        _selectedBranch = _activatedDialog.stepBranches[0];
        _selectedStep = _selectedBranch.dialogSteps[0];
        Player.Instance.canMove = _activatedDialog.canMove;
        Interactions.Instance.lockInter = !_activatedDialog.canInter;
        if (!_activatedDialog.moreRead)
            _activatedDialog.read = true;
        _canStepNext = false;

        dialogMenu.gameObject.SetActive(true);
        ActivateDialogWindow();
        CameraManager.Instance.CameraZoom(-5f, true);
        TextManager.EndCursedText(textDialogMain);

        StartCoroutine(ActivateUIMainMenuWithDelay(_activatedDialog.mainPanelStartDelay));

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

        if (_activatedDialog.styleOfDialog == Dialog.DialogStyle.BigPicture)
            GameMenuManager.Instance.ActivateNoVision(1.5f, DoActionToClose);
        else if (_activatedDialog.darkAfterEnd)
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
        _choiceDialogMenuTransform.DOPivotY(3f, 0.3f);
        _mainDialogMenuTransform.DOPivotY(3f, 0.3f).OnComplete(() =>
        {
            CameraManager.Instance.CameraZoom(0, true);
            dialogMenu.gameObject.SetActive(false);
            mainDialogMenu.gameObject.SetActive(false);
            subDialogMenu.gameObject.SetActive(false);
            bigPictureMenu.gameObject.SetActive(false);
            //if (_activatedDialog.posAfterEnd)
            //Player.Instance.transform.localPosition = _activatedDialog.posAfterEnd.position;

            Player.Instance.canMove = true;
            TextManager.EndCursedText(textDialogMain);
            Player.Instance.virtualCamera.Follow = Player.Instance.transform;
            _activatedDialog = null;
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

        for (int i = 0; i < _selectedBranch.choices.Count; i++)
        {
            var obj = Instantiate(buttonChoicePrefab, new Vector3(0, 0, 0), Quaternion.identity,
                choicesContainer.transform);
            obj.transform.Find("Text").GetComponent<TextMeshProUGUI>().text =
                _selectedBranch.choices[i].textQuestion.text;
            int num = i;
            obj.GetComponent<Button>().onClick.AddListener(delegate { ActivateChoiceStep(num); });
            if (_selectedBranch.choices[i].read)
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
        StepBranch newBranch = _activatedDialog.FindBranch(nameOfBranch);
        _selectedBranch = newBranch;
        _selectedStep = _selectedBranch.dialogSteps[0];
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
            if (totalStep != _selectedBranch.dialogSteps.Count)
            {
                _selectedStep = _selectedBranch.dialogSteps[totalStep];
                DialogUpdateAction();
                return true;
            }

            // Продолжение не найдено, выдаём false
            return false;
        }

        if (_selectedStep.totalNpc.animator)
            _selectedStep.totalNpc.animator?.SetBool("talk", false);

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
        _selectedStep.UpdateStep();

        if (_activatedDialog.styleOfDialog is Dialog.DialogStyle.Main)
        {
            textNameMain.text = _selectedStep.totalNpc.nameOfNpc.text;
            iconImageMain.sprite = _selectedStep.icon;
            iconImageMain.SetNativeSize();
            _iconImageTransform.DOPunchAnchorPos(new Vector3(1, 1, 1), 5f, 3);
        }
        else if (_activatedDialog.styleOfDialog is Dialog.DialogStyle.BigPicture)
        {
            textDialogBigPicture.text = _selectedStep.totalNpc.nameOfNpc.text;
            if (_selectedStep.bigPictureName != "") // Меняем
                bigPicture.sprite = _selectedStep.GetBigPicture();
        }

        foreach (Npc totalNpc in NpcManager.Instance.allNpc)
        {
            if (!totalNpc.canMeet) continue;
            if (totalNpc.nameOfNpc != _selectedStep.totalNpc.nameOfNpc) continue;
            if (Player.Instance.familiarNpc.Contains(totalNpc)) continue;
            Player.Instance.familiarNpc.Add(totalNpc);
            break;
        }

        if (_selectedStep.totalNpc.animator)
            _selectedStep.totalNpc.animator.SetBool("talk", true);

        if (_selectedStep.cursedText)
            TextManager.SetCursedText(textDialogMain, Random.Range(5, 40));
        else
            StartCoroutine(SetText());

        if (_selectedStep.totalNpc.nameInWorld != "nothing")
            Player.Instance.virtualCamera.Follow = _selectedStep.totalNpc.animator.transform;

        _selectedStep.fastChanges.ActivateChanges();
        
        CutsceneManager.Instance.ActivateCutsceneStep(_selectedStep.activateCutsceneStep);
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
                    else if (_activatedDialog.styleOfDialog == Dialog.DialogStyle.BigPicture)
                        textDialogBigPicture.text = _selectedStep.dialogText.text;
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
        textDialogBigPicture.text = "";
        textDialogSub.text = "";
        _animatingText = true;
        char[] textChar = _selectedStep.dialogText.text.ToCharArray();
        foreach (char tChar in textChar)
        {
            if (!_animatingText) continue;
            if (_activatedDialog.styleOfDialog == Dialog.DialogStyle.Main)
                textDialogMain.text += tChar;
            else if (_activatedDialog.styleOfDialog == Dialog.DialogStyle.BigPicture)
                textDialogBigPicture.text += tChar;
            else
                textDialogSub.text += tChar;
            yield return new WaitForSeconds(0.0025f * textChar.Length);
        }

        _animatingText = false;
        if (_selectedStep.delayAfterNext != 0)
        {
            yield return new WaitForSeconds(_selectedStep.delayAfterNext);
            if (totalStep + 1 == _activatedDialog.stepBranches.Count) // Завершение
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