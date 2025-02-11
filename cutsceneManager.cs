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
    public Cutscene totalCutscene = new Cutscene();
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private Cutscene[] allCutscene = new Cutscene[0];
    private Dictionary<string, Cutscene> _allCutsceneDict = new Dictionary<string, Cutscene>();
    [SerializeField] private GameObject startViewMenu;
    [SerializeField] private bool svBlock; // dev only
    [SerializeField] private Image noViewPanel;
    private AllScripts _scripts;

    private void Start()
    {
        _scripts = GameObject.Find("scripts").GetComponent<AllScripts>();
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
        totalCutscene = _allCutsceneDict.GetValueOrDefault(cutsceneName);
        if (totalCutscene != null)
            ActivateCutsceneStep(0);
    }

    private void StepDo(int step) // Выполнить шаг катсцены.
    {
        Cutscene.CutsceneStep totalCutsceneStep = totalCutscene.steps[step];
        if (totalCutsceneStep.startViewMenuActivate != "")
            StartViewMenuActivate(totalCutsceneStep.startViewMenuActivate);

        if (totalCutsceneStep.closeDialogMenu)
            _scripts.dialogsManager.DialogCLose();

        totalCutsceneStep.fastChanges.ActivateChanges(_scripts);

        if (totalCutsceneStep.newVolumeProfile)
            _scripts.postProcessingController.SetVolumeProfile(totalCutsceneStep.newVolumeProfile);

        if (totalCutsceneStep.editCameraSize != 0)
            virtualCamera.m_Lens.OrthographicSize =
                _scripts.main.startCameraSize + totalCutsceneStep.editCameraSize;

        foreach (Cutscene.ObjectState objState in totalCutsceneStep.objectsChangeState)
            objState.obj.gameObject.SetActive(objState.newState);
        foreach (Cutscene.ObjectSprite objectSprite in totalCutsceneStep.objectsChangeSprite)
            objectSprite.spriteRenderer.sprite = objectSprite.newSprite;
        foreach (Cutscene.ObjectTransform objectTransform in totalCutsceneStep.objectsChangeTransform)
            objectTransform.obj.transform.position = objectTransform.newTransform.position;
        foreach (Cutscene.Animations animations in totalCutsceneStep.animatorsChanges)
            animations.animator.SetBool(animations.boolName, animations.boolStatus);

        foreach (Cutscene.LocationsLock locationsLock in totalCutsceneStep.locksLocations)
            _scripts.manageLocation.SetLockToLocation(locationsLock.location, locationsLock.lockLocation);
        _scripts.main.lockAnyMenu = totalCutsceneStep.lockAllMenu;

        foreach (Cutscene.NpcMoveToPlayer npcMoveToPlayer in totalCutsceneStep.npcMoveToPlayer)
            npcMoveToPlayer.npc.moveToPlayer = npcMoveToPlayer.move;

        foreach (Cutscene.HumanMove humanMove in totalCutsceneStep.humansMove)
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
        if (totalCutscene.name == "" || step == -1)
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

[System.Serializable]
public class Cutscene
{
    [System.Serializable]
    public class LocationsLock
    {
        public string location;
        public bool lockLocation;
    }

    [System.Serializable]
    public class NpcMoveToPlayer
    {
        public NpcController npc;
        public bool move;
    }

    [System.Serializable]
    public class ObjectState
    {
        public GameObject obj;
        public bool newState;
    }

    [System.Serializable]
    public class ObjectTransform
    {
        public GameObject obj;
        public Transform newTransform;
    }

    [System.Serializable]
    public class ObjectSprite
    {
        public SpriteRenderer spriteRenderer;
        public Sprite newSprite;
    }

    [System.Serializable]
    public class Animations
    {
        public Animator animator;
        public string boolName;
        public bool boolStatus;
    }

    [System.Serializable]
    public class HumanMove
    {
        public GameObject human;
        public Transform pointMove;
    }

    [System.Serializable]
    public class CutsceneStep
    {
        public string name;

        public bool closeDialogMenu;

        [Header("MainChanges")] public FastChangesController fastChanges;

        [Header("Locks")] public bool lockAllMenu;
        public LocationsLock[] locksLocations = new LocationsLock[0];

        [Header("UltraChanges")] public ObjectState[] objectsChangeState = new ObjectState[0];
        public ObjectTransform[] objectsChangeTransform = new ObjectTransform[0];
        public ObjectSprite[] objectsChangeSprite = new ObjectSprite[0];
        public Animations[] animatorsChanges = new Animations[0];

        [Header("Other")] public string startViewMenuActivate;
        public float editCameraSize;
        public NpcMoveToPlayer[] npcMoveToPlayer = new NpcMoveToPlayer[0];
        public HumanMove[] humansMove = new HumanMove[0];
        public VolumeProfile newVolumeProfile;
        public float timeDarkStart;
        public float timeDarkEnd = 1f;
        public float delayAndNext;
    }

    public string name;
    public CutsceneStep[] steps = new CutsceneStep[0];
}