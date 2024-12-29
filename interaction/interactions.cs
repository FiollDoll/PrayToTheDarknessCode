using System;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.Rendering;

public class Interactions : MonoBehaviour
{
    public ExtraInter enteredEI, selectedEI;
    [Header("Настройки")] [SerializeField] private LayerMask layerMaskInteract;

    [SerializeField] private Material materialOfSelected;

    public bool lockInter;
    [SerializeField] private TextMeshProUGUI interLabelText;
    public GameObject floorChangeMenu;

    private RaycastHit _selectedCollider, _enteredCollider;
    private AllScripts _scripts;
    private string _spawnName;
    private bool _locationToClick; // Определять - нужно нажать на кнопку чтобы перейти или нет

    public void Initialize()
    {
        _scripts = GameObject.Find("scripts").GetComponent<AllScripts>();
    }

    /// <summary>
    /// Метод автоматических взаимодействий, испольщзуемый в player
    /// </summary>
    /// <param name="enteredCollider"></param>
    public void ActivateInteractionAuto(RaycastHit enteredCollider)
    {
        _enteredCollider = enteredCollider;
        _locationToClick = false;
        switch (_enteredCollider.collider.tag)
        {
            case "dialog":
                _scripts.dialogsManager.ActivateDialog(_enteredCollider.collider.name);
                break;
            case "location":
                enteredEI = _enteredCollider.collider.gameObject.GetComponent<ExtraInteraction>().interactions[0];
                _spawnName = enteredEI.moveToSpawn;

                if (!_scripts.manageLocation.GetLocation(_enteredCollider.collider.name).locked)
                {
                    if (_scripts.manageLocation.GetLocation(_enteredCollider.collider.name).autoEnter &&
                        _scripts.manageLocation.totalLocation.autoEnter)
                        _scripts.manageLocation.ActivateLocation(_enteredCollider.collider.name, _spawnName);
                    else
                        _locationToClick = true;
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

    /// <summary>
    /// Метод очистки коллайдеров, в которые вошел игрок. Используется в player
    /// </summary>
    public void ClearEnteredCollider()
    {
        _spawnName = "";
        _enteredCollider = new RaycastHit();
    }

    private void SetEiOptions(ExtraInter totalInter, string colliderName)
    {
        if (totalInter != null)
        {
            _scripts.inventoryManager.AddItem(totalInter.itemNameAdd);
            if (totalInter.darkAfterUse)
                _scripts.main.ActivateNoVision(1f);
            if (totalInter.nextStep && totalInter.stageInter == _scripts.questsSystem.totalQuest.totalStep)
                _scripts.questsSystem.NextStep();
            if (totalInter.activateCutscene)
                _scripts.cutsceneManager.ActivateCutscene(colliderName);
            if (totalInter.swapPlayerVisual)
                _scripts.player.ChangeVisual(totalInter.playerVisual);
            if (totalInter.destroyAfterInter)
                Destroy(GameObject.Find(colliderName));
            if (totalInter.moveToSpawn != "")
                _scripts.manageLocation.ActivateLocation(colliderName, totalInter.moveToSpawn);
        }
    }

    /// <summary>
    /// Добавление ДОП материала на объект(оутлайн выделения)
    /// </summary>
    /// <param name="renderer"></param>
    /// <returns></returns>
    public void AddMaterial(MeshRenderer renderer)
    {
        Material[] newMaterials = new Material[renderer.materials.Length + 1];

        for (int i = 0; i < renderer.materials.Length; i++)
            newMaterials[i] = renderer.materials[i];

        newMaterials[newMaterials.Length - 1] = materialOfSelected;

        renderer.materials = newMaterials;
    }

    public void RemoveMaterial(MeshRenderer renderer)
    {
        Material[] newMaterials = new Material[renderer.materials.Length - 1];

        int index = 0;
        for (int i = 0; i < renderer.materials.Length; i++)
        {
            if (renderer.materials[i] != materialOfSelected)
            {
                newMaterials[index] = renderer.materials[i];
                index++;
            }

            renderer.materials = newMaterials;
        }
    }

    private void FixedUpdate()
    {
        enteredEI = null;
        selectedEI = null;
        if (_selectedCollider.collider != null)
            RemoveMaterial(_selectedCollider.collider.gameObject.GetComponent<MeshRenderer>());

        // Луч из мыши
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out _selectedCollider, 30f,
                layerMaskInteract) && !lockInter)
        {
            switch (_selectedCollider.collider.tag)
            {
                case "dialog":
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
                    }

                    AddMaterial(_selectedCollider.collider.gameObject.GetComponent<MeshRenderer>());
                    break;
            }
        }
    }

    private void Update()
    {
        Debug.DrawLine(transform.position, _selectedCollider.point, Color.green);

        // Перебор по коллайдерам, в которые вошли
        if (_enteredCollider.collider != null)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (!_enteredCollider.collider.CompareTag("location"))
                {
                    if (_enteredCollider.collider.gameObject
                            .GetComponent<ExtraInteraction>() != null)
                    {
                        if (enteredEI == null)
                            return;
                    }
                }

                SetEiOptions(enteredEI, _enteredCollider.collider.name);

                switch (_enteredCollider.collider.tag)
                {
                    case "location":
                        if (!_scripts.manageLocation.GetLocation(_enteredCollider.collider.name).locked)
                            _scripts.manageLocation.ActivateLocation(_enteredCollider.collider.name,
                                enteredEI.moveToSpawn);
                        break;
                    default:
                        if (_enteredCollider.collider.name == "floorChange" &&
                            (!_scripts.main.CheckAnyMenuOpen() || floorChangeMenu.gameObject.activeSelf))
                            floorChangeMenu.gameObject.SetActive(!floorChangeMenu.gameObject.activeSelf);
                        else
                        {
                            if (!_scripts.dialogsManager.dialogMenu.activeSelf)
                                _scripts.dialogsManager.ActivateDialog(_enteredCollider.collider.name);
                        }

                        break;
                }
            }
        }

        if (_selectedCollider.collider != null)
        {
            if (Input.GetMouseButton(0))
            {
                SetEiOptions(selectedEI, _selectedCollider.collider.name);

                switch (_selectedCollider.collider.tag)
                {
                    case "cutscene":
                        _scripts.cutsceneManager.ActivateCutscene(_selectedCollider.collider.name);
                        break;
                    case "item":
                        _scripts.inventoryManager.AddItem(_selectedCollider.collider.name);
                        Destroy(_selectedCollider.collider.gameObject);
                        break;
                    case "interact":
                    case "dialog":
                        _scripts.dialogsManager.ActivateDialog(_selectedCollider.collider.name);
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

        interLabelText.text = "";
        if (selectedEI != null)
            interLabelText.text = selectedEI.interLabel;
        if (enteredEI != null && _locationToClick) // Рассматривается случай только для перехода по локациям
            interLabelText.text = enteredEI.interLabel;
    }
}