using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using DG.Tweening;

public class cutsceneManager : MonoBehaviour
{
    public cutscene totalCutscene;
    [SerializeField] private GameObject playerCamera;
    [SerializeField] private cutscene[] cutsceneInGame = new cutscene[0];
    [SerializeField] private Image noViewPanel;
    [SerializeField] private allScripts scripts;

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
        for (int i = 0; i < totalCutscene.steps[step].objectsChangeState.Length; i++)
            totalCutscene.steps[step].objectsChangeState[i].obj.gameObject.SetActive(totalCutscene.steps[step].objectsChangeState[i].newState);
        for (int i = 0; i < totalCutscene.steps[step].objectsChangeSprite.Length; i++)
            totalCutscene.steps[step].objectsChangeSprite[i].obj.GetComponent<SpriteRenderer>().sprite = totalCutscene.steps[step].objectsChangeSprite[i].newSprite;
        for (int i = 0; i < totalCutscene.steps[step].objectsChangeTransform.Length; i++)
            totalCutscene.steps[step].objectsChangeTransform[i].obj.transform.position = totalCutscene.steps[step].objectsChangeTransform[i].newTransform.position;

        if (totalCutscene.steps[step].moveToLocation != "")
            scripts.locations.ActivateLocation(totalCutscene.steps[step].moveToLocation, "0", totalCutscene.steps[step].toLocationWithFade);

        if (totalCutscene.steps[step].activatedDialog != "")
            scripts.dialogsManager.ActivateDialog(totalCutscene.steps[step].activatedDialog);
        if (totalCutscene.steps[step].questStepNext)
            scripts.quests.NextStep();
        if (totalCutscene.steps[step].newVolumeProfile != null)
            playerCamera.GetComponent<Volume>().profile = totalCutscene.steps[step].newVolumeProfile;

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
    public class cutsceneStep
    {
        public string name;
        [Header("PostProcessing")]
        public VolumeProfile newVolumeProfile;
        
        [Header("DoInScripts")]
        public string activatedDialog;
        public string moveToLocation;
        public bool toLocationWithFade = true;
        public string addNote;
        public bool questStepNext;

        [Header("Objects")]
        public objectState[] objectsChangeState = new objectState[0];
        public objectTransform[] objectsChangeTransform = new objectTransform[0];
        public objectSprite[] objectsChangeSprite = new objectSprite[0];
        [Header("dark")]
        public float timeDarkStart;
        public float timeDarkEnd;
    }

    public string name;
    public cutsceneStep[] steps = new cutsceneStep[0];
}