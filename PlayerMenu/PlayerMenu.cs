using System.Collections;
using UnityEngine;
using MyBox;

public class PlayerMenu : MonoBehaviour, IMenuable
{
    [SerializeField] private GameObject playerMenu, notebookMenu;

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
        playerMenu.gameObject.SetActive(true);
    }

    private IEnumerator DisablePlayerMenu()
    {
        Player.Instance.canMove = true;
        Coroutine cameraZoom = StartCoroutine(CameraManager.Instance.SmoothlyZoom(10f));
        yield return cameraZoom;
        // Сделать плавное изменение slider
        playerMenu.gameObject.SetActive(false);
        notebookMenu.SetActive(false);
        NotebookUI.Instance.CloseNotes();
    }
}