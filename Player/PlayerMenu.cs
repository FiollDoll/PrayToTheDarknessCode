using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerMenu : MonoBehaviour, IMenuable
{
    public static PlayerMenu Instance { get; private set; }
    public GameObject menu => playerMenu;

    [Header("Menu")]
    [SerializeField] private GameObject playerMenu;
    [SerializeField] private GameObject notebookMenu;
    [SerializeField] private GameObject buttonsPage;

    [Header("PlayerMenuElements")]
    [SerializeField] private Image personImage;
    [SerializeField] private Button buttonPersons, buttonInventory, buttonHumans;
    [SerializeField] private Slider hpSlider, hungerSlider, addictionSlider, sanitySlider;

    [HideInInspector] public bool personCan, inventoryCan, humansCan;

    private bool _isZooming; // Антиспам

    private void Awake() => Instance = this;

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
                NotebookUI.Instance.OpenNoteMenu();
                break;
            case 2: // НПС
                notebookMenu.SetActive(true);
                NotebookUI.Instance.OpenNpcMenu();
                break;
            case 3: // ЛИЧНОСТИ
                notebookMenu.SetActive(true);
                NotebookUI.Instance.OpenPersonMenu();
                break;
        }
    }

    public void CloseAllSubMenu()
    {
        notebookMenu.SetActive(false);
        NotebookUI.Instance.CloseNotes();
        InventoryUI.Instance.ManageInventoryPanel(false);
        buttonsPage.SetActive(true);
    }

    public async Task ActivatePlayerMenu()
    {
        if (_isZooming) return;
        Player.Instance.canMove = false;
        _isZooming = true;
        await CameraManager.Instance.CameraZoom(-10f, true);
        _isZooming = false;

        playerMenu.SetActive(true);
        buttonsPage.SetActive(true);
        // Включаем кнопочки и их текст
        buttonHumans.interactable = humansCan;
        buttonHumans.transform.Find("Text").gameObject.SetActive(buttonHumans.interactable);
        buttonInventory.interactable = inventoryCan;
        buttonInventory.transform.Find("Text").gameObject.SetActive(buttonInventory.interactable);
        buttonPersons.interactable = personCan;
        buttonPersons.transform.Find("Text").gameObject.SetActive(buttonPersons.interactable);

        personImage.sprite = Player.Instance.selectedPerson.npcEntity.GetStyleIcon();
    }

    public async void DisablePlayerMenu()
    {
        if (_isZooming) return;
        Player.Instance.canMove = true;
        _isZooming = true;
        await CameraManager.Instance.CameraZoom(10f, true);
        playerMenu.GetComponent<Animator>().Play("Disable");
        _isZooming = false;
        await Task.Delay(1000);

        playerMenu.SetActive(false);
        CloseAllSubMenu();

        CameraManager.Instance.CameraZoom(0f, true); // Чтобы камера не улетала
        NotebookUI.Instance.CloseNotes();
    }

    public void UpdateHpSlider() => hpSlider.value = PlayerStats.Hp;
    public void UpdateHungerSlider() => hungerSlider.value = PlayerStats.Hunger;
    public void UpdateAddictionSlider() => addictionSlider.value = PlayerStats.Addiction;
    public void UpdateSanitySlider() => sanitySlider.value = PlayerStats.Sanity;
}