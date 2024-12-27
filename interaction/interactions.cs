using System.Linq;
using UnityEngine;
using TMPro;

public class Interactions : MonoBehaviour
{
    public ExtraInter selectedEI;
    [Header("Настройки")]
    [SerializeField] private LayerMask layerMaskInteract;
    public bool lockInter;
    [SerializeField] private TextMeshProUGUI interLabelText;
    public GameObject floorChangeMenu;
    
    private RaycastHit _selectedCollider, _enteredCollider;
    private AllScripts _scripts;
    private string _totalColliderName, _totalColliderMode, _spawnName;

    private void Start()
    {
        _scripts = GameObject.Find("scripts").GetComponent<AllScripts>();
        _scripts.interactions = this;
    }

    /// <summary>
    /// Метод автоматических взаимодействий, испольщзуемый в player
    /// </summary>
    /// <param name="enteredCollider"></param>
    public void ActivateInteractionAuto(RaycastHit enteredCollider)
    {
        _enteredCollider = enteredCollider;
        switch (_enteredCollider.collider.tag)
        {
            case "dialog":
                _scripts.dialogsManager.ActivateDialog(_enteredCollider.collider.name);
                break;
            case "location":
                _totalColliderName = _enteredCollider.collider.name;
                _totalColliderMode = _enteredCollider.collider.tag;

                selectedEI = _enteredCollider.collider.gameObject.GetComponent<ExtraInteraction>().interactions[0];
                _spawnName = selectedEI.moveToSpawn;

                if (!_scripts.manageLocation.GetLocation(_totalColliderName).locked)
                {
                    if (_scripts.manageLocation.GetLocation(_totalColliderName).autoEnter &&
                        _scripts.manageLocation.totalLocation.autoEnter)
                        _scripts.manageLocation.ActivateLocation(_totalColliderName, _spawnName);
                    else
                        interLabelText.text = selectedEI.interLabel;
                }

                break;
            case "cutsceneAuto":
                if (_enteredCollider.collider.gameObject.GetComponent<ExtraInteraction>() != null)
                {
                    ExtraInter totalEI = _enteredCollider.collider.gameObject.GetComponent<ExtraInteraction>()
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

                _scripts.cutsceneManager.ActivateCutscene(_enteredCollider.collider.gameObject.name);
                break;
        }
    }

    private void FixedUpdate()
    {
        selectedEI = null;
        _totalColliderName = "";
        _totalColliderMode = "";
        _spawnName = "";

        interLabelText.text = "";

        // TODO: добавить взаимодействие с интеракциями через нажатие мыши(сейчас работает через E, либо не работает)
        // Луч из мыши
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out _selectedCollider, 30f, layerMaskInteract) && !lockInter)
        {
            switch (_selectedCollider.collider.tag)
            {
                case "dialog":
                    _scripts.dialogsManager.ActivateDialog(_selectedCollider.collider.name);
                    break;
                case "cutscene":
                case "interact":
                case "item":
                    if (_selectedCollider.collider.gameObject.GetComponent<ExtraInteraction>() != null)
                    {
                        ExtraInteraction EI = _selectedCollider.collider.gameObject.GetComponent<ExtraInteraction>();
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
                            _selectedCollider.collider.gameObject.name = totalEI.interName; // Сделано плохо
                            break;
                        }

                        if (selectedEI == null)
                            return;
                    }

                    _totalColliderName = _selectedCollider.collider.gameObject.name;
                    _totalColliderMode = _selectedCollider.collider.gameObject.tag;

                    break;
            }
        }
    }

    private void Update()
    {
        Debug.DrawLine(transform.position, _selectedCollider.point, Color.green);

        if (_enteredCollider.collider != null)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                _totalColliderName = _enteredCollider.collider.gameObject.name;
                _totalColliderMode = GameObject.Find(_totalColliderName).tag;
                if (_totalColliderMode != "location")
                {
                    if (_enteredCollider.collider.gameObject
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
                        _scripts.cutsceneManager.ActivateCutscene(_totalColliderName);
                    if (selectedEI.swapPlayerVisual)
                        _scripts.player.ChangeVisual(selectedEI.playerVisual);
                    if (selectedEI.destroyAfterInter)
                        Destroy(GameObject.Find(_totalColliderName));
                    if (selectedEI.moveToSpawn != "")
                        _scripts.manageLocation.ActivateLocation(_totalColliderName, selectedEI.moveToSpawn);
                }

                switch (_totalColliderMode)
                {
                    case "item":
                        _scripts.inventory.AddItem(_totalColliderName);
                        Destroy(GameObject.Find(_totalColliderName));
                        break;
                    case "location":
                        if (!_scripts.manageLocation.GetLocation(_totalColliderName).locked)
                            _scripts.manageLocation.ActivateLocation(_totalColliderName, selectedEI.moveToSpawn);
                        break;
                    default:
                        if (_totalColliderName == "floorChange" &&
                            (!_scripts.main.CheckAnyMenuOpen() || floorChangeMenu.gameObject.activeSelf))
                            floorChangeMenu.gameObject.SetActive(!floorChangeMenu.gameObject.activeSelf);
                        else
                        {
                            if (!_scripts.dialogsManager.dialogMenu.activeSelf)
                                _scripts.dialogsManager.ActivateDialog(_totalColliderName);
                        }
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