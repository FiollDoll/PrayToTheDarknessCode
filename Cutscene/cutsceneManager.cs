using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering;
using DG.Tweening;
using Cinemachine;

public class CutsceneManager : MonoBehaviour
{
    public static CutsceneManager Instance { get; private set; }
    public Cutscene totalCutscene;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private Cutscene[] allCutscene = new Cutscene[0];
    private readonly Dictionary<string, Cutscene> _allCutsceneDict = new Dictionary<string, Cutscene>();
    [SerializeField] private GameObject startViewMenu;
    [SerializeField] private bool svBlock; // dev only
    [SerializeField] private Image noViewPanel;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        foreach (Cutscene cutscene in allCutscene)
            _allCutsceneDict.Add(cutscene.name, cutscene);

        if (!svBlock) // dev only
            startViewMenu.gameObject.SetActive(true);
        StartViewMenuActivate();
    }

    private void StartViewMenuActivate(string newText = "")
    {
        if (svBlock) return;
        if (newText != "")
            startViewMenu.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = newText;
        startViewMenu.GetComponent<Animator>().Play("startGameInGame");
    }

    public void ActivateCutscene(string cutsceneName)
    {
        if (string.IsNullOrEmpty(cutsceneName)) return;
        totalCutscene = new Cutscene();
        totalCutscene = _allCutsceneDict.GetValueOrDefault(cutsceneName);
        if (totalCutscene != null)
            ActivateCutsceneStep(0);
    }

    private void StepDo(int step) // Выполнить шаг катсцены.
    {
        CutsceneStep totalCutsceneStep = totalCutscene.steps[step];
        if (totalCutsceneStep.startViewMenuActivate != "")
            StartViewMenuActivate(totalCutsceneStep.startViewMenuActivate);

        if (totalCutsceneStep.closeDialogMenu)
            DialogsManager.Instance.DialogCLose();

        totalCutsceneStep.fastChanges.ActivateChanges();

        if (totalCutsceneStep.newVolumeProfile)
            CameraManager.Instance.SetVolumeProfile(totalCutsceneStep.newVolumeProfile);

        if (totalCutsceneStep.editCameraSize != 0)
            CameraManager.Instance.CameraZoom(totalCutsceneStep.editCameraSize, true);

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
        GameMenuManager.Instance.lockAnyMenu = totalCutsceneStep.lockAllMenu;

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

    public void ActivateCutsceneStep(int step)
    {
        if (totalCutscene == null || step == -1)
            return;

        if (totalCutscene.steps[step].delayAndNext != 0)
        {
            StartCoroutine(DelayAndNext(totalCutscene.steps[step].delayAndNext, step++));
            return;
        }

        if (totalCutscene.steps[step].timeDarkStart != 0)
        {
            Sequence sequence = DOTween.Sequence();
            Tween stepSequence =
                noViewPanel.DOFade(100f, totalCutscene.steps[step].timeDarkStart).SetEase(Ease.InQuart);
            stepSequence.OnComplete(() => { StepDo(step); });
            sequence.Append(stepSequence);
            sequence.Append(noViewPanel.DOFade(0f, totalCutscene.steps[step].timeDarkEnd).SetEase(Ease.OutQuart));
            sequence.Insert(0, transform.DOScale(new Vector3(1, 1, 1), sequence.Duration()));
        }
        else
            StepDo(step);
    }

    private IEnumerator DelayAndNext(float delay, int newStep)
    {
        yield return new WaitForSeconds(delay);
        ActivateCutsceneStep(newStep);
    }
}