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

    public void StepDo(int step)
    {
        if (totalCutscene.steps[step].moveToLocation != "")
            scripts.locations.ActivateLocation(totalCutscene.steps[step].moveToLocation, "0", totalCutscene.steps[step].toLocationWithFade);

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

        for (int i = 0; i < totalCutscene.steps[step].objectsChangeState.Length; i++)
            totalCutscene.steps[step].objectsChangeState[i].obj.gameObject.SetActive(totalCutscene.steps[step].objectsChangeState[i].newState);
        for (int i = 0; i < totalCutscene.steps[step].objectsChangeSprite.Length; i++)
            totalCutscene.steps[step].objectsChangeSprite[i].obj.GetComponent<SpriteRenderer>().sprite = totalCutscene.steps[step].objectsChangeSprite[i].newSprite;
        for (int i = 0; i < totalCutscene.steps[step].objectsChangeTransform.Length; i++)
            totalCutscene.steps[step].objectsChangeTransform[i].obj.transform.position = totalCutscene.steps[step].objectsChangeTransform[i].newTransform.position;
        for (int i = 0; i < totalCutscene.steps[step].animatorsChanges.Length; i++)
            totalCutscene.steps[step].animatorsChanges[i].animator.SetBool(totalCutscene.steps[step].animatorsChanges[i].boolName, totalCutscene.steps[step].animatorsChanges[i].boolStatus);

        for (int i = 0; i < totalCutscene.steps[step].locksLocations.Length; i++)
            scripts.locations.SetLockToLocation(totalCutscene.steps[step].locksLocations[i].location, totalCutscene.steps[step].locksLocations[i].lockLocation);

        for (int i = 0; i < totalCutscene.steps[step].npcMove.Length; i++)
            GameObject.Find(totalCutscene.steps[step].npcMove[i].NPC_name).GetComponent<NPC_movement>().moveToPlayer = totalCutscene.steps[step].npcMove[i].move;

        for (int i = 0; i < totalCutscene.steps[step].addQuests.Length; i++)
            scripts.quests.ActivateQuest(totalCutscene.steps[step].addQuests[i]);
        for (int i = 0; i < totalCutscene.steps[step].addItem.Length; i++)
            scripts.inventory.AddItem(totalCutscene.steps[step].addItem[i]);

    }

    public void ActivateCutsceneStep(int step)
    {
        if (totalCutscene.steps[step].timeDarkStart != 0)
        {
            Sequence sequence = DOTween.Sequence();
            Tween stepSequence = noViewPanel.DOFade(100f, totalCutscene.steps[step].timeDarkStart).SetEase(Ease.InQuart);
            stepSequence.OnComplete(() =>
            {
                StepDo(step);
            });
            sequence.Append(stepSequence);
            sequence.Append(noViewPanel.DOFade(0f, totalCutscene.steps[step].timeDarkEnd).SetEase(Ease.OutQuart));
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
        [Header("PostProcessing")]
        public VolumeProfile newVolumeProfile;

        [Header("DoInScripts")]
        public string activatedDialog;
        public string moveToLocation;
        public string[] addItem;
        public string[] addQuests;
        public bool toLocationWithFade = true;
        public string addNote;
        public bool closeDialogMenu;
        public bool questStepNext;
        public bool lockNote, lockInventory;

        [Header("Objects")]
        public objectState[] objectsChangeState = new objectState[0];
        public objectTransform[] objectsChangeTransform = new objectTransform[0];
        public objectSprite[] objectsChangeSprite = new objectSprite[0];
        public animations[] animatorsChanges = new animations[0];
        public locationsLock[] locksLocations = new locationsLock[0];
        public NPC_moveToPlayer[] npcMove = new NPC_moveToPlayer[0];


        [Header("Camera")]
        public float editCameraSize;

        [Header("dark")]
        public float timeDarkStart;
        public float timeDarkEnd;

    }

    public string name;
    public cutsceneStep[] steps = new cutsceneStep[0];
}