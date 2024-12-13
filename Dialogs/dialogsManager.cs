using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.Serialization;

public class DialogsManager : MonoBehaviour
{
    private TextMeshProUGUI _textName, _textDialog;
    public GameObject dialogMenu;
    [SerializeField] private GameObject mainDialogMenu, choiceDialogMenu, subDialogMenu;
    [SerializeField] private GameObject choicesContainer, buttonChoicePrefab;
    [SerializeField] private NPC[] allNpc = new NPC[0];
    [SerializeField] private Image iconImage;
    [SerializeField] private Image bigPicture;
    [SerializeField] private Image noViewPanel;
    [SerializeField] private AllScripts _scripts;

    public int totalStep;
    private string _totalMode;
    private bool _animatingText, _canStepNext;
    public Dialog[] dialogs = new Dialog[0];
    private Dialog _activatedDialog;
    private DialogStepChoice _selectedChoice;
    private DialogStep _selectedStep;

    private void Start()
    {
        mainDialogMenu.GetComponent<RectTransform>().DOPivotY(4f, 0.3f);
        choiceDialogMenu.GetComponent<RectTransform>().DOPivotY(4f, 0.3f);
    }

    public void ActivateDialog(string nameDialog) // Старт диалога
    {
        if (_activatedDialog == null)
        {
            foreach (Dialog totalDialog in dialogs)
            {
                if (totalDialog.nameDialog == nameDialog && !totalDialog.readed)
                {
                    _activatedDialog = totalDialog;
                    dialogMenu.gameObject.SetActive(true);
                    _canStepNext = false;
                    _scripts.interactions.lockInter = !totalDialog.canInter;
                    if (_activatedDialog.steps.Length > 0)
                    {
                        if (_activatedDialog.styleOfDialog == Dialog.DialogStyle.Main)
                        {
                            mainDialogMenu.gameObject.SetActive(true);
                            _textName = mainDialogMenu.transform.Find("TextName").GetComponent<TextMeshProUGUI>();
                            _textDialog = mainDialogMenu.transform.Find("TextDialog").GetComponent<TextMeshProUGUI>();
                        }
                        else
                        {
                            subDialogMenu.gameObject.SetActive(true);
                            _textDialog = subDialogMenu.transform.Find("Text").GetComponent<TextMeshProUGUI>();
                        }

                        _selectedStep = _activatedDialog.steps[0];
                        _totalMode = "dialog";
                    }
                    else
                    {
                        choiceDialogMenu.gameObject.SetActive(true);
                        _totalMode = "choice";
                    }

                    _scripts.main.EndCursedText(_textDialog);

                    if (_activatedDialog.bigPicture != null)
                    {
                        Sequence sequence = DOTween.Sequence();
                        Tween stepSequence = noViewPanel.DOFade(100f, 0.5f).SetEase(Ease.InQuart);
                        stepSequence.OnComplete(() =>
                        {
                            bigPicture.sprite = _activatedDialog.bigPicture;
                            if (_activatedDialog.bigPictureSecond != null)
                                StartCoroutine(BigPictureAnimate(_activatedDialog));
                        });
                        sequence.Append(stepSequence);
                        sequence.Append(bigPicture.DOFade(100f, 0.001f).SetEase(Ease.InQuart));
                        sequence.Append(noViewPanel.DOFade(0f, 0.5f).SetEase(Ease.OutQuart));
                        sequence.Insert(0, transform.DOScale(new Vector3(1, 1, 1), sequence.Duration()));
                    }
                    else
                        bigPicture.sprite = _scripts.main.nullSprite;

                    if (_activatedDialog.mainPanelStartDelay == 0)
                        OpenPanels();
                    else
                        StartCoroutine(ActivateUIMainMenuWithDelay(_activatedDialog,
                            _activatedDialog.mainPanelStartDelay));

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
            sequence = sequence.Append(noViewPanel.DOFade(100f, 0.5f).SetEase(Ease.InQuart));
            sequence.Append(mainDialogMenu.GetComponent<RectTransform>().DOPivotY(4f, 0.3f));
            sequence.Append(bigPicture.DOFade(0f, 0.1f).SetEase(Ease.OutQuart));
            sequence.Append(noViewPanel.DOFade(0f, 0.5f).SetEase(Ease.OutQuart));
        }
        else
            sequence.Append(mainDialogMenu.GetComponent<RectTransform>().DOPivotY(4f, 0.3f));

        choiceDialogMenu.GetComponent<RectTransform>().DOPivotY(4f, 0.5f);

        if (_activatedDialog.darkAfterEnd && !_activatedDialog.disableFadeAtEnd)
        {
            Tween extraSequence = noViewPanel.DOFade(100f, 0.7f).SetEase(Ease.InQuart);
            if (_activatedDialog.posAfterEnd != null)
            {
                extraSequence.OnComplete(() =>
                {
                    _scripts.player.transform.localPosition = _activatedDialog.posAfterEnd.position;
                });
            }

            sequence.Append(extraSequence);
            sequence.Append(noViewPanel.DOFade(0f, 0.7f).SetEase(Ease.OutQuart));
        }

        sequence.Insert(0, transform.DOScale(new Vector3(1, 1, 1), sequence.Duration()));
        sequence.OnComplete(() =>
        {
            dialogMenu.gameObject.SetActive(false);
            mainDialogMenu.gameObject.SetActive(false);
            subDialogMenu.gameObject.SetActive(false);
            if (_activatedDialog.nextStepQuest)
                _scripts.questsSystem.NextStep();
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

    private void ActivateChoiceStep(int id)
    {
        _totalMode = "choice";
        _selectedChoice = _activatedDialog.dialogsChoices[id];
        if (!_selectedChoice.moreRead)
            _selectedChoice.readed = true;
        choiceDialogMenu.gameObject.SetActive(false);
        mainDialogMenu.gameObject.SetActive(true);
        totalStep = 0;
        _selectedStep = _selectedChoice.steps[0];
        if (_selectedStep.cameraTarget != null)
            _scripts.player.virtualCamera.Follow = _selectedStep.cameraTarget;
        _textName.text = _selectedStep.totalNpc.nameOfNpc;
        iconImage.sprite = _selectedStep.icon;
        if (GameObject.Find(_selectedStep.totalNpc.nameInWorld) && _selectedStep.animateTalking)
            GameObject.Find(_selectedStep.totalNpc.nameInWorld).GetComponent<Animator>().SetBool("talk", true);
        StartCoroutine(SetText(_selectedStep.text));
    }

    private void ActivateChoiceMenu(bool setScale = false)
    {
        choiceDialogMenu.gameObject.SetActive(true);
        choiceDialogMenu.transform.Find("Scroll View").GetComponent<AdaptiveScrollView>().UpdateContentSize();
        if (setScale)
            choiceDialogMenu.GetComponent<RectTransform>().localScale = new Vector2(1f, 1f);
        foreach (Transform child in choicesContainer.transform)
            Destroy(child.gameObject);

        for (int i = 0; i < _activatedDialog.dialogsChoices.Length; i++)
        {
            var obj = Instantiate(buttonChoicePrefab, new Vector3(0, 0, 0), Quaternion.identity,
                choicesContainer.transform);
            obj.transform.Find("Text").GetComponent<TextMeshProUGUI>().text =
                _activatedDialog.dialogsChoices[i].textQuestion;
            int num = i;
            obj.GetComponent<Button>().onClick.AddListener(delegate { ActivateChoiceStep(num); });
            if (_activatedDialog.dialogsChoices[i].readed)
                obj.GetComponent<Button>().interactable = false;
        }

        choiceDialogMenu.transform.Find("Scroll View").GetComponent<AdaptiveScrollView>().UpdateContentSize();
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

        switch (_totalMode)
        {
            // Окончание обычного диалога
            case "dialog" when (totalStep + 1) == _activatedDialog.steps.Length:
            {
                if (_activatedDialog.dialogsChoices.Length == 0)
                    DialogCLose();
                else
                    ActivateChoiceMenu(true);
                return;
            }
            case "choice" when (totalStep + 1) == _selectedChoice.steps.Length:
            {
                if (_selectedChoice.returnToStartChoices)
                    ActivateChoiceMenu(true);
                else
                    DialogCLose();
                return;
            }
            default:
                AddStep();
                break;
        }
    }

    private void DialogUpdateAction()
    {
        if (_activatedDialog.styleOfDialog == Dialog.DialogStyle.Main)
        {
            _textName.text = _selectedStep.totalNpc.nameOfNpc;
            iconImage.sprite = _selectedStep.icon;
            iconImage.SetNativeSize();
            if (_selectedStep.shakeIcon)
                iconImage.GetComponent<RectTransform>().DOPunchAnchorPos(new Vector3(10, 10, 10), 2f, 10);
            else
                iconImage.GetComponent<RectTransform>().DOPunchAnchorPos(new Vector3(1, 1, 1), 5f, 3);
        }

        foreach (NPC totalNpc in allNpc)
        {
            if (totalNpc.nameOfNpc != _selectedStep.totalNpc.nameOfNpc) continue;
            if (_scripts.player.familiarNPC.Contains(totalNpc)) continue;
            _scripts.player.familiarNPC.Add(totalNpc);
            break;
        }

        if (_selectedStep.setCloseMeet)
            _selectedStep.totalNpc.playerCloseMeet = true;

        if (GameObject.Find(_selectedStep.totalNpc.nameInWorld) && _selectedStep.animateTalking)
            GameObject.Find(_selectedStep.totalNpc.nameInWorld).GetComponent<Animator>().SetBool("talk", true);

        if (_selectedStep.cursedText)
            _scripts.main.SetCursedText(_textDialog, Random.Range(5, 40));
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

    private void OpenPanels()
    {
        mainDialogMenu.GetComponent<RectTransform>().GetComponent<RectTransform>().DOPivotY(0.5f, 0.3f)
            .SetEase(Ease.InQuart).OnComplete(() => { _canStepNext = true; });
        choiceDialogMenu.GetComponent<RectTransform>().DOPivotY(0.5f, 0.3f).SetEase(Ease.InQuart).OnComplete(() =>
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
            if (!_animatingText) continue;
            _textDialog.text += tChar;
            yield return new WaitForSeconds(0.05f);
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

    private IEnumerator BigPictureAnimate(Dialog totalDialog)
    {
        bigPicture.sprite = totalDialog.bigPicture;
        yield return new WaitForSeconds(0.35f);
        bigPicture.sprite = totalDialog.bigPictureSecond;
        yield return new WaitForSeconds(0.35f);
        if (totalDialog.bigPictureThird != null)
        {
            bigPicture.sprite = totalDialog.bigPictureThird;
            yield return new WaitForSeconds(0.35f);
        }

        StartCoroutine(BigPictureAnimate(totalDialog));
    }

    private IEnumerator ActivateUIMainMenuWithDelay(Dialog totalDialog, float delay)
    {
        yield return new WaitForSeconds(delay);
        OpenPanels();
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
                    _textDialog.text = _selectedStep.text;
                }
                else
                    DialogMoveNext();
            }
        }
    }
}