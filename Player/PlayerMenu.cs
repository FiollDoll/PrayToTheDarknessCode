using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerMenu : MonoBehaviour, IMenuable
{
    public static PlayerMenu Instance { get; private set; }
    [SerializeField] private GameObject playerMenu, notebookMenu;
    [SerializeField] private GameObject buttonsPage;
    [SerializeField] private Slider hpSlider, hungerSlider, addictionSlider, sanitySlider;

    private bool _isZooming; // Антиспам
    public GameObject menu => playerMenu;

    private void Start()
    {
        UpdateHpSlider();
        UpdateHungerSlider();
        UpdateAddictionSlider();
        UpdateSanitySlider();
    }

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
                InventoryUI.Instance.ManageInventoryPanel(true);
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
        if (_isZooming) return;
        Player.Instance.canMove = false;
        _isZooming = true;
        await CameraManager.Instance.CameraZoom(-10f, true);
        _isZooming = false;
        playerMenu.SetActive(true);
        buttonsPage.SetActive(true);
    }

    private async void DisablePlayerMenu()
    {
        if (_isZooming) return;
        Player.Instance.canMove = true;
        _isZooming = true;
        await CameraManager.Instance.CameraZoom(10f, true);
        _isZooming = false;
        playerMenu.SetActive(false);
        notebookMenu.SetActive(false);
        NotebookUI.Instance.CloseNotes();
    }

    private async void UpdateAnySlider(Slider slider, float value)
    {
        float step = 0.01f * Mathf.Sign(value - slider.value);
        while (Mathf.Abs(value - slider.value) > Mathf.Epsilon)
        {
            slider.value += Mathf.Clamp(slider.value + step, 0f, 1f);
            await Task.Delay(50);
        }
    }

    public void UpdateHpSlider() => UpdateAnySlider(hpSlider, PlayerStats.Hp);
    public void UpdateHungerSlider() => UpdateAnySlider(hungerSlider, PlayerStats.Hunger);
    public void UpdateAddictionSlider() => UpdateAnySlider(addictionSlider, PlayerStats.Addiction);
    public void UpdateSanitySlider() => UpdateAnySlider(sanitySlider, PlayerStats.Sanity);
}