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
            _dialogsDict.TryAdd(dialog.name, dialog);
            dialogsTempInfo.TryAdd(dialog, new DialogTempInfo());
            // Перебираем все закрытые choice и записываем их
            foreach (DialogStepNode node in dialog.nodes)
            {
                if (node.choices > 1) continue;
                for (int i = 0; i < node.choices; i++)
                {
                    if (node.choiceLock[i])
                        dialogsTempInfo[dialog].lockedChoices.Add(node, new LockedChoices(node.choiceName[i], node.choiceLock[i]));
                }
            }
        }
        return Task.CompletedTask;
    }

    /// <summary>
    /// Поиск диалога
    /// </summary>
    private Dialog GetDialog(string nameDialog) => _dialogsDict.GetValueOrDefault(nameDialog);

    /// <summary>
    /// Проверка - пора ли активировать выборы и есть ли они вообще?
    /// </summary>
    public bool CanChoice() => DialogUI.currentDialogStepNode.choices > 1;

    /// <summary>
    ///  Активирует новый диалог
    /// </summary>
    /// <param name="nameDialog">Название диалога</param>
    public async Task ActivateDialog(string nameDialog) // Старт диалога
    {
        Dialog newDialog = GetDialog(nameDialog);
        if (!newDialog) return;
        if (dialogsTempInfo[newDialog].dialogHasRead || dialogsTempInfo[newDialog].dialogLock) return;

        DialogUI.story = newDialog;
        DialogUI.currentDialogStepNode = DialogUI.story.GetFirstNode();
        Player.Instance.canMove = DialogUI.currentDialogStepNode.canMove;
        Interactions.Instance.lockInter = !DialogUI.currentDialogStepNode.canInter;
        if (!newDialog.GetFirstNode().moreRead)
            dialogsTempInfo[newDialog].dialogHasRead = true;

        await DialogUI.ActivateDialogWindow();

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
            dialogsTempInfo[totalStep.dialogChoiceLock].FindLockedChoice(totalStep.choiceNameToChange).choiceLock = totalStep.choiceNewState;
        
        if (totalStep.npcChangeRelation != null)
            NpcManager.Instance.npcTempInfo[totalStep.npcChangeRelation].relationshipWithPlayer += totalStep.changeRelation;

        if (totalStep.npcChangeMoveToPlayer != null)
        {
            if (totalStep.npcChangeMoveToPlayer.IHumanable is NpcController npcController)
                npcController.moveToPlayer = totalStep.newStateMoveToPlayer;
        }

        totalStep.fastChanges?.ActivateChanges();

        if (totalStep.stepSpeech)
            AudioManager.Instance.PlaySpeech(totalStep.stepSpeech);
        else
            AudioManager.Instance.StopSpeech();
    }
}