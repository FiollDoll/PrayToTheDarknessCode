using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class dialogsManager : MonoBehaviour
{
    private TextMeshProUGUI _textName, _textDialog;
    [SerializeField] private Image _iconImage, _bigPicture, _noViewPanel;
    public GameObject dialogMenu;
    [SerializeField] private GameObject _mainDialogMenu, _choiceDialogMenu, _subDialogMenu, _choicesContainer;
    [SerializeField] private GameObject _buttonChoicePrefab;
    [SerializeField] private NPC[] allNpc = new NPC[0];
    [SerializeField] private allScripts _scripts;

    public int totalStep;
    private string _totalMode;
    private bool _animatingText, _canStepNext;
    public dialog[] dialogs = new dialog[0];
    private dialog _activatedDialog;
    private dialogStepChoice _selectedChoice;
    private dialogStep _selectedStep;

    private void Start()
    {
        _mainDialogMenu.GetComponent<RectTransform>().DOPivotY(4f, 0.3f);
        _choiceDialogMenu.GetComponent<RectTransform>().DOPivotY(4f, 0.3f);
    }

    public void ActivateDialog(string name) // Старт диалога
    {
        if (_activatedDialog == null)
        {
            foreach (dialog totalDialog in dialogs)
            {
                if (totalDialog.nameDialog == name && !totalDialog.readed)
                {
                    _activatedDialog = totalDialog;
                    dialogMenu.gameObject.SetActive(true);
                    _canStepNext = false;
                    _scripts.interactions.lockInter = !totalDialog.canInter;
                    if (_activatedDialog.steps.Length > 0)
                    {
                        if (_activatedDialog.styleOfDialog == dialog.dialogStyle.main)
                        {
                            _mainDialogMenu.gameObject.SetActive(true);
                            _textName = _mainDialogMenu.transform.Find("TextName").GetComponent<TextMeshProUGUI>();
                            _textDialog = _mainDialogMenu.transform.Find("TextDialog").GetComponent<TextMeshProUGUI>();
                        }
                        else
                        {
                            _subDialogMenu.gameObject.SetActive(true);
                            _textDialog = _subDialogMenu.transform.Find("Text").GetComponent<TextMeshProUGUI>();
                        }
                        _selectedStep = _activatedDialog.steps[0];
                        _totalMode = "dialog";
                    }
                    else
                    {
                        _choiceDialogMenu.gameObject.SetActive(true);
                        _totalMode = "choice";
                    }

                    _scripts.main.EndCursedText(_textDialog);

                    if (_activatedDialog.bigPicture != null)
                    {
                        Sequence sequence = DOTween.Sequence();
                        Tween stepSequence = _noViewPanel.DOFade(100f, 0.5f).SetEase(Ease.InQuart);
                        stepSequence.OnComplete(() =>
                        {
                            _bigPicture.sprite = _activatedDialog.bigPicture;
                            if (_activatedDialog.bigPictureSecond != null)
                                StartCoroutine(bigPictureAnimate(_activatedDialog));
                        });
                        sequence.Append(stepSequence);
                        sequence.Append(_noViewPanel.DOFade(0f, 0.5f).SetEase(Ease.OutQuart));
                        sequence.Insert(0, transform.DOScale(new Vector3(1, 1, 1), sequence.Duration()));
                    }
                    else
                        _bigPicture.sprite = _scripts.main.nullSprite;

                    if (_activatedDialog.mainPanelStartDelay == 0)
                        OpenPanels();
                    else
                        StartCoroutine(ActivateUIMainMenuWithDelay(_activatedDialog, _activatedDialog.mainPanelStartDelay));

                    if (!_activatedDialog.moreRead)
                        _activatedDialog.readed = true;
                    _scripts.player.canMove = totalDialog.canMove;

                    if (_totalMode == "dialog")
                        DialogUpdateAction();
                    else
                        ActivateChoiceMenu();
                    break;
                }
            }
        }
    }

    public void DialogCLose()
    {
        _canStepNext = false;
        _scripts.interactions.lockInter = false;
        if (_activatedDialog.activatedCutsceneStepAtEnd != -1)
            _scripts.cutsceneManager.ActivateCutsceneStep(_activatedDialog.activatedCutsceneStepAtEnd);

        totalStep = 0;
        Sequence sequence = DOTween.Sequence();
        if (_activatedDialog.bigPicture != null && !_activatedDialog.disableFadeAtEnd)
        {
            sequence = sequence.Append(_noViewPanel.DOFade(100f, 0.5f).SetEase(Ease.InQuart));
            sequence.Append(_mainDialogMenu.GetComponent<RectTransform>().DOPivotY(4f, 0.3f));
            sequence.Append(_bigPicture.DOFade(0f, 0.1f).SetEase(Ease.OutQuart));
            sequence.Append(_noViewPanel.DOFade(0f, 0.5f).SetEase(Ease.OutQuart));
        }
        else
            sequence.Append(_mainDialogMenu.GetComponent<RectTransform>().DOPivotY(4f, 0.3f));
        _choiceDialogMenu.GetComponent<RectTransform>().DOPivotY(4f, 0.5f);

        if (_activatedDialog.darkAfterEnd && !_activatedDialog.disableFadeAtEnd)
        {
            Tween extraSequence = _noViewPanel.DOFade(100f, 0.7f).SetEase(Ease.InQuart);
            if (_activatedDialog.posAfterEnd != null)
            {
                extraSequence.OnComplete(() =>
                {
                    _scripts.player.transform.localPosition = _activatedDialog.posAfterEnd.position;
                });
            }
            sequence.Append(extraSequence);
            sequence.Append(_noViewPanel.DOFade(0f, 0.7f).SetEase(Ease.OutQuart));
        }
        sequence.Insert(0, transform.DOScale(new Vector3(1, 1, 1), sequence.Duration()));
        sequence.OnComplete(() =>
        {
            dialogMenu.gameObject.SetActive(false);
            _mainDialogMenu.gameObject.SetActive(false);
            _subDialogMenu.gameObject.SetActive(false);
            if (_activatedDialog.nextStepQuest)
                _scripts.quests.NextStep();
            _scripts.player.canMove = true;
            _scripts.main.EndCursedText(_textDialog);
            _scripts.player.virtualCamera.Follow = _scripts.player.transform;
            if (_activatedDialog.noteAdd != "")
                _scripts.notebook.AddNote(_activatedDialog.noteAdd);
            string newDialog = _activatedDialog.startNewDialogAfterEnd;
            _activatedDialog = null;
            _canStepNext = true;
            if (newDialog != "")
                ActivateDialog(newDialog);
        });
    }

    public void ActivateChoiceStep(int id)
    {
        _totalMode = "choice";
        _selectedChoice = _activatedDialog.dialogsChoices[id];
        if (!_selectedChoice.moreRead)
            _selectedChoice.readed = true;
        _choiceDialogMenu.gameObject.SetActive(false);
        _mainDialogMenu.gameObject.SetActive(true);
        totalStep = 0;
        _selectedStep = _selectedChoice.steps[0];
        if (_selectedStep.cameraTarget != null)
            _scripts.player.virtualCamera.Follow = _selectedStep.cameraTarget;
        _textName.text = _selectedStep.totalNpc.name;
        _iconImage.sprite = _selectedStep.icon;
        if (GameObject.Find(_selectedStep.totalNpc.nameInWorld) && _selectedStep.animateTalking)
            GameObject.Find(_selectedStep.totalNpc.nameInWorld).GetComponent<Animator>().SetBool("talk", true);
        StartCoroutine(SetText(_selectedStep.text));
    }

    private void ActivateChoiceMenu(bool setScale = false)
    {
        _choiceDialogMenu.gameObject.SetActive(true);
        _choiceDialogMenu.transform.Find("Scroll View").GetComponent<AdaptiveScrollView>().UpdateContentSize();
        if (setScale)
            _choiceDialogMenu.GetComponent<RectTransform>().localScale = new Vector2(1f, 1f);
        foreach (Transform child in _choicesContainer.transform)
            Destroy(child.gameObject);

        for (int i = 0; i < _activatedDialog.dialogsChoices.Length; i++)
        {
            var obj = Instantiate(_buttonChoicePrefab, new Vector3(0, 0, 0), Quaternion.identity, _choicesContainer.transform);
            obj.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = _activatedDialog.dialogsChoices[i].textQuestion;
            int num = i;
            obj.GetComponent<Button>().onClick.AddListener(delegate { ActivateChoiceStep(num); });
            if (_activatedDialog.dialogsChoices[i].readed)
                obj.GetComponent<Button>().interactable = false;
        }
        _choiceDialogMenu.transform.Find("Scroll View").GetComponent<AdaptiveScrollView>().UpdateContentSize();
    }

    private void DialogMoveNext()
    {
        void AddStep()
        {
            totalStep++;
            if (_totalMode == "dialog")
                _selectedStep = _activatedDialog.steps[totalStep];
            else
                _selectedStep = _selectedChoice.steps[totalStep];
            DialogUpdateAction();
        }

        if (GameObject.Find(_selectedStep.totalNpc.nameInWorld) && _selectedStep.animateTalking)
            GameObject.Find(_selectedStep.totalNpc.nameInWorld).GetComponent<Animator>().SetBool("talk", false);

        if (_totalMode == "dialog" && (totalStep + 1) == _activatedDialog.steps.Length) // Окончание обычного диалога
        {
            if (_activatedDialog.dialogsChoices.Length == 0)
                DialogCLose();
            else
                ActivateChoiceMenu(true);
            return;
        }
        else if (_totalMode == "choice" && (totalStep + 1) == _selectedChoice.steps.Length)
        {
            if (_selectedChoice.returnToStartChoices)
                ActivateChoiceMenu(true);
            else
                DialogCLose();
            return;
        }
        AddStep();
    }

    private void DialogUpdateAction()
    {
        if (_activatedDialog.styleOfDialog == dialog.dialogStyle.main)
        {
            _textName.text = _selectedStep.totalNpc.name;
            _iconImage.sprite = _selectedStep.icon;
            if (_selectedStep.shakeIcon)
                _iconImage.GetComponent<RectTransform>().DOPunchAnchorPos(new Vector3(10, 10, 10), 2f, 10);
            else
                _iconImage.GetComponent<RectTransform>().DOPunchAnchorPos(new Vector3(1, 1, 1), 5f, 3);
        }

        foreach (NPC totalNpc in allNpc)
        {
            if (totalNpc.name == _selectedStep.totalNpc.name)
            {
                if (!_scripts.player.familiarNPC.Contains(totalNpc))
                {
                    _scripts.player.familiarNPC.Add(totalNpc);
                    break;
                }
            }
        }
        if (_selectedStep.setCloseMeet)
            _selectedStep.totalNpc.playerCloseMeet = true;

        if (GameObject.Find(_selectedStep.totalNpc.nameInWorld) && _selectedStep.animateTalking)
            GameObject.Find(_selectedStep.totalNpc.nameInWorld).GetComponent<Animator>().SetBool("talk", true);

        if (_selectedStep.cursedText)
            _scripts.main.SetCursedText(_textDialog, Random.Range(5, 40));
        else
            StartCoroutine(SetText(_selectedStep.text));

        if (_selectedStep.cameraTarget != null)
            _scripts.player.virtualCamera.Follow = _selectedStep.cameraTarget;
        else
            _scripts.player.virtualCamera.Follow = _scripts.player.transform;

        if (_selectedStep.questStart != "")
            _scripts.quests.ActivateQuest(_selectedStep.questStart, true);
        if (_selectedStep.activatedCutsceneStep != -1)
            _scripts.cutsceneManager.ActivateCutsceneStep(_selectedStep.activatedCutsceneStep);
    }

    private void OpenPanels()
    {
        _mainDialogMenu.GetComponent<RectTransform>().GetComponent<RectTransform>().DOPivotY(0.5f, 0.3f).SetEase(Ease.InQuart).OnComplete(() =>
        {
            _canStepNext = true;
        });
        _choiceDialogMenu.GetComponent<RectTransform>().DOPivotY(0.5f, 0.3f).SetEase(Ease.InQuart).OnComplete(() =>
        {
            _canStepNext = true;
        });
    }

    private IEnumerator SetText(string text)
    {
        _textDialog.text = "";
        _animatingText = true;
        char[] textChar = text.ToCharArray();
        foreach (char tChar in textChar)
        {
            if (_animatingText)
            {
                _textDialog.text += tChar;
                yield return new WaitForSeconds(0.05f);
            }
        }
        _animatingText = false;
        if (_selectedStep.delayAfterNext != 0)
        {
            yield return new WaitForSeconds(_selectedStep.delayAfterNext);
            if (_totalMode == "dialog" && (totalStep + 1) == _activatedDialog.steps.Length)
                DialogCLose();
            else
                DialogMoveNext();
        }
    }

    private IEnumerator bigPictureAnimate(dialog totalDialog)
    {
        _bigPicture.sprite = totalDialog.bigPicture;
        yield return new WaitForSeconds(0.35f);
        _bigPicture.sprite = totalDialog.bigPictureSecond;
        yield return new WaitForSeconds(0.35f);
        if (totalDialog.bigPictureThird != null)
        {
            _bigPicture.sprite = totalDialog.bigPictureThird;
            yield return new WaitForSeconds(0.35f);
        }
        StartCoroutine(bigPictureAnimate(totalDialog));
    }

    private IEnumerator ActivateUIMainMenuWithDelay(dialog totalDialog, float delay)
    {
        yield return new WaitForSeconds(delay);
        OpenPanels();
    }

    private void Update()
    {
        if (_activatedDialog != null)
        {
            if (_selectedStep.delayAfterNext == 0)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    if (_canStepNext)
                    {
                        if (_animatingText)
                        {
                            _animatingText = false;
                            StopAllCoroutines();
                            _textDialog.text = _selectedStep.text;
                        }
                        else
                            DialogMoveNext();
                    }
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
    public enum dialogStyle { main, subMain };
    public dialogStyle styleOfDialog;
    public bool canMove, canInter;
    [Tooltip("Обычные этапы диалога")] public dialogStep[] steps = new dialogStep[0];
    [Tooltip("Диалоги с выбором варианта. Начинается после steps")] public dialogStepChoice[] dialogsChoices = new dialogStepChoice[0];
    [Header("AfterEnd")]
    public string startNewDialogAfterEnd;
    public bool darkAfterEnd;
    [Tooltip("Работает только с darkAfterEnd")] public Transform posAfterEnd;
    [Header("Other")]
    public string noteAdd;
    public Sprite bigPicture, bigPictureSecond, bigPictureThird;
    public int activatedCutsceneStepAtEnd = -1;
    public bool disableFadeAtEnd;
    public float mainPanelStartDelay; // Задержка
    public bool nextStepQuest;
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
    public bool cursedText;
    public bool animateTalking = true;
    public bool setCloseMeet;
    public float delayAfterNext;
    public enum iconMood { standart, happy, angry, sad, scary, wonder, confusion, curse }
    public iconMood iconMoodSelected;
    public Sprite icon
    {
        get
        {
            return iconMoodSelected switch
            {
                iconMood.standart => totalNpc.icon.standartIcon,
                iconMood.happy => totalNpc.icon.happyIcon,
                iconMood.sad => totalNpc.icon.sadIcon,
                iconMood.scary => totalNpc.icon.scaryIcon,
                iconMood.wonder => totalNpc.icon.wonderIcon,
                iconMood.confusion => totalNpc.icon.confusionIcon,
                iconMood.angry => totalNpc.icon.angryIcon,
                iconMood.curse => totalNpc.icon.curseIcon,
            };
        }
    }
    public bool shakeIcon;
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
    public bool readed, moreRead;
    public bool returnToStartChoices;
}
