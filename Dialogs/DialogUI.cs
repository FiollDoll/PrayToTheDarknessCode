using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DialogUI : DisplayBase, IMenuable
{
    public static DialogUI Instance { get; private set; }
    private DialogsManager _dialogsManager;

    [Header("GameObjects")]
    [SerializeField]
    private GameObject dialogMenu;

    public GameObject menu => dialogMenu;

    [SerializeField] private GameObject choicesContainer;

    [SerializeField] private TextMeshProUGUI textDialogMain, textDialogSub, textDialogBigPicture;
    private TextMeshProUGUI _selectedTextDialog;
    [SerializeField] private GameObject mainMenu, choiceMenu, subDialogMenu, bigPictureMenu;
    [SerializeField] private Image[] talkerIcons = new Image[3];
    private Dictionary<Npc, Image> _npcAndTalkerIcon = new Dictionary<Npc, Image>();
    [SerializeField] private Image bigPicture;
    [SerializeField] private AdaptiveScrollView adaptiveScrollViewChoice;

    [Header("Prefabs")][SerializeField] private GameObject buttonChoicePrefab;

    [Header("Preferences")]
    [SerializeField]
    private Color dontSelectedColor;

    private Coroutine _textGenerate;

    private void Awake() => Instance = this;

    public async Task Initialize()
    {
        _dialogsManager = new DialogsManager();
        await _dialogsManager.Initialize();
        _dialogsManager.DialogUI = this;
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
    public async Task ActivateDialogWindow()
    {
        dialogMenu.SetActive(true);
        dialogMenu.GetComponent<RectTransform>().localScale = Vector3.zero;

        await Task.Delay(Mathf.RoundToInt(currentDialogStepNode.mainPanelStartDelay * 1000));

        for (float i = 0; i < 1.1f; i += 0.1f) // Плавно поднимаем
        {
            dialogMenu.GetComponent<RectTransform>().localScale = new Vector3(i, i, 1);
            await Task.Delay(10);
        }

        if (!_dialogsManager.CanChoice()) // Потом уже управление менюшками
        {
            switch (currentDialogStepNode.styleOfDialog)
            {
                case Enums.DialogStyle.Main:
                    await CameraManager.Instance.CameraZoom(-5f, true);
                    mainMenu.SetActive(true);
                    _selectedTextDialog = textDialogMain;
                    break;
                case Enums.DialogStyle.BigPicture:
                    _selectedTextDialog = textDialogBigPicture;
                    GameMenuManager.Instance.NoVisionForTime(1.2f, BigPictureActivate());
                    break;
                case Enums.DialogStyle.SubMain:
                    _selectedTextDialog = textDialogSub;
                    subDialogMenu.SetActive(true);
                    break;
            }
        }
        else
            choiceMenu.SetActive(true);
    }

    // Сучий костыль
    private async Task BigPictureActivate() => bigPictureMenu.SetActive(true);

    /// <summary>
    /// Активация + генерация меню с выборами
    /// </summary>
    public async void ActivateChoiceMenu()
    {
        _selectedTextDialog.text = "";
        choiceMenu.SetActive(true);

        foreach (Transform child in choicesContainer.transform)
            Destroy(child.gameObject);

        for (int i = 0; i < currentDialogStepNode.choices; i++)
        {
            if (_dialogsManager.dialogsTempInfo[story].lockedChoices[currentDialogStepNode].choiceLock) continue; // Если выбор заблокирован
            
            var obj = Instantiate(buttonChoicePrefab, Vector3.zero, new Quaternion(),
                choicesContainer.transform);

            obj.transform.Find("Text").GetComponent<TextMeshProUGUI>().text =
                new LanguageSetting(currentDialogStepNode.choiceOptionsRu[i], currentDialogStepNode.choiceOptionsEn[i])
                    .Text;
            int num = i;
            obj.GetComponent<Button>().onClick.AddListener(() => { ActivateChoiceStep(num); });
            obj.transform.localPosition = Vector3.zero;
        }

        await adaptiveScrollViewChoice.UpdateContentSize();
    }

    /// <summary>
    /// Выбор шага диалога
    /// </summary>
    private void ActivateChoiceStep(int id)
    {
        choiceMenu.SetActive(false);
        _dialogsManager.DialogMoveNext(id);
    }

    public void UpdateDialogWindow(Npc npc)
    {
        switch (story.GetFirstNode().styleOfDialog)
        {
            case Enums.DialogStyle.Main:
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

    public void AddTalkerToDict(Npc npc) => _npcAndTalkerIcon.Add(npc, talkerIcons[_npcAndTalkerIcon.Count]);

    private async void SetIcon(Npc npc)
    {
        foreach (KeyValuePair<Npc, Image> talker in _npcAndTalkerIcon)
        {
            talker.Value.color = dontSelectedColor;
            talker.Value.rectTransform.sizeDelta = new Vector2(406, 564);
        }

        if (!currentDialogStepNode.character || !npc) return;

        foreach (KeyValuePair<Npc, Image> talker in _npcAndTalkerIcon)
        {
            if (npc != talker.Key) continue;
            talker.Value.sprite = npc.GetStyleIcon(currentDialogStepNode.mood);
            talker.Value.color = Color.white;
            talker.Value.rectTransform.sizeDelta = new Vector2(talker.Value.rectTransform.sizeDelta.x + 35, talker.Value.rectTransform.sizeDelta.y + 35);
        }
    }

    /// <summary>
    /// Закрытие окна диалога
    /// </summary>
    public async void CLoseDialogWindow()
    {
        Interactions.Instance.lockInter = false;

        if (story.GetFirstNode().styleOfDialog == Enums.DialogStyle.BigPicture)
            GameMenuManager.Instance.NoVisionForTime(1.5f, DoActionsToClose());
        else if (story.GetFirstNode().darkAfterEnd)
            GameMenuManager.Instance.NoVisionForTime(1.2f, DoActionsToClose());
        else
            await DoActionsToClose();
    }

    private async Task DoActionsToClose()
    {
        for (float i = 1; i > 0f; i -= 0.1f) // Плавно поднимаем
        {
            dialogMenu.GetComponent<RectTransform>().localScale = new Vector3(i, i, 1);
            await Task.Delay(10);
        }
        dialogMenu.SetActive(false);
        mainMenu.SetActive(false);
        subDialogMenu.SetActive(false);
        bigPictureMenu.SetActive(false);
        foreach (KeyValuePair<Npc, Image> talker in _npcAndTalkerIcon)
            talker.Value.sprite = GameMenuManager.Instance.NullSprite;

        _dialogsManager.DoActionsToClose();

        story = null;
        currentDialogStepNode = null;
        lastNode = false;
        _npcAndTalkerIcon = new Dictionary<Npc, Image>();
        await CameraManager.Instance.CameraZoom(0, true);
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