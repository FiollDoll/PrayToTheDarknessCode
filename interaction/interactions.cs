using JetBrains.Annotations;
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

    /// <summary>
    /// Можно ли активировать взаимодействие?
    /// </summary>
    /// <param name="interactable">Взаимодействие</param>
    /// <param name="mouseInteraction">Взаимодействие через мышь?</param>
    /// <returns></returns>
    public bool CanActivateInteraction(IInteractable interactable, bool mouseInteraction = false)
    {
        if (mouseInteraction)
        {
            // Ограничение по области
            float pos = Player.singleton.gameObject.transform.position.x -
                        _selectedCollider.collider.transform.position.x;

            if (pos is > 6f or < -6f)
                return false;
        }

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
        if (!Main.singleton.CanMenuOpen()) // Т.е открыто какое-либо меню
            return;
        MeshRenderer selectedColliderRenderer = _selectedCollider.collider?.GetComponent<MeshRenderer>();
        if (selectedColliderRenderer && selectedColliderRenderer.materials.Length > 1)
            Main.singleton.RemoveMaterial(selectedColliderRenderer);

        // Взаимодействия по мыши
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out _selectedCollider, 30f,
                layerMaskInteract))
        {
            if (_selectedCollider.collider.TryGetComponent(out IInteractable interactable))
            {
                if (CanActivateInteraction(interactable, true))
                {
                    SelectedInteraction = interactable;
                    if (selectedColliderRenderer) // Если 3D объект
                        Main.singleton.AddMaterial(selectedColliderRenderer);
                }
            }
        }
    }

    private void Update()
    {
        if (!Main.singleton.CanMenuOpen()) // Т.е открыто какое-либо меню
            return;
        Debug.DrawLine(Input.mousePosition, _selectedCollider.point, Color.green);

        // Перебор по коллайдерам, в которые вошёл игрок, и в которых !autoUse
        if (!lockInter)
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