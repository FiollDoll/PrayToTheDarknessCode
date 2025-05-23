using System.Collections;
using DG.Tweening;
using LastFramework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogUI : MonoBehaviour, IMenuable
{
    public static DialogUI Instance { get; private set; }
    private DialogsManager _dialogsManager;
    [Header("Prefabs")] [SerializeField] private GameObject buttonChoicePrefab;
    [SerializeField] private Npc nothingNpc; // Костыль. Вырезать

    [Header("Preferences")] [SerializeField]
    private Color dontSelectedColor;

    [SerializeField] private GameObject dialogMenu;

    public GameObject menu => dialogMenu;

    [SerializeField] private GameObject choicesContainer;

    [SerializeField] private TextMeshProUGUI textNameMain, textDialogMain, textDialogSub, textDialogBigPicture;
    [SerializeField] private GameObject mainDialogMenu, choiceDialogMenu, subDialogMenu, bigPictureMenu;
    [SerializeField] private Image firstTalkerIcon, secondTalkerIcon, thirdTalkerIcon, iconImageChoice;
    [SerializeField] private Image bigPicture;
    [SerializeField] private AdaptiveScrollView adaptiveScrollViewChoice;
    private RectTransform _mainDialogMenuTransform, _choiceDialogMenuTransform;

    private bool _animatingText;

    private void Awake() => Instance = this;

    public void Initialize(DialogsManager dialogsManager)
    {
        _mainDialogMenuTransform = mainDialogMenu.GetComponent<RectTransform>();
        _choiceDialogMenuTransform = choiceDialogMenu.GetComponent<RectTransform>();
        _mainDialogMenuTransform.DOPivotY(3f, 0.01f);
        _choiceDialogMenuTransform.DOPivotY(3f, 0.01f);

        _dialogsManager = dialogsManager;
        _dialogsManager.DialogUI = this;
    }

    public void ManageActivationMenu()
    {
    }

    /// <summary>
    /// Активация диалоговых меню
    /// </summary>
    public void ActivateDialogWindow()
    {
        dialogMenu.gameObject.SetActive(true);
        StartCoroutine(ActivateUIMainMenuWithDelay(_dialogsManager.ActivatedDialog.mainPanelStartDelay));
        TextManager.EndCursedText(textDialogMain);
        if (!_dialogsManager.CanChoice()) // Потом уже управление менюшками
        {
            switch (_dialogsManager.ActivatedDialog.styleOfDialog)
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
    /// Включение анимаций появления менюшек
    /// </summary>
    private void OpenPanels()
    {
        // TODO: переделать
        _mainDialogMenuTransform.DOPivotY(0.5f, 0.3f).SetEase(Ease.InQuart)
            .OnComplete(() =>
            {
                /*_canStepNext = true;*/
            });
        _choiceDialogMenuTransform.DOPivotY(0.5f, 0.3f).SetEase(Ease.InQuart)
            .OnComplete(() =>
            {
                /*_canStepNext = true;*/
            });
    }

    // ReSharper disable Unity.PerformanceAnalysis
    /// <summary>
    /// Активация + генерация меню с выборами
    /// </summary>
    /// <param name="setScale"></param>
    public void ActivateChoiceMenu(bool setScale = false)
    {
        choiceDialogMenu.gameObject.SetActive(true);
        iconImageChoice.sprite = Player.Instance.npcEntity.GetStyleIcon(NpcIcon.IconMood.Standart);
        adaptiveScrollViewChoice.UpdateContentSize();
        Player.Instance.virtualCamera.Follow = Player.Instance.transform;
        if (setScale)
            _choiceDialogMenuTransform.localScale = new Vector2(1f, 1f);
        foreach (Transform child in choicesContainer.transform)
            Destroy(child.gameObject);

        for (int i = 0; i < _dialogsManager.SelectedBranch.choices.Count; i++)
        {
            var obj = Instantiate(buttonChoicePrefab, new Vector3(0, 0, 0), Quaternion.identity,
                choicesContainer.transform);
            obj.transform.Find("Text").GetComponent<TextMeshProUGUI>().text =
                _dialogsManager.SelectedBranch.choices[i].textQuestion.text;
            int num = i;
            obj.GetComponent<Button>().onClick.AddListener(delegate { ActivateChoiceStep(num); });
            if (_dialogsManager.SelectedBranch.choices[i].read)
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
        DialogChoice choice = _dialogsManager.SelectedBranch.choices[id];
        if (choice.nameNewBranch == "") // Быстрое закрытие диалога
            DialogCLose();
        else
        {
            choiceDialogMenu.gameObject.SetActive(false);
            _dialogsManager.TotalStep = 0;
            if (!choice.moreRead)
                choice.read = true;
            _dialogsManager.ChangeDialogBranch(choice.nameNewBranch);
        }
    }

    public void UpdateUI()
    {
        if (_dialogsManager.ActivatedDialog.styleOfDialog is Dialog.DialogStyle.Main)
        {
            textNameMain.text = _dialogsManager.SelectedStep.totalNpc.nameOfNpc.text;
            SetIcon();
        }
        else if (_dialogsManager.ActivatedDialog.styleOfDialog is Dialog.DialogStyle.BigPicture)
        {
            textDialogBigPicture.text = _dialogsManager.SelectedStep.totalNpc.nameOfNpc.text;
            if (_dialogsManager.SelectedStep.bigPictureName != "") // Меняем
                bigPicture.sprite = _dialogsManager.SelectedStep.GetBigPicture();
        }

        if (_dialogsManager.SelectedStep.cursedText)
            TextManager.SetCursedText(textDialogMain, Random.Range(5, 40));
        else
            StartCoroutine(SetText());
    }

    private void SetIcon()
    {
        if (_dialogsManager.SelectedStep.totalNpc.nameInWorld == "nothing") return;

        firstTalkerIcon.color = dontSelectedColor;
        secondTalkerIcon.color = dontSelectedColor;
        thirdTalkerIcon.color = dontSelectedColor;

        switch (_dialogsManager.NpcInTotalDialog.Count)
        {
            case > 0 when _dialogsManager.SelectedStep.totalNpc == _dialogsManager.NpcInTotalDialog[0]:
                firstTalkerIcon.sprite = _dialogsManager.SelectedStep.icon;
                firstTalkerIcon.GetComponent<RectTransform>().DOPunchAnchorPos(new Vector3(1, 1, 1), 5f, 3);
                firstTalkerIcon.color = Color.white;
                break;
            case > 1 when _dialogsManager.SelectedStep.totalNpc == _dialogsManager.NpcInTotalDialog[1]:
                secondTalkerIcon.sprite = _dialogsManager.SelectedStep.icon;
                secondTalkerIcon.GetComponent<RectTransform>().DOPunchAnchorPos(new Vector3(1, 1, 1), 5f, 3);
                secondTalkerIcon.color = Color.white;
                break;
            case > 2 when _dialogsManager.SelectedStep.totalNpc == _dialogsManager.NpcInTotalDialog[2]:
                thirdTalkerIcon.sprite = _dialogsManager.SelectedStep.icon;
                thirdTalkerIcon.GetComponent<RectTransform>().DOPunchAnchorPos(new Vector3(1, 1, 1), 5f, 3);
                thirdTalkerIcon.color = Color.white;
                break;
        }
    }

    /// <summary>
    /// Закрытие диалога
    /// </summary>
    public void DialogCLose()
    {
        Interactions.Instance.lockInter = false;

        if (_dialogsManager.ActivatedDialog.styleOfDialog == Dialog.DialogStyle.BigPicture)
            GameMenuManager.Instance.ActivateNoVision(1.5f, DoActionToClose);
        else if (_dialogsManager.ActivatedDialog.darkAfterEnd)
            GameMenuManager.Instance.ActivateNoVision(1.2f, DoActionToClose);
        else
            DoActionToClose();
    }

    private void DoActionToClose()
    {
        StopAllCoroutines();
        _choiceDialogMenuTransform.DOPivotY(3f, 0.3f);
        _mainDialogMenuTransform.DOPivotY(3f, 0.3f).OnComplete(() =>
        {
            CameraManager.Instance.CameraZoom(0, true);
            TextManager.EndCursedText(textDialogMain);
            dialogMenu.gameObject.SetActive(false);
            mainDialogMenu.gameObject.SetActive(false);
            subDialogMenu.gameObject.SetActive(false);
            bigPictureMenu.gameObject.SetActive(false);
            firstTalkerIcon.sprite = nothingNpc.GetStyleIcon(NpcIcon.IconMood.Standart);
            secondTalkerIcon.sprite = nothingNpc.GetStyleIcon(NpcIcon.IconMood.Standart);
            thirdTalkerIcon.sprite = nothingNpc.GetStyleIcon(NpcIcon.IconMood.Standart);

            //if (_activatedDialog.posAfterEnd)
            //Player.Instance.transform.localPosition = _activatedDialog.posAfterEnd.position;

            _dialogsManager.DoActionToClose();
        });
    }

    private void Update()
    {
        if (_dialogsManager.ActivatedDialog == null) return;
        if (_dialogsManager.SelectedStep.delayAfterNext == 0)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (_animatingText)
                {
                    _animatingText = false;
                    StopAllCoroutines();
                    if (_dialogsManager.ActivatedDialog.styleOfDialog == Dialog.DialogStyle.Main)
                        textDialogMain.text = _dialogsManager.SelectedStep.dialogText.text;
                    else if (_dialogsManager.ActivatedDialog.styleOfDialog == Dialog.DialogStyle.BigPicture)
                        textDialogBigPicture.text = _dialogsManager.SelectedStep.dialogText.text;
                    else
                        textDialogSub.text = _dialogsManager.SelectedStep.dialogText.text;
                }
                else
                    _dialogsManager.DialogMoveNext();
            }
        }
    }
    
    private IEnumerator ActivateUIMainMenuWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        OpenPanels();
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
        char[] textChar = _dialogsManager.SelectedStep.dialogText.text.ToCharArray();
        foreach (char tChar in textChar)
        {
            if (!_animatingText && _dialogsManager.ActivatedDialog != null) continue;
            if (_dialogsManager.ActivatedDialog?.styleOfDialog == Dialog.DialogStyle.Main)
                textDialogMain.text += tChar;
            else if (_dialogsManager.ActivatedDialog?.styleOfDialog == Dialog.DialogStyle.BigPicture)
                textDialogBigPicture.text += tChar;
            else
                textDialogSub.text += tChar;
            yield return new WaitForSeconds(1f / textChar.Length);
        }

        _animatingText = false;
        if (_dialogsManager.SelectedStep.delayAfterNext != 0)
        {
            yield return new WaitForSeconds(_dialogsManager.SelectedStep.delayAfterNext);
            if (_dialogsManager.TotalStep + 1 == _dialogsManager.ActivatedDialog?.stepBranches.Count) // Завершение
                DialogCLose();
            else
            {
                if (!_dialogsManager.CanChoice())
                    _dialogsManager.DialogUpdateAction();
                else
                    ActivateChoiceMenu();
            }
        }
    }
}