using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using DG.Tweening;
using Cinemachine;

public class cutsceneManager : MonoBehaviour
{
    public cutscene totalCutscene;
    [SerializeField] private GameObject playerCamera, virtualCamera;
    [SerializeField] private cutscene[] cutsceneInGame = new cutscene[0];
    [SerializeField] private Image noViewPanel;
    [SerializeField] private allScripts scripts;
    private Volume volume;
    private float startCameraSize;

    private void Start()
    {
        volume = playerCamera.GetComponent<Volume>();
        startCameraSize = virtualCamera.GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize;
    }

    public void ActivateCutscene(string name)
    {
        foreach (cutscene Cutscene in cutsceneInGame)
        {
            if (Cutscene.name == name)
            {
                totalCutscene = Cutscene;
                ActivateCutsceneStep(0);
                break;
            }
        }
    }

    public void StepDo(int step) // Выполнить шаг катсцены.
    {
        if (totalCutscene.steps[step].moveToLocation != "")
            scripts.locations.ActivateLocation(totalCutscene.steps[step].moveToLocation, totalCutscene.steps[step].moveToLocationSpawn, totalCutscene.steps[step].toLocationWithFade);

        if (totalCutscene.steps[step].closeDialogMenu)
            scripts.dialogsManager.DialogCLose();

        if (totalCutscene.steps[step].activatedDialog != "")
            scripts.dialogsManager.ActivateDialog(totalCutscene.steps[step].activatedDialog);

        if (totalCutscene.steps[step].addNote != "")
            scripts.notebook.AddNote(totalCutscene.steps[step].addNote);

        if (totalCutscene.steps[step].questStepNext)
            scripts.quests.NextStep();

        if (totalCutscene.steps[step].newVolumeProfile != null)
            volume.profile = totalCutscene.steps[step].newVolumeProfile;

        if (totalCutscene.steps[step].editCameraSize != 0)
            virtualCamera.GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize = startCameraSize + totalCutscene.steps[step].editCameraSize;

        foreach (cutscene.objectState objState in totalCutscene.steps[step].objectsChangeState)
            objState.obj.gameObject.SetActive(objState.newState);
        foreach (cutscene.objectSprite objectSprite in totalCutscene.steps[step].objectsChangeSprite)
            objectSprite.obj.GetComponent<SpriteRenderer>().sprite = objectSprite.newSprite;
        foreach (cutscene.objectTransform objectTransform in totalCutscene.steps[step].objectsChangeTransform)
            objectTransform.obj.transform.position = objectTransform.newTransform.position;
        foreach (cutscene.animations animations in totalCutscene.steps[step].animatorsChanges)
            animations.animator.SetBool(animations.boolName, animations.boolStatus);

        foreach (cutscene.locationsLock locationsLock in totalCutscene.steps[step].locksLocations)
            scripts.locations.SetLockToLocation(locationsLock.location, locationsLock.lockLocation);

        foreach (cutscene.NPC_moveToPlayer nPC_MoveToPlayer in totalCutscene.steps[step].npcMove)
            GameObject.Find(nPC_MoveToPlayer.NPC_name).GetComponent<NPC_movement>().moveToPlayer = nPC_MoveToPlayer.move;

        foreach (string quest in totalCutscene.steps[step].addQuests)
            scripts.quests.ActivateQuest(quest);
        foreach (string item in totalCutscene.steps[step].addItem)
            scripts.inventory.AddItem(item);

    }

    public void ActivateCutsceneStep(int step)
    {
        if (step == -1)
            return;
            
        if (totalCutscene.steps[step].timeDarkStart != 0)
        {
            Sequence sequence = DOTween.Sequence();
            Tween stepSequence = noViewPanel.DOFade(100f, totalCutscene.steps[step].timeDarkStart).SetEase(Ease.InQuart);
            stepSequence.OnComplete(() =>
            {
                StepDo(step);
            });
            sequence.Append(stepSequence);
            sequence.Append(noViewPanel.DOFade(0f, 0f).SetEase(Ease.OutQuart));
            sequence.Insert(0, transform.DOScale(new Vector3(1, 1, 1), sequence.Duration()));
        }
        else
            StepDo(step);
    }
}

[System.Serializable]
public class cutscene
{
    [System.Serializable]
    public class locationsLock
    {
        public string location;
        public bool lockLocation;
    }
    [System.Serializable]
    public class NPC_moveToPlayer
    {
        public string NPC_name;
        public bool move;
    }
    [System.Serializable]
    public class objectState
    {
        public GameObject obj;
        public bool newState;
    }
    [System.Serializable]
    public class objectTransform
    {
        public GameObject obj;
        public Transform newTransform;
    }

    [System.Serializable]
    public class objectSprite
    {
        public GameObject obj;
        public Sprite newSprite;
    }

    [System.Serializable]
    public class animations
    {
        public Animator animator;
        public string boolName;
        public bool boolStatus;
    }

    [System.Serializable]
    public class cutsceneStep
    {
        public string name;

        [Header("-DoInScripts")]
        public bool questStepNext;

        [Header("-Dialogs")]
        public string activatedDialog;
        public bool closeDialogMenu;

        [Header("-AddAnything")]
        public string[] addItem;
        public string[] addQuests;
        public string addNote;


        [Header("-Locations")]
        public string moveToLocation;
        public string moveToLocationSpawn;
        public bool toLocationWithFade = true;


        [Header("-Locks")]
        public bool lockNote;
        public bool lockInventory;
        public locationsLock[] locksLocations = new locationsLock[0];


        [Header("-UltraChanges")]
        public objectState[] objectsChangeState = new objectState[0];
        public objectTransform[] objectsChangeTransform = new objectTransform[0];
        public objectSprite[] objectsChangeSprite = new objectSprite[0];
        public animations[] animatorsChanges = new animations[0];


        [Header("-Other")]
        public NPC_moveToPlayer[] npcMove = new NPC_moveToPlayer[0];
        public float editCameraSize;
        public VolumeProfile newVolumeProfile;
        public float timeDarkStart;
    }

    public string name;
    public cutsceneStep[] steps = new cutsceneStep[0];
}