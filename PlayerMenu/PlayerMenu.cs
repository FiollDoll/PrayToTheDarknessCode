using System.Collections;
using UnityEngine;
using MyBox;

public class PlayerMenu : MonoBehaviour, IMenuable
{
    [SerializeField] private GameObject playerMenu, notebookMenu;
    [SerializeField] private GameObject buttonsPage;
    public GameObject menu => playerMenu;

    [Header("InventoryPage")] [SerializeField]
    private GameObject pageInventory;

    public void ManagePlayerMenu()
    {
        if (!playerMenu.activeSelf)
        {
            if (GameMenuManager.Instance.CanMenuOpen())
                StartCoroutine(ActivatePlayerMenu());
        }
        else
            StartCoroutine(DisablePlayerMenu());
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
            ManagePlayerMenu();
    }

    private IEnumerator ActivatePlayerMenu()
    {
        Player.Instance.canMove = false;
        Coroutine cameraZoom = StartCoroutine(CameraManager.Instance.SmoothlyZoom(-10f));
        yield return cameraZoom;
        // Сделать плавное изменение slider
        playerMenu.SetActive(true);
        buttonsPage.SetActive(true);
    }

    private IEnumerator DisablePlayerMenu()
    {
        Player.Instance.canMove = true;
        Coroutine cameraZoom = StartCoroutine(CameraManager.Instance.SmoothlyZoom(10f));
        yield return cameraZoom;
        // Сделать плавное изменение slider
        playerMenu.SetActive(false);
        notebookMenu.SetActive(false);
        NotebookUI.Instance.CloseNotes();
    }
}