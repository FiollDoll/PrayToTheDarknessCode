using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class DialogsManager
{
    public static DialogsManager Instance { get; private set; }
    public DialogUI DialogUI;

    // Все диалоги
    private readonly Dictionary<string, Dialog> _dialogsDict = new Dictionary<string, Dialog>();
    public Dictionary<Dialog, DialogTempInfo> dialogsTempInfo = new Dictionary<Dialog, DialogTempInfo>();


    public List<Npc> NpcInSelectedDialog = new List<Npc>();

    public Task Initialize()
    {
        Instance = this;
        Dialog[] storyObjects = Resources.LoadAll<Dialog>("Dialogs");
        foreach (Dialog dialog in storyObjects)
        {
            _dialogsDict.Add(dialog.name, dialog);
            dialogsTempInfo.Add(dialog, new DialogTempInfo());

            if (dialog.GetFirstNode().thisDialogLock)
                dialogsTempInfo[dialog].dialogLock = true;

            // Записываем choice
            foreach (DialogStepNode node in dialog.nodes)
            {
                try
                {
                    if (node.choices == 1) continue;
                    dialogsTempInfo[dialog].AddChoicesForNode(node);
                }
                catch (System.Exception ex)
                {
                    Debug.Log(ex);
                }
            }
        }
        return Task.CompletedTask;
    }

    /// <summary>
    /// Поиск диалога
    /// </summary>
    public Dialog GetDialog(string nameDialog) => _dialogsDict.GetValueOrDefault(nameDialog);

    /// <summary>
    /// Проверка - пора ли активировать выборы и есть ли они вообще?
    /// </summary>
    public bool CanChoice() => DialogUI.currentDialogStepNode.choices > 1;

    public async Task ActivateDialog(string nameDialog)
    {
        Dialog newDialog = GetDialog(nameDialog);
        if (newDialog)
            await InitializeDialog(newDialog);
    }

    public async Task ActivateDialog(Dialog dialog) => await InitializeDialog(dialog);

    private async Task InitializeDialog(Dialog dialog)
    {
        if (dialogsTempInfo[dialog].dialogHasRead || dialogsTempInfo[dialog].dialogLock) return; // Запрет
        if (DialogUI.story != null) DialogCLose(); // Если какой-то диалог открыт

        DialogUI.story = dialog;
        DialogUI.currentDialogStepNode = DialogUI.story.GetFirstNode();
        Player.Instance.canMove = DialogUI.currentDialogStepNode.canMove;
        Interactions.Instance.lockInter = !DialogUI.currentDialogStepNode.canInter;
        if (!dialog.GetFirstNode().moreRead)
            dialogsTempInfo[dialog].dialogHasRead = true;

        await DialogUI.ActivateDialogWindow();

        if (!CanChoice())
            DialogUpdateAction();
        else
            DialogUI.ActivateChoiceMenu();
    }

    /// <summary>
    /// Закрытие диалога
    /// </summary>
    public void DialogCLose() => DialogUI.CloseDialogWindow();

    /// <summary>
    /// Выполнить действия для закрытия диалога
    /// </summary>
    public void DoActionsToClose()
    {
        NpcInSelectedDialog = new List<Npc>();
        Player.Instance.canMove = true;
        Player.Instance.virtualCamera.Follow = Player.Instance.transform;
    }

    /// <summary>
    /// Продвижение диалога далее(не для выбора)
    /// </summary>
    public void DialogMoveNext(int choiceId = 0)
    {
        // Окончание обычного диалога
        if (DialogUI.lastNode)
        {
            DialogCLose();
            return;
        }

        DialogUI.NextNode(choiceId);
        DialogUpdateAction();
    }

    /// <summary>
    /// Обновить текст и выполнить иные действия
    /// </summary>
    private void DialogUpdateAction()
    {
        DialogStepNode totalStep = DialogUI.currentDialogStepNode;
        // Проверка на нпс/личность игрока
        Npc npc;
        if (totalStep.character && totalStep.character.nameInWorld != "Mark")
            npc = totalStep.character;
        else
            npc = Player.Instance.NpcEntity;

        // Проверяем - был нип в диалоге или нет
        if (npc && !NpcInSelectedDialog.Contains(npc))
        {
            NpcInSelectedDialog.Add(npc);
            DialogUI.AddTalkerToDict(npc);
        }

        DialogUI.UpdateDialogWindow(npc);

        foreach (Npc totalNpc in NpcManager.Instance.AllNpc)
        {
            if (!totalNpc.canMeet) continue;
            if (totalNpc.nameOfNpc.Text != npc.nameOfNpc.Text) continue;

            NpcManager.Instance.npcTempInfo[totalNpc].meetWithPlayer = true;

            if (PlayerStats.FamiliarNpc.Contains(totalNpc))
                continue;
            PlayerStats.FamiliarNpc.Add(totalNpc);
            break;
        }

        PlayerStats.Sanity += totalStep.changeSanity;
        PlayerStats.Karma += totalStep.changeKarma;

        if (totalStep.dialogLock != null)
            dialogsTempInfo[totalStep.dialogLock].dialogLock = totalStep.changeDialogLock;

        if (totalStep.dialogChoiceLock != null)
            dialogsTempInfo[totalStep.dialogChoiceLock].FindLockedChoice(totalStep.dialogChoiceLock, totalStep.choiceNameToChange).choiceLock = totalStep.choiceNewState;

        if (totalStep.npcChangeRelation != null)
            NpcManager.Instance.npcTempInfo[totalStep.npcChangeRelation].relationshipWithPlayer += totalStep.changeRelation;

        if (totalStep.npcChangeMoveToPlayer != null)
            NpcManager.Instance.npcControllers[totalStep.npcChangeMoveToPlayer].moveToPlayer = totalStep.newStateMoveToPlayer;

        if (!DialogUI.lastNode) // Если не последняя нода
            totalStep.fastChanges?.ActivateChanges();

        if (totalStep.stepSpeech)
            AudioManager.Instance.PlaySpeech(totalStep.stepSpeech);
        else
            AudioManager.Instance.StopSpeech();
    }
}