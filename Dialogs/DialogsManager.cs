using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class DialogsManager
{
    public static DialogsManager Instance { get; private set; }
    public DialogUI DialogUI;

    // Все диалоги
    private readonly Dictionary<string, Dialog> _dialogsDict = new Dictionary<string, Dialog>();

    public List<Npc> NpcInSelectedDialog = new List<Npc>();

    public async Task Initialize()
    {
        Instance = this;
        Dialog[] storyObjects = Resources.LoadAll<Dialog>("Dialogs");
        foreach (Dialog dialog in storyObjects)
            _dialogsDict.TryAdd(dialog.name, dialog);
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
        //if (newDialog.read) return;

        DialogUI.story = newDialog;
        DialogUI.currentDialogStepNode = DialogUI.story.GetFirstNode();
        Player.Instance.canMove = DialogUI.currentDialogStepNode.canMove;
        Interactions.Instance.lockInter = !DialogUI.currentDialogStepNode.canInter;
        //if (!SelectedDialog.moreRead)
        //SelectedDialog.read = true;

        DialogUI.ActivateDialogWindow();
        await CameraManager.Instance.CameraZoom(-5f, true);

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
        //if (_activatedDialog.posAfterEnd)
        //Player.Instance.transform.localPosition = _activatedDialog.posAfterEnd.position;
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
    public void DialogUpdateAction()
    {
        // Проверка на пустого нипа
        Npc npc = DialogUI.currentDialogStepNode.character
            ? DialogUI.currentDialogStepNode.character
            : NpcManager.Instance.GetNpcByName(".");

        // Проверяем - был нип в диалоге или нет
        if (npc && !NpcInSelectedDialog.Contains(npc))
        {
            NpcInSelectedDialog.Add(npc);
            DialogUI.UpdateTalkersDict(npc);
        }

        DialogUI.UpdateDialogWindow(npc);

        foreach (Npc totalNpc in NpcManager.Instance.AllNpc)
        {
            if (!totalNpc.canMeet) continue;
            if (totalNpc.nameOfNpc.text != npc.nameOfNpc.text) continue;
            if (Player.Instance.familiarNpc.Contains(totalNpc)) continue;
            Player.Instance.familiarNpc.Add(totalNpc);
            break;
        }

        if (DialogUI.currentDialogStepNode.fastChanges)
            DialogUI.currentDialogStepNode.fastChanges.ActivateChanges();

        CutsceneManager.Instance.ActivateCutsceneStep(DialogUI.currentDialogStepNode.activateCutsceneStep);

        if (DialogUI.currentDialogStepNode.stepSpeech)
            AudioManager.Instance.PlaySpeech(DialogUI.currentDialogStepNode.stepSpeech);
        else
            AudioManager.Instance.StopSpeech();
    }
}