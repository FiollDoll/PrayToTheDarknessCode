using System;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.Rendering;

public class Interactions : MonoBehaviour
{
    public IInteractable enteredInteraction, selectedInteraction;
    [Header("Настройки")] [SerializeField] private LayerMask layerMaskInteract;

    public bool lockInter;
    [SerializeField] private TextMeshProUGUI interLabelText;
    public GameObject floorChangeMenu;

    private RaycastHit _selectedCollider, _enteredCollider;
    private AllScripts _scripts;

    public void Initialize()
    {
        _scripts = GameObject.Find("scripts").GetComponent<AllScripts>();
    }

    /// <summary>
    /// Метод очистки коллайдеров, в которые вошел игрок. Используется в player
    /// </summary>
    public void ClearEnteredCollider() => _enteredCollider = new RaycastHit();


    public bool CheckActiveInteraction(IInteractable interactable)
    {
        // Общая для всех проверка: можно ли использовать?
        if (_scripts.questsSystem.totalQuest != null)
        {
            // Проверка: пора по квесту?
            if (interactable.nameQuestRequired != _scripts.questsSystem.totalQuest.nameInGame)
            {
                if (interactable.nameQuestRequired != "")
                    return false;
            }

            // Проверка: пора по стадии квеста?
            if (interactable.stageInter != _scripts.questsSystem.totalQuest.totalStep &&
                interactable.nameQuestRequired != "")
                return false;
        }

        return true;
    }

    private void FixedUpdate()
    {
        enteredInteraction = null;
        selectedInteraction = null;
        if (_selectedCollider.collider != null)
            _scripts.main.RemoveMaterial(_selectedCollider.collider.gameObject.GetComponent<MeshRenderer>());

        // Взаимодействия по мыши
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out _selectedCollider, 30f,
                layerMaskInteract) && !lockInter)
        {
            if (_selectedCollider.collider.TryGetComponent(out IInteractable interactable))
            {
                selectedInteraction = interactable;
                if (CheckActiveInteraction(interactable))
                    _scripts.main.AddMaterial(_selectedCollider.collider.gameObject.GetComponent<MeshRenderer>());
            }
        }
    }

    private void Update()
    {
        Debug.DrawLine(transform.position, _selectedCollider.point, Color.green);

        // Перебор по коллайдерам, в которые вошёл игрок, и в которых !autoUse
        if (_enteredCollider.collider != null)
        {
            if (Input.GetKeyDown(KeyCode.E))
                enteredInteraction.DoInteraction();
        }

        if (_selectedCollider.collider != null)
        {
            if (Input.GetMouseButton(0))
                selectedInteraction.DoInteraction();
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
        if (selectedInteraction != null)
            interLabelText.text = selectedInteraction.interLabel;
        if (enteredInteraction != null) // Рассматривается случай только для перехода по локациям
            interLabelText.text = enteredInteraction.interLabel;
    }
}