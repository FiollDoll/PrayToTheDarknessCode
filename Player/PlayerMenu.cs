using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerMenu : MonoBehaviour, IMenuable
{
    [SerializeField] private GameObject playerMenu, notebookMenu;
    [SerializeField] private GameObject buttonsPage;
    [SerializeField] private Slider hpSlider, hungerSlider, addictionSlider, sanitySlider;

    public GameObject menu => playerMenu;

    public void OnManagePlayerMenu(InputAction.CallbackContext context)
    {
        if (context.action.WasPerformedThisFrame())
            ManagePlayerMenu();
    }

    public async void ManagePlayerMenu()
    {
        if (!playerMenu.activeSelf)
        {
            if (GameMenuManager.Instance.CanMenuOpen())
                await ActivatePlayerMenu();
        }
        else
            DisablePlayerMenu();
    }

    public void ChoicePagePlayerMenu(int page)
    {
        buttonsPage.SetActive(false);
        // Открытие новых
        switch (page)
        {
            case 0: // Инвентарь
                InventoryManager.Instance.ManageInventoryPanel(true);
                break;
            case 1: // Записки
                notebookMenu.SetActive(true);
                NotebookUI.Instance.OpenNotes();
                break;
            case 2: // НПС
                notebookMenu.SetActive(true);
                NotebookUI.Instance.OpenRelation();
                break;
        }
    }

    public void CloseNotebookMenu()
    {
        notebookMenu.SetActive(false);
        NotebookUI.Instance.CloseNotes();
        buttonsPage.SetActive(true);
    }

    private async Task ActivatePlayerMenu()
    {
        Player.Instance.canMove = false;

        await CameraManager.Instance.CameraZoom(-10f, true);

        playerMenu.SetActive(true);
        buttonsPage.SetActive(true);
        UpdateHpSlider();
        UpdateHungerSlider();
        UpdateAddictionSlider();
        UpdateSanitySlider();
    }

    private async void UpdateAnySlider(Slider slider, float value)
    {
        while (Mathf.Abs(value - slider.value) > Mathf.Epsilon)
        {
            addictionSlider.value += Mathf.Clamp(slider.value + value, 0f, 1f);
            await Task.Delay(50);
        }
    }
    
    private void UpdateHpSlider() => UpdateAnySlider(hpSlider, Player.Instance.PlayerStats.Hp);
    private void UpdateHungerSlider() => UpdateAnySlider(hungerSlider, Player.Instance.PlayerStats.Hunger);

    private void UpdateAddictionSlider() => UpdateAnySlider(addictionSlider, Player.Instance.PlayerStats.Addiction);

    private void UpdateSanitySlider() => UpdateAnySlider(sanitySlider, Player.Instance.PlayerStats.Sanity);

    private async void DisablePlayerMenu()
    {
        Player.Instance.canMove = true;
        await CameraManager.Instance.CameraZoom(10f, true);
        playerMenu.SetActive(false);
        notebookMenu.SetActive(false);
        NotebookUI.Instance.CloseNotes();
    }
}