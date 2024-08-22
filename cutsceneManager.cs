using UnityEngine;

public class cutsceneManager : MonoBehaviour
{
    public cutscene totalCutscene;
    [SerializeField] private cutscene[] cutsceneInGame = new cutscene[0];
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

    public void ActivateCutsceneStep(int step)
    {
        for (int i = 0; i < totalCutscene.steps[step].objectsChangeState.Length; i++)
            totalCutscene.steps[step].objectsChangeState[i].obj.gameObject.SetActive(totalCutscene.steps[step].objectsChangeState[i].newState);
        for (int i = 0; i < totalCutscene.steps[step].objectsChangeSprite.Length; i++)
            totalCutscene.steps[step].objectsChangeSprite[i].obj.GetComponent<SpriteRenderer>().sprite = totalCutscene.steps[step].objectsChangeSprite[i].newSprite;
        for (int i = 0; i < totalCutscene.steps[step].objectsChangeTransform.Length; i++)
            totalCutscene.steps[step].objectsChangeTransform[i].obj.transform.position = totalCutscene.steps[step].objectsChangeTransform[i].newTransform.position;
        
        if (totalCutscene.steps[step].moveToLocation != "")
            scripts.locations.ActivateLocation(totalCutscene.steps[step].moveToLocation, "0");

        if (totalCutscene.steps[step].activatedDialog != "")
            scripts.dialogsManager.ActivateDialog(totalCutscene.steps[step].activatedDialog);
        if (totalCutscene.steps[step].questStepNext)
            scripts.quests.NextStep();
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
        public string activatedDialog;
        public string moveToLocation;
        public string addNote;
        public objectState[] objectsChangeState = new objectState[0];
        public objectTransform[] objectsChangeTransform = new objectTransform[0];
        public objectSprite[] objectsChangeSprite = new objectSprite[0];
        public bool questStepNext;
    }

    public string name;
    public cutsceneStep[] steps = new cutsceneStep[0];
}