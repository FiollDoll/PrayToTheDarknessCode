using System.Linq;
using UnityEngine;
using TMPro;

public class Interactions : MonoBehaviour
{
    public bool lockInter;
    [SerializeField] private LayerMask layerMaskInteract;
    [SerializeField] private Transform rayStart;
    private RaycastHit2D[] enteredColliders = new RaycastHit2D[0];
    public GameObject floorChangeMenu;
    [SerializeField] private int selectedColliderId;
    [SerializeField] private TextMeshProUGUI interLabelText;
    private AllScripts _scripts;
    private string totalColliderName, totalColliderMode, spawnName;
    public ExtraInter selectedEI = null;

    private void Start()
    {
        _scripts = GameObject.Find("scripts").GetComponent<AllScripts>();
        _scripts.interactions = this;
    }

    /// <summary>
    /// Обновление UI меню, где показываются возможные условия
    /// </summary>
    /// <param name="idx"></param>
    /// <param name="label"></param>
    private void UpdateIntersMenu(int idx, string label)
    {
        if (selectedColliderId == idx)
            interLabelText.text += ("> " + label + "\n");
        else
            interLabelText.text += (label + "\n");
    }

    /// <summary>
    /// Выбор взаимодействия
    /// </summary>
    /// <param name="mode"></param>
    private void ChoiceInter(int mode)
    {
        if (enteredColliders.Length != selectedColliderId + mode && selectedColliderId + mode >= 0)
            selectedColliderId += mode;
    }

