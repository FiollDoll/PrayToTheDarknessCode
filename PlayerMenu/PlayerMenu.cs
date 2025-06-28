using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMenu : MonoBehaviour, IMenuable
{
    [SerializeField] private GameObject playerMenu, notebookMenu;
    [SerializeField] private GameObject buttonsPage;
    [SerializeField] private Slider addictionSlider, sanitySlider;

    public GameObject menu => playerMenu;

    private Coroutine _cameraZoomCoroutine;

    public void ManagePlayerMenu()
    {
        if (!playerMenu.activeSelf)
        {
            if (GameMenuManager.Instance.CanMenuOpen())
                ActivatePlayerMenu();
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
        UpdateAddictionSlider();
        UpdateSanitySlider();
        _cameraZoomCoroutine = null;
    }

    private async void UpdateAddictionSlider()
    {
        for (int i = 0; i < Player.Instance.addiction; i++)
        {
            addictionSlider.value++;
            await Task.Delay(50);
        }
    }

    private async void UpdateSanitySlider()
    {
        for (int i = 0; i < Player.Instance.sanity; i++)
        {
            sanitySlider.value++;
            await Task.Delay(50);
        }
    }

    private async void DisablePlayerMenu()
    {
        Player.Instance.canMove = true;
        await CameraManager.Instance.CameraZoom(10f, true);
        addictionSlider.value = 0f;
        sanitySlider.value = 0f;
        playerMenu.SetActive(false);
        notebookMenu.SetActive(false);
        NotebookUI.Instance.CloseNotes();
        _cameraZoomCoroutine = null;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
            ManagePlayerMenu();
        if (Input.GetKeyDown(KeyCode.C))
            DevConsole.Instance.ManageDevMenu();
    }
}