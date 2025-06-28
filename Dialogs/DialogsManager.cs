using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using VNCreator;

public class DialogsManager
{
    public static DialogsManager Instance { get; private set; }
    public DialogUI DialogUI;

    // Все диалоги
    private readonly Dictionary<string, StoryObject> _dialogsDict = new Dictionary<string, StoryObject>();

    public List<Npc> NpcInSelectedDialog = new List<Npc>();

    public async Task Initialize()
    {
        Instance = this;
        StoryObject[] storyObjects = Resources.LoadAll<StoryObject>("Dialogs");
        foreach (StoryObject dialog in storyObjects)
            _dialogsDict.TryAdd(dialog.name, dialog);
    }

    /// <summary>
    /// Поиск диалога
    /// </summary>
    private StoryObject GetDialog(string nameDialog) => _dialogsDict.GetValueOrDefault(nameDialog);

    /// <summary>
    /// Поиск быстрых действий ДЛЯ диалогов
    /// </summary>
    /// <returns></returns>
    //private FastChangesController GetFastChanges(string name) => _fastChangesDict.GetValueOrDefault(name);

    /// <summary>
    /// Проверка - пора ли активировать выборы и есть ли они вообще?
    /// </summary>
    public bool CanChoice() => DialogUI.currentNode.choices > 1;

    /// <summary>
    ///  Активирует новый диалог
    /// </summary>
    /// <param name="nameDialog">Название диалога</param>
    public async Task ActivateDialog(string nameDialog) // Старт диалога
    {
        StoryObject newDialog = GetDialog(nameDialog);
        if (!newDialog) return;
        //if (newDialog.read) return;

        DialogUI.story = newDialog;
        DialogUI.currentNode = DialogUI.story.GetFirstNode();
        Player.Instance.canMove = DialogUI.currentNode.canMove;
        Interactions.Instance.lockInter = !DialogUI.currentNode.canInter;
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
    /// Продвижение диалога далее
    /// </summary>
    public void DialogMoveNext()
    {
        // Окончание обычного диалога
        if (DialogUI.lastNode)
        {
            DialogCLose();
            return;
        }

        DialogUI.NextNode(0);
        DialogUpdateAction();
    }

    /// <summary>
    /// Обновить текст и выполнить иные действия
    /// </summary>
    public void DialogUpdateAction()
    {
        Npc npc = NpcManager.Instance.GetNpcByName(DialogUI.currentNode.characterName);
        if (DialogUI.currentNode.characterName != "." && !NpcInSelectedDialog.Contains(npc))
        {
            NpcInSelectedDialog.Add(npc);
            DialogUI.UpdateTalkersDict(npc);
        }

        DialogUI.UpdateDialogWindow(npc);

        foreach (Npc totalNpc in NpcManager.Instance.AllNpc)
        {
            if (!totalNpc.canMeet) continue;
            if (totalNpc.nameOfNpc.text != DialogUI.currentNode.characterName) continue;
            if (Player.Instance.familiarNpc.Contains(totalNpc)) continue;
            Player.Instance.familiarNpc.Add(totalNpc);
            break;
        }

        //if (DialogUI.currentNode.fastChangesName != "")
            //GetFastChanges(DialogUI.currentNode.fastChangesName)?.ActivateChanges();

        CutsceneManager.Instance.ActivateCutsceneStep(DialogUI.currentNode.activateCutsceneStep);

        if (DialogUI.currentNode.stepSpeech)
            AudioManager.Instance.PlaySpeech(DialogUI.currentNode.stepSpeech);
        else
            AudioManager.Instance.StopSpeech();
    }
}