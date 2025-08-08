using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class Interactions : MonoBehaviour
{
    public static Interactions Instance { get; private set; }
    private List<IInteractable> EnteredInteractions = new List<IInteractable>();
    public bool lockInter;

    [Header("Preferences")]
    [SerializeField]
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

    private void InteractButton(int slot, InputAction.CallbackContext context)
    {
        try
        {
            if (!lockInter && EnteredInteractions.Count >= slot + 1 && context.action.WasPerformedThisFrame())
            {
                EnteredInteractions[slot].DoInteraction();
                PlayerStats.Hunger += 0.001f + (PlayerStats.Addiction * 0.001f);
            }
        }
        catch (System.Exception ex)
        {
            Debug.Log("Interact button slot " + slot + " error: " + ex);
        }
    }

    public void OnInteractFirst(InputAction.CallbackContext context) => InteractButton(0, context);

    public void OnInteractSecond(InputAction.CallbackContext context) => InteractButton(1, context);

    public void OnInteractThird(InputAction.CallbackContext context) => InteractButton(2, context);

    private void FixedUpdate()
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

                if (CanActivateInteraction(interactable) && interactable.AutoUse)
                    interactable.DoInteraction();
            }
        }

        interLabelText.text = "";
        if (EnteredInteractions.Count >= 1)
            interLabelText.text += "(E)" + EnteredInteractions[0].InterLabel + "\n";
        if (EnteredInteractions.Count >= 2)
            interLabelText.text += "(F)" + EnteredInteractions[1].InterLabel + "\n";
        if (EnteredInteractions.Count == 3)
            interLabelText.text += "(G)" + EnteredInteractions[2].InterLabel + "\n";
    }
}