using UnityEngine;
using TMPro;

public class Interactions : MonoBehaviour
{
    public static Interactions singleton { get; private set; }
    public IInteractable EnteredInteraction, SelectedInteraction;
    [Header("Настройки")] [SerializeField] private LayerMask layerMaskInteract;

    public bool lockInter;
    [SerializeField] private TextMeshProUGUI interLabelText;
    public GameObject floorChangeMenu;

    private RaycastHit _selectedCollider;

    private void Awake()
    {
        singleton = this;
    }

    public bool CheckActiveInteraction(IInteractable interactable)
    {
        // Общая для всех проверка: можно ли использовать?
        if (QuestsSystem.singleton.totalQuest != null)
        {
            // Проверка: пора по квесту?
            if (interactable.nameQuestRequired != QuestsSystem.singleton.totalQuest.nameInGame)
            {
                if (interactable.nameQuestRequired != "")
                    return false;
            }

            // Проверка: пора по стадии квеста?
            if (interactable.stageInter != QuestsSystem.singleton.totalQuest.totalStep &&
                interactable.nameQuestRequired != "")
                return false;
        }

        return true;
    }

    private void FixedUpdate()
    {
        EnteredInteraction = null;
        SelectedInteraction = null;
        MeshRenderer selectedColliderRenderer = _selectedCollider.collider?.GetComponent<MeshRenderer>();
        if (selectedColliderRenderer && selectedColliderRenderer.materials.Length > 1)
            Main.singleton.RemoveMaterial(selectedColliderRenderer);

        // Взаимодействия по мыши
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out _selectedCollider, 30f,
                layerMaskInteract))
        {
            if (_selectedCollider.collider.TryGetComponent(out IInteractable interactable))
            {
                SelectedInteraction = interactable;
                if (CheckActiveInteraction(interactable) && selectedColliderRenderer)
                    Main.singleton.AddMaterial(selectedColliderRenderer);
            }
        }
    }

    private void Update()
    {
        Debug.DrawLine(transform.position, _selectedCollider.point, Color.green);

        // Перебор по коллайдерам, в которые вошёл игрок, и в которых !autoUse
        if(!lockInter)
        {
            if (Input.GetKeyDown(KeyCode.E))
                EnteredInteraction?.DoInteraction();

            if (Input.GetMouseButtonDown(0))
                SelectedInteraction?.DoInteraction();
        }

        interLabelText.text = "";
        if (SelectedInteraction != null)
            interLabelText.text = SelectedInteraction.interLabel;
        if (EnteredInteraction != null) // Рассматривается случай только для перехода по локациям
            interLabelText.text = EnteredInteraction.interLabel;
    }
}