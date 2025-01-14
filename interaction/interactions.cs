using UnityEngine;
using TMPro;

public class Interactions : MonoBehaviour
{
    public IInteractable EnteredInteraction, SelectedInteraction;
    [Header("Настройки")] [SerializeField] private LayerMask layerMaskInteract;

    public bool lockInter;
    [SerializeField] private TextMeshProUGUI interLabelText;
    public GameObject floorChangeMenu;

    private RaycastHit _selectedCollider;
    private AllScripts _scripts;

    public void Initialize()
    {
        _scripts = GameObject.Find("scripts").GetComponent<AllScripts>();
    }

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
        EnteredInteraction = null;
        SelectedInteraction = null;
        if (_selectedCollider.collider != null && _selectedCollider.collider.GetComponent<MeshRenderer>())
            _scripts.main.RemoveMaterial(_selectedCollider.collider.GetComponent<MeshRenderer>());

        // Взаимодействия по мыши
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out _selectedCollider, 30f,
                layerMaskInteract) && !lockInter)
        {
            if (_selectedCollider.collider.TryGetComponent(out IInteractable interactable))
            {
                SelectedInteraction = interactable;
                if (CheckActiveInteraction(interactable) && _selectedCollider.collider.GetComponent<MeshRenderer>())
                    _scripts.main.AddMaterial(_selectedCollider.collider.GetComponent<MeshRenderer>());
            }
        }
    }

    private void Update()
    {
        Debug.DrawLine(transform.position, _selectedCollider.point, Color.green);

        // Перебор по коллайдерам, в которые вошёл игрок, и в которых !autoUse
        if (Input.GetKeyDown(KeyCode.E))
            EnteredInteraction?.DoInteraction();

        if (Input.GetMouseButton(0))
            SelectedInteraction?.DoInteraction();

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (!_scripts.main.CheckAnyMenuOpen())
                _scripts.player.playerMenu.gameObject.SetActive(true);
            else
                _scripts.player.playerMenu.gameObject.SetActive(false);
            _scripts.player.canMove = !_scripts.player.playerMenu.activeSelf;
        }

        interLabelText.text = "";
        if (SelectedInteraction != null)
            interLabelText.text = SelectedInteraction.interLabel;
        if (EnteredInteraction != null) // Рассматривается случай только для перехода по локациям
            interLabelText.text = EnteredInteraction.interLabel;
    }
}