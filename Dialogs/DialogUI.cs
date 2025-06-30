using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using LastFramework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogUI : DisplayBase, IMenuable
{
    public static DialogUI Instance { get; private set; }
    private DialogsManager _dialogsManager;

    [Header("GameObjects")] [SerializeField]
    private GameObject dialogMenu;

    public GameObject menu => dialogMenu;

    [SerializeField] private GameObject choicesContainer;

    [SerializeField] private TextMeshProUGUI textNameMain, textDialogMain, textDialogSub, textDialogBigPicture;
    private TextMeshProUGUI _selectedTextDialog;
    [SerializeField] private GameObject mainDialogMenu, choiceDialogMenu, subDialogMenu, bigPictureMenu;
    [SerializeField] private Image firstTalkerIcon, secondTalkerIcon, thirdTalkerIcon, iconImageChoice;
    private Dictionary<Npc, Image> _npcAndTalkerIcon = new Dictionary<Npc, Image>();
    [SerializeField] private Image bigPicture;
    [SerializeField] private AdaptiveScrollView adaptiveScrollViewChoice;
    private RectTransform _mainDialogMenuTransform, _choiceDialogMenuTransform;

    [Header("Prefabs")] [SerializeField] private GameObject buttonChoicePrefab;
    public FastChangesController[] fastChangesInDialogs = new FastChangesController[0];

    [Header("Preferences")] [SerializeField]
    private Color dontSelectedColor;

    private Coroutine _textGenerate;

    private void Awake() => Instance = this;

    public async Task Initialize(DialogsManager dialogsManager)
    {
        _mainDialogMenuTransform = mainDialogMenu.GetComponent<RectTransform>();
        _choiceDialogMenuTransform = choiceDialogMenu.GetComponent<RectTransform>();
        _mainDialogMenuTransform.DOPivotY(3f, 0.01f);
        _choiceDialogMenuTransform.DOPivotY(3f, 0.01f);

        _dialogsManager = dialogsManager;
        _dialogsManager.DialogUI = this;
    }

    public void ManagePlayerMenu()
    {
    }

    /// <summary>
    /// Активация диалоговых меню
    /// </summary>
    public async void ActivateDialogWindow()
    {
        dialogMenu.gameObject.SetActive(true);
        StartCoroutine(ActivateUIMainMenuWithDelay(currentDialogStepNode.mainPanelStartDelay));
        TextManager.EndCursedText(textDialogMain);
        if (!_dialogsManager.CanChoice()) // Потом уже управление менюшками
        {
            switch (currentDialogStepNode.styleOfDialog)
            {
                case Enums.DialogStyle.Main:
                    mainDialogMenu.gameObject.SetActive(true);
                    _selectedTextDialog = textDialogMain;
                    break;
                case Enums.DialogStyle.BigPicture:
                    _selectedTextDialog = textDialogBigPicture;
                    GameMenuManager.Instance.NoVisionForTime(1.2f,
                        new Task(() => { bigPictureMenu.gameObject.SetActive(true); }));
                    break;
                case Enums.DialogStyle.SubMain:
                    _selectedTextDialog = textDialogSub;
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
        _mainDialogMenuTransform.DOPivotY(0.5f, 0.3f).SetEase(Ease.InQuart);
        _choiceDialogMenuTransform.DOPivotY(0.5f, 0.3f).SetEase(Ease.InQuart);
    }

    // ReSharper disable Unity.PerformanceAnalysis
    /// <summary>
    /// Активация + генерация меню с выборами
    /// </summary>
    /// <param name="setScale"></param>
    public void ActivateChoiceMenu(bool setScale = false)
    {
        choiceDialogMenu.gameObject.SetActive(true);
        iconImageChoice.sprite = Player.Instance.npcEntity.GetStyleIcon(NpcIcon.IconMood.Standard);
        adaptiveScrollViewChoice.UpdateContentSize();
        Player.Instance.virtualCamera.Follow = Player.Instance.transform;
        if (setScale)
            _choiceDialogMenuTransform.localScale = new Vector2(1f, 1f);
        foreach (Transform child in choicesContainer.transform)
            Destroy(child.gameObject);

        for (int i = 0; i < currentDialogStepNode.choices; i++)
        {
            var obj = Instantiate(buttonChoicePrefab, new Vector3(0, 0, 0), Quaternion.identity,
                choicesContainer.transform);
            /*
            obj.transform.Find("Text").GetComponent<TextMeshProUGUI>().text =
                _dialogsManager.SelectedBranch.choices[i].textQuestion.text;
            int num = i;
            obj.GetComponent<Button>().onClick.AddListener(delegate { ActivateChoiceStep(num); });
            if (_dialogsManager.SelectedBranch.choices[i].read)
                obj.GetComponent<Button>().interactable = false;
                */
        }

        adaptiveScrollViewChoice.UpdateContentSize();
    }

    /// <summary>
    /// Активация выбора
    /// </summary>
    /// <param name="id"></param>
    private void ActivateChoiceStep(int id)
    {
        /*
        DialogChoice choice = _dialogsManager.SelectedBranch.choices[id];
        if (choice.nameNewBranch == "") // Быстрое закрытие диалога
            CLoseDialogWindow();
        else
        {
            choiceDialogMenu.gameObject.SetActive(false);
            _dialogsManager.TotalStep = 0;
            if (!choice.moreRead)
                choice.read = true;
            _dialogsManager.ChangeDialogBranch(choice.nameNewBranch);
        }
        */
    }

    public void UpdateDialogWindow(Npc npc)
    {
        switch (currentDialogStepNode.styleOfDialog)
        {
            case Enums.DialogStyle.Main:
                textNameMain.text = npc.nameOfNpc.text;
                SetIcon(npc);
                break;
            case Enums.DialogStyle.BigPicture:
            {
                if (currentDialogStepNode.bigPicture) // Меняем
                    bigPicture.sprite = currentDialogStepNode.bigPicture;
                break;
            }
        }

        if (currentDialogStepNode.cursedText)
            TextManager.SetCursedText(_selectedTextDialog, Random.Range(5, 40));
        else
            _textGenerate = StartCoroutine(SetText());
    }

    public void UpdateTalkersDict(Npc npc)
    {
        _npcAndTalkerIcon.Add(npc, _npcAndTalkerIcon.Count switch
        {
            0 => firstTalkerIcon,
            1 => secondTalkerIcon,
            2 => thirdTalkerIcon
        });
    }

    private void SetIcon(Npc npc)
    {
        if (!currentDialogStepNode.character) return;

        foreach (KeyValuePair<Npc, Image> talker in _npcAndTalkerIcon)
            talker.Value.color = dontSelectedColor;

        foreach (KeyValuePair<Npc, Image> talker in _npcAndTalkerIcon)
        {
            if (npc != talker.Key) continue;
            talker.Value.sprite = npc.GetStyleIcon((NpcIcon.IconMood)currentDialogStepNode.mood);
            talker.Value.rectTransform.DOPunchAnchorPos(new Vector3(1, 1, 1), 5f, 3);
            talker.Value.color = Color.white;
        }
    }

    /// <summary>
    /// Закрытие окна диалога
    /// </summary>
    public async void CLoseDialogWindow()
    {
        Interactions.Instance.lockInter = false;

        if (currentDialogStepNode.styleOfDialog == Enums.DialogStyle.BigPicture)
            GameMenuManager.Instance.NoVisionForTime(1.5f, DoActionsToClose());
        else if (currentDialogStepNode.darkAfterEnd)
            GameMenuManager.Instance.NoVisionForTime(1.2f, DoActionsToClose());
        else
            await DoActionsToClose();
    }

    private async Task DoActionsToClose()
    {
        _choiceDialogMenuTransform.DOPivotY(3f, 0.3f);
        _mainDialogMenuTransform.DOPivotY(3f, 0.3f).OnComplete(async () =>
        {
            await CameraManager.Instance.CameraZoom(0, true);
            TextManager.EndCursedText(textDialogMain);
            dialogMenu.gameObject.SetActive(false);
            mainDialogMenu.gameObject.SetActive(false);
            subDialogMenu.gameObject.SetActive(false);
            bigPictureMenu.gameObject.SetActive(false);
            foreach (KeyValuePair<Npc, Image> talker in _npcAndTalkerIcon)
                talker.Value.sprite = GameMenuManager.Instance.NullSprite;

            //if (_activatedDialog.posAfterEnd)
            //Player.Instance.transform.localPosition = _activatedDialog.posAfterEnd.position;

            _dialogsManager.DoActionsToClose();
        });
        story = null;
        currentDialogStepNode = null;
        lastNode = false;
        _npcAndTalkerIcon = new Dictionary<Npc, Image>();
    }

    private void Update()
    {
        if (!story) return;
        if (currentDialogStepNode.delayAfterNext == 0)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (_textGenerate != null)
                {
                    StopCoroutine(_textGenerate);
                    _textGenerate = null;
                    _selectedTextDialog.text = currentDialogStepNode.dialogTextRu;
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
        _selectedTextDialog.text = "";
        LanguageSetting totalLanguage = new LanguageSetting(currentDialogStepNode.dialogTextRu, currentDialogStepNode.dialogTextEn);
        char[] textChar = totalLanguage.text.ToCharArray();
        foreach (char tChar in textChar)
        {
            _selectedTextDialog.text += tChar;
            yield return null;
        }

        if (currentDialogStepNode.delayAfterNext != 0)
        {
            yield return new WaitForSeconds(currentDialogStepNode.delayAfterNext);
            _dialogsManager.DialogMoveNext();
        }

        _textGenerate = null;
    }
}