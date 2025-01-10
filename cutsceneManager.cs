using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering;
using DG.Tweening;
using Cinemachine;

public class CutsceneManager : MonoBehaviour
{
    public Cutscene totalCutscene;
    [SerializeField] private GameObject virtualCamera;
    [SerializeField] private Cutscene[] cutsceneInGame = new Cutscene[0];
    [SerializeField] private GameObject startViewMenu;
    [SerializeField] private bool svBlock; // dev only
    [SerializeField] private Image noViewPanel;
    private AllScripts _scripts;

    public void Initialize()
    {
        _scripts = GameObject.Find("scripts").GetComponent<AllScripts>();
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
        foreach (Cutscene cutscene in cutsceneInGame)
        {
            if (cutscene.name == cutsceneName)
            {
                totalCutscene = cutscene;
                ActivateCutsceneStep(0);
                break;
            }
        }
    }

    private void StepDo(int step) // Выполнить шаг катсцены.
    {
        if (totalCutscene.steps[step].startViewMenuActivate != "")
            StartViewMenuActivate(totalCutscene.steps[step].startViewMenuActivate);
        _scripts.player.changeSpeed = totalCutscene.steps[step].editSpeed;
        if (totalCutscene.steps[step].changeVisualPlayer != -1)
            _scripts.player.ChangeVisual(totalCutscene.steps[step].changeVisualPlayer);
        if (totalCutscene.steps[step].moveToLocation != "")
            _scripts.manageLocation.ActivateLocation(totalCutscene.steps[step].moveToLocation,
                totalCutscene.steps[step].moveToLocationSpawn, totalCutscene.steps[step].toLocationWithFade);

        if (totalCutscene.steps[step].closeDialogMenu)
            _scripts.dialogsManager.DialogCLose();

        if (totalCutscene.steps[step].activatedDialog != "")
            _scripts.dialogsManager.ActivateDialog(totalCutscene.steps[step].activatedDialog);

        if (totalCutscene.steps[step].addNote != "")
            _scripts.notebook.AddNote(totalCutscene.steps[step].addNote);

        if (totalCutscene.steps[step].questStepNext)
            _scripts.questsSystem.NextStep();

        if (totalCutscene.steps[step].newVolumeProfile != null)
            _scripts.postProcessingController.SetVolumeProfile(totalCutscene.steps[step].newVolumeProfile);

        if (totalCutscene.steps[step].editCameraSize != 0)
            virtualCamera.GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize =
                _scripts.main.startCameraSize + totalCutscene.steps[step].editCameraSize;

        foreach (Cutscene.ObjectState objState in totalCutscene.steps[step].objectsChangeState)
            objState.obj.gameObject.SetActive(objState.newState);
        foreach (Cutscene.ObjectSprite objectSprite in totalCutscene.steps[step].objectsChangeSprite)
            objectSprite.spriteRenderer.sprite = objectSprite.newSprite;
        foreach (Cutscene.ObjectTransform objectTransform in totalCutscene.steps[step].objectsChangeTransform)
            objectTransform.obj.transform.position = objectTransform.newTransform.position;
        foreach (Cutscene.Animations animations in totalCutscene.steps[step].animatorsChanges)
            animations.animator.SetBool(animations.boolName, animations.boolStatus);

        foreach (Cutscene.LocationsLock locationsLock in totalCutscene.steps[step].locksLocations)
            _scripts.manageLocation.SetLockToLocation(locationsLock.location, locationsLock.lockLocation);
        _scripts.main.lockAnyMenu = totalCutscene.steps[step].lockAllMenu;

        foreach (Cutscene.NpcMoveToPlayer npcMoveToPlayer in totalCutscene.steps[step].npcMoveToPlayer)
            npcMoveToPlayer.npc.moveToPlayer = npcMoveToPlayer.move;

        foreach (Cutscene.HumanMove humanMove in totalCutscene.steps[step].humansMove)
        {
            NPC_movement npcMovement = humanMove.human.GetComponent<NPC_movement>();
            if (npcMovement)
            {
                npcMovement.moveToPoint = true;
                npcMovement.point = humanMove.pointMove;
                npcMovement.locationOfPointName = npcMovement.totalLocation;
            }
            else
                humanMove.human.GetComponent<Player>()?.MoveTo(humanMove.pointMove);
        }

        foreach (string quest in totalCutscene.steps[step].addQuests)
            _scripts.questsSystem.ActivateQuest(quest, true);
        foreach (string item in totalCutscene.steps[step].addItem)
            _scripts.inventoryManager.AddItem(item);
    }

    public void ActivateCutsceneStep(int step)
    {
        if (step == -1)
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
        public NPC_movement npc;
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

        [Header("-DoInScripts")] public bool questStepNext;
        public float editSpeed;
        public int changeVisualPlayer = -1;

        [Header("-Dialogs")] public string activatedDialog;
        public bool closeDialogMenu;

        [Header("-AddAnything")] public string[] addItem;
        public string[] addQuests;
        public string addNote;


        [Header("-Locations")] public string moveToLocation;
        public string moveToLocationSpawn;
        public bool toLocationWithFade = true;


        [Header("-Locks")] public bool lockAllMenu;
        public LocationsLock[] locksLocations = new LocationsLock[0];


        [Header("-UltraChanges")] public ObjectState[] objectsChangeState = new ObjectState[0];
        public ObjectTransform[] objectsChangeTransform = new ObjectTransform[0];
        public ObjectSprite[] objectsChangeSprite = new ObjectSprite[0];
        public Animations[] animatorsChanges = new Animations[0];

        [Header("-Other")] public string startViewMenuActivate;
        public NpcMoveToPlayer[] npcMoveToPlayer = new NpcMoveToPlayer[0];
        public HumanMove[] humansMove = new HumanMove[0];
        public float editCameraSize;
        public VolumeProfile newVolumeProfile;
        public float timeDarkStart;
        public float timeDarkEnd = 1f;
        public float delayAndNext;
    }

    public string name;
    public CutsceneStep[] steps = new CutsceneStep[0];
}