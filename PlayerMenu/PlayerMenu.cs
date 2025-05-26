using System.Collections;
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
        if (_cameraZoomCoroutine == null)
            _cameraZoomCoroutine = StartCoroutine(CameraManager.Instance.SmoothlyZoom(-10f));
        else
            CameraManager.Instance.CameraZoom(0f);
        yield return _cameraZoomCoroutine;

        playerMenu.SetActive(true);
        buttonsPage.SetActive(true);
        StartCoroutine(UpdateAddictionSlider());
        StartCoroutine(UpdateSanitySlider());
        _cameraZoomCoroutine = null;
    }

    private IEnumerator UpdateAddictionSlider()
    {
        for (int i = 0; i < Player.Instance.addiction; i++)
        {
            addictionSlider.value++;
            yield return null;
        }
    }

    private IEnumerator UpdateSanitySlider()
    {
        for (int i = 0; i < Player.Instance.sanity; i++)
        {
            sanitySlider.value++;
            yield return null;
        }
    }

    private IEnumerator DisablePlayerMenu()
    {
        Player.Instance.canMove = true;
        if (_cameraZoomCoroutine == null)
            _cameraZoomCoroutine = StartCoroutine(CameraManager.Instance.SmoothlyZoom(10f));
        else
            CameraManager.Instance.CameraZoom(0f);

        yield return _cameraZoomCoroutine;
        
        addictionSlider.value = 0f;
        sanitySlider.value = 0f;
        playerMenu.SetActive(false);
        notebookMenu.SetActive(false);
        NotebookUI.Instance.CloseNotes();
        _cameraZoomCoroutine = null;
    }
}