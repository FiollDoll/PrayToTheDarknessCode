using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cinemachine;

public class CutsceneManager
{
    public static CutsceneManager Instance { get; private set; }
    public Cutscene TotalCutscene;
    private CinemachineVirtualCamera _virtualCamera;
    private readonly Dictionary<string, Cutscene> _allCutsceneDict = new Dictionary<string, Cutscene>();

    public Task Initialize()
    {
        Instance = this;
        Cutscene[] allCutscenes = Resources.LoadAll<Cutscene>("Cutscenes/");
        foreach (Cutscene cutscene in allCutscenes)
            _allCutsceneDict.Add(cutscene.cutsceneName, cutscene);

        _virtualCamera = Player.Instance.virtualCamera;
        return Task.CompletedTask;
    }

    public Task ActivateCutscene(string cutsceneName)
    {
        if (string.IsNullOrEmpty(cutsceneName)) return Task.CompletedTask;
        TotalCutscene = _allCutsceneDict.GetValueOrDefault(cutsceneName);
        if (TotalCutscene)
            ActivateCutsceneStep(0);
        return Task.CompletedTask;
    }

    public void ActivateCutsceneStep(int step)
    {
        if (!TotalCutscene || step == -1)
            return;

        if (TotalCutscene.steps[step].timeDarkStart != 0)
        {
            // Где
        }
        else
            StepDo(step);

        if (TotalCutscene.steps[step].delayAndNext != 0)
            DelayAndNext(TotalCutscene.steps[step].delayAndNext, step);
    }

    private async void StepDo(int step) // Выполнить шаг катсцены.
    {
        CutsceneStep totalCutsceneStep = TotalCutscene.steps[step];
        if (totalCutsceneStep.chapterNext != "")
            ChapterManager.Instance.StartLoadChapter(
                ChapterManager.Instance.GetChapterByName(totalCutsceneStep.chapterNext));
        if (totalCutsceneStep.startViewMenuActivate != "")
            GameMenuManager.Instance.ViewMenuActivate(totalCutsceneStep.startViewMenuActivate);

        if (totalCutsceneStep.closeDialogMenu)
            DialogsManager.Instance.DialogCLose();

        await totalCutsceneStep.fastChanges.ActivateChanges();

        if (totalCutsceneStep.newVolumeProfile)
            CameraManager.Instance.SetVolumeProfile(totalCutsceneStep.newVolumeProfile);

        if (totalCutsceneStep.editCameraSize != 0)
            await CameraManager.Instance.CameraZoom(totalCutsceneStep.editCameraSize, true);

        foreach (ObjectState objState in totalCutsceneStep.objectsChangeState)
            objState.obj.gameObject.SetActive(objState.newState);
        foreach (ObjectSprite objectSprite in totalCutsceneStep.objectsChangeSprite)
            objectSprite.spriteRenderer.sprite = objectSprite.newSprite;
        foreach (ObjectTransform objectTransform in totalCutsceneStep.objectsChangeTransform)
            objectTransform.obj.transform.position = objectTransform.newTransform.position;
        foreach (Animations animations in totalCutsceneStep.animatorsChanges)
            animations.animator.SetBool(animations.boolName, animations.boolStatus);

        foreach (LocationsLock locationsLock in totalCutsceneStep.locksLocations)
            ManageLocation.Instance.SetLockToLocation(locationsLock.location, locationsLock.lockLocation);
        GameMenuManager.Instance.LockAnyMenu = totalCutsceneStep.lockAllMenu;

        foreach (NpcMoveToPlayer npcMoveToPlayer in totalCutsceneStep.npcMoveToPlayer)
            npcMoveToPlayer.npc.moveToPlayer = npcMoveToPlayer.move;

        foreach (HumanMove humanMove in totalCutsceneStep.humansMove)
        {
            NpcController npcController = humanMove.human.GetComponent<NpcController>();
            if (npcController)
            {
                npcController.moveToPoint = true;
                npcController.point = humanMove.pointMove;
                npcController.locationOfPointName = npcController.totalLocation;
            }
            else
                humanMove.human.GetComponent<Player>()?.MoveTo(humanMove.pointMove);
        }
    }

    private async void DelayAndNext(float delay, int totalStep)
    {
        await Task.Delay(Mathf.RoundToInt(delay * 1000));
        totalStep++;
        ActivateCutsceneStep(totalStep);
    }
}