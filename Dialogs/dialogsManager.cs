using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class DialogsManager : MonoBehaviour
{
    [Header("Настройки")] public GameObject dialogMenu;
    private TextMeshProUGUI _textNameMain, _textDialogMain, _textDialogSub;
    [SerializeField] private GameObject mainDialogMenu, choiceDialogMenu, subDialogMenu;
    [SerializeField] private GameObject choicesContainer, buttonChoicePrefab;
    private Image _iconImageMain, _iconImageChoices;
    private Image _bigPicture;

    [Header("Текущий диалог")] public int totalStep;

    private Dialog ActivatedDialog
    {
        get
        {
            if (_activatedDialog == null || _activatedDialog.nameDialog == "")
                return null;
            else
                return _activatedDialog;
        }
        set => _activatedDialog = value;
    }

    private Dialog _activatedDialog;

    [Header("Все диалоги")] public Dialog[] dialogs = new Dialog[0];

    private bool _animatingText, _canStepNext;

    private StepBranch _selectedBranch;
    private DialogStep _selectedStep;

    private Image _noViewPanel;
    private AllScripts _scripts;

    public void Initialize()
    {
        mainDialogMenu.GetComponent<RectTransform>().DOPivotY(4f, 0.3f);
        choiceDialogMenu.GetComponent<RectTransform>().DOPivotY(4f, 0.3f);
        _scripts = GameObject.Find("scripts").GetComponent<AllScripts>();
        _noViewPanel = _scripts.main.noViewPanel;

        _textNameMain = mainDialogMenu.transform.Find("TextName").GetComponent<TextMeshProUGUI>();
        _textDialogMain = mainDialogMenu.transform.Find("TextDialog").GetComponent<TextMeshProUGUI>();
        _iconImageMain = mainDialogMenu.transform.Find("Image").GetComponent<Image>();
        _iconImageChoices = choiceDialogMenu.transform.Find("Image").GetComponent<Image>();
        _textDialogSub = subDialogMenu.transform.Find("Text").GetComponent<TextMeshProUGUI>();
        _bigPicture = dialogMenu.transform.Find("bigPicture").gameObject.GetComponent<Image>();
    }

    /// <summary>
    /// Поиск диалога
    /// </summary>
    /// <param name="nameDialog">Имя диалога, который ищем</param>
    /// <returns>Возвращает диалог</returns>
    public Dialog GetDialog(string nameDialog)
    {
        foreach (Dialog totalDialog in dialogs)
        {
            if (totalDialog.nameDialog == nameDialog && !totalDialog.readed)
                return totalDialog;
        }

        Debug.Log("Dialog " + nameDialog + "don`t find or readed");
        return null;
    }

    /// <summary>
    ///  Активирует новый диалог
    /// </summary>
    /// <param name="nameDialog">Название диалога</param>
    public void ActivateDialog(string nameDialog) // Старт диалога
    {
        ActivatedDialog = null; // Если уже назначен

        Dialog newDialog = GetDialog((nameDialog));
        if (newDialog == null) return;

        ActivatedDialog = newDialog;
        _selectedBranch = ActivatedDialog.stepBranches[0];
        _selectedStep = _selectedBranch.dialogSteps[0];
        dialogMenu.gameObject.SetActive(true);
        _canStepNext = false;
        _scripts.interactions.lockInter = ActivatedDialog.canInter;
        if (_selectedStep.dialogMode == DialogStep.DialogMode.Dialog)
        {
            if (ActivatedDialog.styleOfDialog == Dialog.DialogStyle.Main)
                mainDialogMenu.gameObject.SetActive(true);
            else
                subDialogMenu.gameObject.SetActive(true);
        }
        else
            choiceDialogMenu.gameObject.SetActive(true);

        _scripts.main.EndCursedText(_textDialogMain);

        if (ActivatedDialog.bigPicture)
        {
            Sequence sequence = DOTween.Sequence();
            Tween stepSequence = _noViewPanel.DOFade(100f, 0.5f).SetEase(Ease.InQuart);
            stepSequence.OnComplete(() => { _bigPicture.sprite = ActivatedDialog.bigPicture; });
            sequence.Append(stepSequence);
            sequence.Append(_bigPicture.DOFade(100f, 0.001f).SetEase(Ease.InQuart));
            sequence.Append(_noViewPanel.DOFade(0f, 0.5f).SetEase(Ease.OutQuart));
            sequence.Insert(0, transform.DOScale(new Vector3(1, 1, 1), sequence.Duration()));
        }
        else
            _bigPicture.sprite = _scripts.main.nullSprite;

        if (ActivatedDialog.mainPanelStartDelay == 0)
            OpenPanels();
        else
            StartCoroutine(ActivateUIMainMenuWithDelay(
                ActivatedDialog.mainPanelStartDelay));

        if (!ActivatedDialog.moreRead)
            ActivatedDialog.readed = true;
        _scripts.player.canMove = ActivatedDialog.canMove;

        if (_selectedStep.dialogMode == DialogStep.DialogMode.Dialog)
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
        if (ActivatedDialog.activatedCutsceneStepAtEnd != -1)
            _scripts.cutsceneManager.ActivateCutsceneStep(ActivatedDialog.activatedCutsceneStepAtEnd);

        totalStep = 0;
        Sequence sequence = DOTween.Sequence();
        if (ActivatedDialog.bigPicture != null && !ActivatedDialog.disableFadeAtEnd)
        {
            sequence = sequence.Append(_noViewPanel.DOFade(100f, 0.5f).SetEase(Ease.InQuart));
            sequence.Append(mainDialogMenu.GetComponent<RectTransform>().DOPivotY(4f, 0.3f));
            sequence.Append(_bigPicture.DOFade(0f, 0.1f).SetEase(Ease.OutQuart));
            sequence.Append(_noViewPanel.DOFade(0f, 0.5f).SetEase(Ease.OutQuart));
        }
        else
            sequence.Append(mainDialogMenu.GetComponent<RectTransform>().DOPivotY(4f, 0.3f));

        choiceDialogMenu.GetComponent<RectTransform>().DOPivotY(4f, 0.5f);

        if (ActivatedDialog.darkAfterEnd && !ActivatedDialog.disableFadeAtEnd)
        {
            Tween extraSequence = _noViewPanel.DOFade(100f, 0.7f).SetEase(Ease.InQuart);
            if (ActivatedDialog.posAfterEnd != null)
            {
                extraSequence.OnComplete(() =>
                {
                    _scripts.player.transform.localPosition = ActivatedDialog.posAfterEnd.position;
                });
            }

            sequence.Append(extraSequence);
            sequence.Append(_noViewPanel.DOFade(0f, 0.7f).SetEase(Ease.OutQuart));
        }

        sequence.Insert(0, transform.DOScale(new Vector3(1, 1, 1), sequence.Duration()));
        sequence.OnComplete(() =>
        {
            dialogMenu.gameObject.SetActive(false);
            mainDialogMenu.gameObject.SetActive(false);
            subDialogMenu.gameObject.SetActive(false);
            if (ActivatedDialog.nextStepQuest)
                _scripts.questsSystem.NextStep();
            _scripts.player.canMove = true;
            _scripts.main.EndCursedText(_textDialogMain);
            _scripts.player.virtualCamera.Follow = _scripts.player.transform;
            if (ActivatedDialog.noteAdd != "")
                _scripts.notebook.AddNote(ActivatedDialog.noteAdd);
            string newDialog = ActivatedDialog.startNewDialogAfterEnd;
            ActivatedDialog = null;
            _canStepNext = true;
            if (newDialog != "")
                ActivateDialog(newDialog);
        });
    }

    /// <summary>
    /// Активация + генерация меню с выборами
    /// </summary>
    /// <param name="setScale"></param>
    private void ActivateChoiceMenu(bool setScale = false)
    {
        choiceDialogMenu.gameObject.SetActive(true);
        choiceDialogMenu.transform.Find("Scroll View").GetComponent<AdaptiveScrollView>()?.UpdateContentSize();
        if (setScale)
            choiceDialogMenu.GetComponent<RectTransform>().localScale = new Vector2(1f, 1f);
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

        choiceDialogMenu.transform.Find("Scroll View").GetComponent<AdaptiveScrollView>().UpdateContentSize();
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
        if (_selectedStep.dialogMode == DialogStep.DialogMode.Dialog)
        {
            if (ActivatedDialog.styleOfDialog == Dialog.DialogStyle.Main)
                mainDialogMenu.gameObject.SetActive(true);
            else
                subDialogMenu.gameObject.SetActive(true);
        }
        else
            choiceDialogMenu.gameObject.SetActive(true);

        if (_selectedStep.dialogMode == DialogStep.DialogMode.Dialog)
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
            if (_selectedStep.dialogMode == DialogStep.DialogMode.Dialog)
                DialogUpdateAction();
            else
                ActivateChoiceMenu();
        }

        if (GameObject.Find(_selectedStep.totalNpc.nameInWorld) && _selectedStep.animateTalking)
            GameObject.Find(_selectedStep.totalNpc.nameInWorld).GetComponent<Animator>().SetBool("talk", false);

        // Окончание обычного диалога
        if (totalStep + 1 == _selectedBranch.dialogSteps.Length)
        {
            // Если выбора нет.
            if (_selectedBranch.choices.Length == 0)
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
        if (ActivatedDialog.styleOfDialog == Dialog.DialogStyle.Main)
        {
            _textNameMain.text = _selectedStep.totalNpc.nameOfNpc;
            _iconImageMain.sprite = _selectedStep.icon;
            _iconImageMain.SetNativeSize();
            if (_selectedStep.shakeIcon)
                _iconImageMain.GetComponent<RectTransform>().DOPunchAnchorPos(new Vector3(10, 10, 10), 2f);
            else
                _iconImageMain.GetComponent<RectTransform>().DOPunchAnchorPos(new Vector3(1, 1, 1), 5f, 3);
        }

        foreach (NPC totalNpc in _scripts.main.allNpc)
        {
            if (totalNpc.nameOfNpc != _selectedStep.totalNpc.nameOfNpc) continue;
            if (_scripts.player.familiarNpc.Contains(totalNpc)) continue;
            _scripts.player.familiarNpc.Add(totalNpc);
            break;
        }

        if (_selectedStep.setCloseMeet)
            _selectedStep.totalNpc.playerCloseMeet = true;

        if (GameObject.Find(_selectedStep.totalNpc.nameInWorld) && _selectedStep.animateTalking)
            GameObject.Find(_selectedStep.totalNpc.nameInWorld).GetComponent<Animator>().SetBool("talk", true);

        if (_selectedStep.cursedText)
            _scripts.main.SetCursedText(_textDialogMain, Random.Range(5, 40));
        else
            StartCoroutine(SetText(_selectedStep.text));

        _scripts.player.virtualCamera.Follow = _selectedStep.cameraTarget != null
            ? _selectedStep.cameraTarget
            : _scripts.player.transform;

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
        mainDialogMenu.GetComponent<RectTransform>().GetComponent<RectTransform>().DOPivotY(0.5f, 0.3f)
            .SetEase(Ease.InQuart).OnComplete(() => { _canStepNext = true; });
        choiceDialogMenu.GetComponent<RectTransform>().DOPivotY(0.5f, 0.3f).SetEase(Ease.InQuart).OnComplete(() =>
        {
            _canStepNext = true;
        });
    }

    private void Update()
    {
        if (ActivatedDialog == null) return;
        if (_selectedStep.delayAfterNext == 0)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (!_canStepNext) return;

                if (_animatingText)
                {
                    _animatingText = false;
                    StopAllCoroutines();
                    if (ActivatedDialog.styleOfDialog == Dialog.DialogStyle.Main)
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
            if (ActivatedDialog.styleOfDialog == Dialog.DialogStyle.Main)
                _textDialogMain.text += tChar;
            else
                _textDialogSub.text += tChar;
            yield return new WaitForSeconds(0.05f);
        }

        _animatingText = false;
        if (_selectedStep.delayAfterNext != 0)
        {
            yield return new WaitForSeconds(_selectedStep.delayAfterNext);
            if (_selectedStep.dialogMode == DialogStep.DialogMode.Dialog &&
                (totalStep + 1) == ActivatedDialog.stepBranches.Length)
                DialogCLose();
            else
                DialogMoveNext();
        }
    }

    private IEnumerator ActivateUIMainMenuWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        OpenPanels();
    }
}