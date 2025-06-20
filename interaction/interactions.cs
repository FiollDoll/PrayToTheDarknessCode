using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Interactions : MonoBehaviour
{
    public static Interactions Instance { get; private set; }
    private List<IInteractable> EnteredInteractions = new List<IInteractable>();
    public bool lockInter;

    [Header("Preferences")] [SerializeField]
    private Transform rayStart;

    [SerializeField] private LayerMask layerMaskInteract;
    [SerializeField] private TextMeshProUGUI interLabelText;

    private void Awake() => Instance = this;

    /// <summary>
    /// Можно ли активировать взаимодействие?
    /// </summary>
    /// <param name="interactable">Взаимодействие</param>
    /// <param name="mouseInteraction">Взаимодействие через мышь?</param>
    /// <returns></returns>
    private bool CanActivateInteraction(IInteractable interactable)
    {
        if (!interactable.CanInteractByQuest())
            return false;

        return true;
    }

    private void Update()
    {
        if (!GameMenuManager.Instance.CanMenuOpen()) // Т.е открыто какое-либо меню
            return;

        var newColliders = Physics.OverlapSphere(rayStart.position, 5f, layerMaskInteract);

        EnteredInteractions = new List<IInteractable>();
        foreach (Collider col in newColliders)
        {
            if (EnteredInteractions.Count >= 3) continue; // Ограничение по возможным взаимодействиям
            if (col.TryGetComponent(out IInteractable interactable))
            {
                EnteredInteractions.Add(interactable);

                if (CanActivateInteraction(interactable) && interactable.autoUse)
                    interactable.DoInteraction();
            }
        }

        // Перебор по коллайдерам, в которые вошёл игрок, и в которых !autoUse
        if (!lockInter)
        {
            if (Input.GetKeyDown(KeyCode.E))
                EnteredInteractions[0]?.DoInteraction();
            if (Input.GetKeyDown(KeyCode.F))
                EnteredInteractions[1]?.DoInteraction();
            if (Input.GetKeyDown(KeyCode.G))
                EnteredInteractions[2]?.DoInteraction();
        }

        interLabelText.text = "";
        if (EnteredInteractions.Count >= 1)
            interLabelText.text += "(E)" + EnteredInteractions[0].interLabel + "\n";
        if (EnteredInteractions.Count >= 2)
            interLabelText.text += "(F)" + EnteredInteractions[1].interLabel + "\n";
        if (EnteredInteractions.Count == 3)
            interLabelText.text += "(G)" + EnteredInteractions[2].interLabel + "\n";
    }
}