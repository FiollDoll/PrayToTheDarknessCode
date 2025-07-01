using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
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

    public Task Initialize(DialogsManager dialogsManager)
    {
        _mainDialogMenuTransform = mainDialogMenu.GetComponent<RectTransform>();
        _choiceDialogMenuTransform = choiceDialogMenu.GetComponent<RectTransform>();
        _mainDialogMenuTransform.DOPivotY(3f, 0.01f);
        _choiceDialogMenuTransform.DOPivotY(3f, 0.01f);

        _dialogsManager = dialogsManager;
        _dialogsManager.DialogUI = this;
        return Task.CompletedTask;
    }

    public void ManagePlayerMenu()
    {
    }

    public void OnNextDialogStep(InputAction.CallbackContext context)
    {
        if (context.action.WasPerformedThisFrame())
        {
            if (!story) return;
            if (_textGenerate != null)
            {
                StopCoroutine(_textGenerate);
                _textGenerate = null;
                _selectedTextDialog.text =
                    new LanguageSetting(currentDialogStepNode.dialogTextRu, currentDialogStepNode.dialogTextEn).Text;
            }
            else
            {
                if (!_dialogsManager.CanChoice())
                    _dialogsManager.DialogMoveNext();
            }
        }
    }

    /// <summary>
    /// Активация диалоговых меню
    /// </summary>
    public async void ActivateDialogWindow()
    {
        dialogMenu.gameObject.SetActive(true);
        await ActivateUIMainMenuWithDelay(currentDialogStepNode.mainPanelStartDelay);
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

    /// <summary>
    /// Активация + генерация меню с выборами
    /// </summary>
    public async void ActivateChoiceMenu()
    {
        mainDialogMenu.SetActive(false);
        choiceDialogMenu.SetActive(true);
        iconImageChoice.sprite = Player.Instance.npcEntity.GetStyleIcon(NpcIcon.IconMood.Standard);

        foreach (Transform child in choicesContainer.transform)
            Destroy(child.gameObject);

        for (int i = 0; i < currentDialogStepNode.choices; i++)
        {
            var obj = Instantiate(buttonChoicePrefab, Vector3.zero, new Quaternion(),
                choicesContainer.transform);

            obj.transform.Find("Text").GetComponent<TextMeshProUGUI>().text =
                currentDialogStepNode.choiceOptions[i];
            int num = i;
            obj.GetComponent<Button>().onClick.AddListener(() => { ActivateChoiceStep(num); });
            obj.transform.localPosition = Vector3.zero; // Хз почему, но по Z слетает
        }

        await adaptiveScrollViewChoice.UpdateContentSize();
    }

    /// <summary>
    /// Выбор шага диалога
    /// </summary>
    private void ActivateChoiceStep(int id)
    {
        choiceDialogMenu.gameObject.SetActive(false);
        mainDialogMenu.SetActive(true);
        _dialogsManager.DialogMoveNext(id);
    }

    public void UpdateDialogWindow(Npc npc)
    {
        switch (currentDialogStepNode.styleOfDialog)
        {
            case Enums.DialogStyle.Main:
                textNameMain.text = npc.nameOfNpc.Text;
                SetIcon(npc);
                break;
            case Enums.DialogStyle.BigPicture:
            {
                if (currentDialogStepNode.bigPicture) // Меняем
                    bigPicture.sprite = currentDialogStepNode.bigPicture;
                break;
            }
        }

        if (!_dialogsManager.CanChoice())
            _textGenerate = StartCoroutine(SetText());
        else
            ActivateChoiceMenu();
    }

    public void UpdateTalkersDict(Npc npc)
    {
        _npcAndTalkerIcon.Add(npc, _npcAndTalkerIcon.Count switch
        {
            0 => firstTalkerIcon,
            1 => secondTalkerIcon,
            2 => thirdTalkerIcon,
            _ => throw new ArgumentOutOfRangeException()
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

    private Task DoActionsToClose()
    {
        _choiceDialogMenuTransform.DOPivotY(3f, 0.3f);
        _mainDialogMenuTransform.DOPivotY(3f, 0.3f).OnComplete(async () =>
        {
            await CameraManager.Instance.CameraZoom(0, true);
            dialogMenu.gameObject.SetActive(false);
            mainDialogMenu.gameObject.SetActive(false);
            subDialogMenu.gameObject.SetActive(false);
            bigPictureMenu.gameObject.SetActive(false);
            foreach (KeyValuePair<Npc, Image> talker in _npcAndTalkerIcon)
                talker.Value.sprite = GameMenuManager.Instance.NullSprite;

            _dialogsManager.DoActionsToClose();
        });
        story = null;
        currentDialogStepNode = null;
        lastNode = false;
        _npcAndTalkerIcon = new Dictionary<Npc, Image>();
        return Task.CompletedTask;
    }

    private async Task ActivateUIMainMenuWithDelay(float delay)
    {
        await Task.Delay(Mathf.RoundToInt(delay * 1000));
        OpenPanels();
    }

    /// <summary>
    ///  Посимвольная установка текста
    /// </summary>
    /// <returns></returns>
    private IEnumerator SetText()
    {
        _selectedTextDialog.text = "";
        string totalText =
            new LanguageSetting(currentDialogStepNode.dialogTextRu, currentDialogStepNode.dialogTextEn).Text;
        char[] textChar = totalText.ToCharArray();
        foreach (char tChar in textChar)
        {
            _selectedTextDialog.text += tChar;
            yield return new WaitForSeconds(0.0001f);
        }

        _selectedTextDialog.text = totalText;
        if (currentDialogStepNode.delayAfterNext != 0)
        {
            yield return new WaitForSeconds(currentDialogStepNode.delayAfterNext);
            _dialogsManager.DialogMoveNext();
        }

        _textGenerate = null;
    }
}