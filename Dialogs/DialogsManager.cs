using System.Collections.Generic;
using UnityEngine;

public class DialogsManager
{
    public static DialogsManager Instance { get; private set; }
    public DialogUI DialogUI;

    // Все диалоги
    private readonly Dictionary<string, Dialog> _dialogsDict = new Dictionary<string, Dialog>();

    [Header("Current dialog")] public int TotalStep;
    public Dialog SelectedDialog;
    public StepBranch SelectedBranch;
    public DialogStep SelectedStep;
    public List<Npc> NpcInSelectedDialog = new List<Npc>();

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

        SelectedDialog = null;
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

        if (SelectedDialog != null)
            TotalStep = 0;

        SelectedDialog = newDialog;
        SelectedBranch = SelectedDialog.stepBranches[0];
        SelectedStep = SelectedBranch.dialogSteps[0];
        Player.Instance.canMove = SelectedDialog.canMove;
        Interactions.Instance.lockInter = !SelectedDialog.canInter;
        if (!SelectedDialog.moreRead)
            SelectedDialog.read = true;

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
    public void DialogCLose() => DialogUI.CLoseDialogWindow();

    /// <summary>
    /// Выполнить действия для закрытия диалога
    /// </summary>
    public void DoActionsToClose()
    {
        TotalStep = 0;
        //if (_activatedDialog.posAfterEnd)
        //Player.Instance.transform.localPosition = _activatedDialog.posAfterEnd.position;
        NpcInSelectedDialog = new List<Npc>();
        Player.Instance.canMove = true;
        Player.Instance.virtualCamera.Follow = Player.Instance.transform;
        SelectedDialog = null;
    }

    public void ChangeDialogBranch(string nameOfBranch)
    {
        StepBranch newBranch = SelectedDialog.FindBranch(nameOfBranch);
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

        if (SelectedStep.totalNpcName != "nothing" && !NpcInSelectedDialog.Contains(SelectedStep.totalNpc))
        {
            NpcInSelectedDialog.Add(SelectedStep.totalNpc);
            DialogUI.UpdateTalkersDict(SelectedStep.totalNpc);
        }

        DialogUI.UpdateDialogWindow();

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