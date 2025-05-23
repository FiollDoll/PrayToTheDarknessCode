using System.Collections.Generic;
using UnityEngine;

public class DialogsManager
{
    public static DialogsManager Instance { get; private set; }
    public DialogUI DialogUI;

    // Все диалоги
    private readonly Dictionary<string, Dialog> _dialogsDict = new Dictionary<string, Dialog>();

    [Header("Current dialog")] public int TotalStep;
    public Dialog ActivatedDialog;
    public StepBranch SelectedBranch;
    public DialogStep SelectedStep;
    public List<Npc> NpcInTotalDialog = new List<Npc>();

    public void Initialize()
    {
        Instance = this;
        DialogsLoader dl = new DialogsLoader();
        List<Dialog> dialogs = dl.LoadDialogs();
        foreach (Dialog dialog in dialogs)
        {
            _dialogsDict.TryAdd(dialog.nameDialog, dialog);
            dialog.UpdateDialogDicts();
        }

        ActivatedDialog = null;
        SelectedBranch = null;
        SelectedStep = null;
    }

    /// <summary>
    /// Поиск диалога
    /// </summary>
    private Dialog GetDialog(string nameDialog) => _dialogsDict.GetValueOrDefault(nameDialog);

    /// <summary>
    /// Проверка - пора ли активировать выборы и есть ли они вообще?
    /// </summary>
    public bool CanChoice() => SelectedBranch.choices.Count != 0 && TotalStep == SelectedBranch.dialogSteps.Count;

    /// <summary>
    ///  Активирует новый диалог
    /// </summary>
    /// <param name="nameDialog">Название диалога</param>
    public void ActivateDialog(string nameDialog) // Старт диалога
    {
        Dialog newDialog = GetDialog(nameDialog);
        if (newDialog == null) return;
        if (newDialog.read) return;

        if (ActivatedDialog != null)
        {
            TotalStep = 0;
            //StopAllCoroutines();
        }

        ActivatedDialog = newDialog;
        SelectedBranch = ActivatedDialog.stepBranches[0];
        SelectedStep = SelectedBranch.dialogSteps[0];
        Player.Instance.canMove = ActivatedDialog.canMove;
        Interactions.Instance.lockInter = !ActivatedDialog.canInter;
        if (!ActivatedDialog.moreRead)
            ActivatedDialog.read = true;

        DialogUI.ActivateDialogWindow();
        CameraManager.Instance.CameraZoom(-5f, true);

        if (!CanChoice())
            DialogUpdateAction();
        else
            DialogUI.ActivateChoiceMenu();
    }

    /// <summary>
    /// Закрытие диалога
    /// </summary>
    public void DialogCLose() => DialogUI.DialogCLose();

    /// <summary>
    /// Выполнить действия для закрытия диалога
    /// </summary>
    public void DoActionToClose()
    {
        TotalStep = 0;
        //if (_activatedDialog.posAfterEnd)
        //Player.Instance.transform.localPosition = _activatedDialog.posAfterEnd.position;
        NpcInTotalDialog = new List<Npc>();
        Player.Instance.canMove = true;
        Player.Instance.virtualCamera.Follow = Player.Instance.transform;
        ActivatedDialog = null;
    }

    public void ChangeDialogBranch(string nameOfBranch)
    {
        StepBranch newBranch = ActivatedDialog.FindBranch(nameOfBranch);
        SelectedBranch = newBranch;
        SelectedStep = SelectedBranch.dialogSteps[0];
        DialogUI.ActivateDialogWindow();

        if (!CanChoice())
            DialogUpdateAction();
        else
            DialogUI.ActivateChoiceMenu();
    }

    /// <summary>
    /// Продвижение диалога далее
    /// </summary>
    public void DialogMoveNext()
    {
        bool AddStep()
        {
            TotalStep++;
            if (TotalStep != SelectedBranch.dialogSteps.Count)
            {
                SelectedStep = SelectedBranch.dialogSteps[TotalStep];
                DialogUpdateAction();
                return true;
            }

            // Продолжение не найдено, выдаём false
            return false;
        }

        // Окончание обычного диалога
        if (!AddStep())
        {
            // Если выбора нет.
            if (!CanChoice())
                DialogCLose();
            else
                DialogUI.ActivateChoiceMenu();
        }
    }

    /// <summary>
    /// Обновить текст и выполнить иные действия
    /// </summary>
    public void DialogUpdateAction()
    {
        SelectedStep.UpdateStep();

        if (SelectedStep.totalNpcName != "nothing" && !NpcInTotalDialog.Contains(SelectedStep.totalNpc))
            NpcInTotalDialog.Add(SelectedStep.totalNpc);

        DialogUI.UpdateUI();

        foreach (Npc totalNpc in NpcManager.Instance.AllNpc)
        {
            if (!totalNpc.canMeet) continue;
            if (totalNpc.nameOfNpc != SelectedStep.totalNpc.nameOfNpc) continue;
            if (Player.Instance.familiarNpc.Contains(totalNpc)) continue;
            Player.Instance.familiarNpc.Add(totalNpc);
            break;
        }

        SelectedStep.fastChanges.ActivateChanges();

        CutsceneManager.Instance.ActivateCutsceneStep(SelectedStep.activateCutsceneStep);

        if (SelectedStep.stepSpeech != "")
            AudioManager.Instance.PlaySpeech(SelectedStep.GetSpeech());
        else
            AudioManager.Instance.StopSpeech();
    }
}