    private void FixedUpdate()
    {
        selectedEI = null;
        totalColliderName = "";
        totalColliderMode = "";
        spawnName = "";

        enteredColliders = Physics2D.RaycastAll(rayStart.position, Vector2.up, 6.5f, layerMaskInteract);
        interLabelText.text = "";
        if (enteredColliders.Length > 0 && !lockInter)
        {
            int idx = 0;
            foreach (RaycastHit2D hit in enteredColliders)
            {
                switch (hit.collider.tag)
                {
                    case "dialog":
                        _scripts.dialogsManager.ActivateDialog(hit.collider.name);
                        break;
                    case "location":
                        totalColliderName = hit.collider.name;
                        totalColliderMode = hit.collider.tag;

                        selectedEI = hit.collider.gameObject.GetComponent<ExtraInteraction>().interactions[0];
                        spawnName = selectedEI.moveToSpawn;

                        if (!_scripts.manageLocation.GetLocation(totalColliderName).locked)
                        {
                            if (_scripts.manageLocation.GetLocation(totalColliderName).autoEnter &&
                                _scripts.manageLocation.totalLocation.autoEnter)
                                _scripts.manageLocation.ActivateLocation(totalColliderName, spawnName);
                            else
                                UpdateIntersMenu(idx, selectedEI.interLabel);
                        }

                        break;
                    case "cutscene":
                    case "interact":
                    case "item":
                        if (hit.collider.gameObject.GetComponent<ExtraInteraction>() != null)
                        {
                            ExtraInteraction EI = hit.collider.gameObject.GetComponent<ExtraInteraction>();
                            for (int i = 0; i < EI.interactions.Length; i++)
                            {
                                ExtraInter totalEI = EI.interactions[i];
                                if (_scripts.questsSystem.totalQuest != null)
                                {
                                    if (totalEI.nameQuestRequired != _scripts.questsSystem.totalQuest.nameInGame)
                                    {
                                        if (totalEI.nameQuestRequired != "")
                                            continue;
                                    }

                                    if (totalEI.stageInter != _scripts.questsSystem.totalQuest.totalStep &&
                                        totalEI.nameQuestRequired != "")
                                        continue;
                                }

                                selectedEI = EI.interactions[i];
                                hit.collider.gameObject.name = totalEI.interName; // Сделано плохо
                                UpdateIntersMenu(idx, selectedEI.interLabel);
                                break;
                            }

                            if (selectedEI == null)
                                return;
                        }

                        totalColliderName = hit.collider.gameObject.name;
                        totalColliderMode = hit.collider.gameObject.tag;

                        break;
                    case "cutsceneAuto":
                        if (hit.collider.gameObject.GetComponent<ExtraInteraction>() != null)
                        {
                            ExtraInter totalEI = hit.collider.gameObject.GetComponent<ExtraInteraction>()
                                .interactions[0];
                            if (totalEI.nameQuestRequired != _scripts.questsSystem.totalQuest.nameInGame)
                            {
                                if (totalEI.nameQuestRequired != "")
                                    break;
                            }

                            if (totalEI.stageInter != _scripts.questsSystem.totalQuest.totalStep &&
                                totalEI.nameQuestRequired != "")
                                break;
                        }

                        _scripts.cutsceneManager.ActivateCutscene(hit.collider.gameObject.name);
                        break;
                }

                if (hit.collider.name != "Player" && hit.collider.name != "Player")
                    idx++;
                Debug.DrawLine(transform.position, hit.point, Color.red);
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
            ChoiceInter(1);
        if (Input.GetKeyDown(KeyCode.UpArrow))
            ChoiceInter(-1);

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (enteredColliders.Length > 0)
            {
                totalColliderName = enteredColliders.ElementAt(selectedColliderId).collider.gameObject.name;
                totalColliderMode = GameObject.Find(totalColliderName).tag;
                if (totalColliderMode != "location")
                {
                    if (enteredColliders.ElementAt(selectedColliderId).collider.gameObject
                            .GetComponent<ExtraInteraction>() != null)
                    {
                        if (selectedEI == null)
                            return;
                    }
                }

                if (selectedEI != null)
                {
                    _scripts.inventory.AddItem(selectedEI.itemNameAdd);
                    if (selectedEI.darkAfterUse)
                        _scripts.main.ActivateNoVision(1f);
                    if (selectedEI.nextStep && selectedEI.stageInter == _scripts.questsSystem.totalQuest.totalStep)
                        _scripts.questsSystem.NextStep();
                    if (selectedEI.activateCutscene)
                        _scripts.cutsceneManager.ActivateCutscene(totalColliderName);
                    if (selectedEI.swapPlayerVisual)
                        _scripts.player.ChangeVisual(selectedEI.playerVisual);
                    if (selectedEI.destroyAfterInter)
                        Destroy(GameObject.Find(totalColliderName));
                    if (selectedEI.moveToSpawn != "")
                        _scripts.manageLocation.ActivateLocation(totalColliderName, selectedEI.moveToSpawn);
                }

                switch (totalColliderMode)
                {
                    case "item":
                        _scripts.inventory.AddItem(totalColliderName);
                        Destroy(GameObject.Find(totalColliderName));
                        break;
                    case "location":
                        if (!_scripts.manageLocation.GetLocation(totalColliderName).locked)
                            _scripts.manageLocation.ActivateLocation(totalColliderName, selectedEI.moveToSpawn);
                        break;
                    default:
                        if (totalColliderName == "floorChange" &&
                            (!_scripts.main.CheckAnyMenuOpen() || floorChangeMenu.gameObject.activeSelf))
                            floorChangeMenu.gameObject.SetActive(!floorChangeMenu.gameObject.activeSelf);
                        else
                        {
                            if (!_scripts.dialogsManager.dialogMenu.activeSelf)
                                _scripts.dialogsManager.ActivateDialog(totalColliderName);
                        }

                        // TODO: в интеракциях, где можно много раз - это мешает.
                        //enteredColliders.Remove(enteredColliders.ElementAt(selectedColliderId).Key);
                        break;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (!_scripts.main.CheckAnyMenuOpen())
                _scripts.player.playerMenu.gameObject.SetActive(true);
            else
                _scripts.player.playerMenu.gameObject.SetActive(false);
            _scripts.player.canMove = !_scripts.player.playerMenu.activeSelf;
        }
    }
}