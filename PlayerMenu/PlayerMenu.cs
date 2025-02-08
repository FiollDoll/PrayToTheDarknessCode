using System;
using UnityEngine;
using MyBox;

public class PlayerMenu : MonoBehaviour, IMenuable
{
    [SerializeField] private RectTransform[] buttonsPlayerMenuTransform = new RectTransform[0];
    [Header("Prefabs")] [SerializeField] private GameObject buttonNotePrefab;
    [SerializeField] private GameObject buttonNpcPrefab;
    
    [Foldout("Scene objects", true)] [SerializeField]
    private GameObject playerMenu;

    public GameObject menu => playerMenu;

    [Header("NotesPage")] [SerializeField] private GameObject pageNote;
    [SerializeField] private GameObject notesContainer;
    private AdaptiveScrollView _notesAdaptiveScrollView;

    [Header("QuestsPage")] [SerializeField]
    private GameObject pageQuest;

    [SerializeField] private GameObject questsContainer;
    private AdaptiveScrollView _questsAdaptiveScrollView;

    [Header("NpcPage")] [SerializeField] private GameObject pageNpc;
    [SerializeField] private GameObject npcContainer;
    private AdaptiveScrollView _npcAdaptiveScrollView;

    [Header("InventoryPage")] [SerializeField]
    private GameObject pageInventory;

    private AdaptiveScrollView _inventoryAdaptiveScrollView;

    private AllScripts _scripts;

    private void Start()
    {
        _scripts = GameObject.Find("scripts").GetComponent<AllScripts>();
        _notesAdaptiveScrollView = pageNote.transform.Find("Scroll View").GetComponent<AdaptiveScrollView>();
        _questsAdaptiveScrollView = pageQuest.transform.Find("Scroll View").GetComponent<AdaptiveScrollView>();
        _npcAdaptiveScrollView = pageNpc.transform.Find("Scroll View").GetComponent<AdaptiveScrollView>();
    }

    public void ManageActivationMenu()
    {
        if (!playerMenu.activeSelf)
        {
            if (_scripts.main.CheckAnyMenuOpen())
            {
                playerMenu.gameObject.SetActive(true);
                // TODO: сделать выбор по клавишам(какую стр открыть)
                ChoicePagePlayerMenu(0);
            }
        }
        else
            playerMenu.gameObject.SetActive(false);

        _scripts.player.canMove = !playerMenu.activeSelf;
    }

    public void ChoicePagePlayerMenu(int page)
    {
        // Начальное
        pageNote.gameObject.SetActive(false);
        pageQuest.gameObject.SetActive(false);
        pageNpc.gameObject.SetActive(false);
        _scripts.inventoryManager.ManageInventoryPanel(false);
        foreach (RectTransform rt in buttonsPlayerMenuTransform)
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, 50f);

        // Работа с активной кнопкой
        buttonsPlayerMenuTransform[page].anchoredPosition =
            new Vector2(buttonsPlayerMenuTransform[page].anchoredPosition.x,
                buttonsPlayerMenuTransform[page].anchoredPosition.y + 5.5f);

        // Открытие новых
        switch (page)
        {
            case 0: // Инвентарь
                _scripts.inventoryManager.ManageInventoryPanel(true);
                break;
            case 1: // Квесты
                pageQuest.gameObject.SetActive(true);
                foreach (Transform child in questsContainer.transform)
                    Destroy(child.gameObject);

                for (int i = 0; i < _scripts.questsSystem.activeQuests.Count; i++)
                {
                    var obj = Instantiate(buttonNotePrefab, questsContainer.transform).GetComponent<PrefabInfo>();
                    if (_scripts.questsSystem.activeQuests[i] == _scripts.questsSystem.totalQuest)
                        obj.prefabNameTextMeshProUGUI.text = "-> " + _scripts.questsSystem.activeQuests[i].name;
                    else
                        obj.prefabNameTextMeshProUGUI.text = _scripts.questsSystem.activeQuests[i].name.text;
                    int number = i;
                    obj.prefabButton.onClick.AddListener(delegate { _scripts.notebook.ReadNote(number, 1); });
                }

                _questsAdaptiveScrollView.UpdateContentSize();
                break;
            case 2: // Записки
                pageNote.gameObject.SetActive(true);
                foreach (Transform child in notesContainer.transform)
                    Destroy(child.gameObject);

                for (int i = 0; i < _scripts.notebook.playerNotes.Count; i++)
                {
                    var obj = Instantiate(buttonNotePrefab, notesContainer.transform).GetComponent<PrefabInfo>();
                    if (_scripts.notebook.playerNotes[i].read)
                        obj.prefabNameTextMeshProUGUI.text = _scripts.notebook.playerNotes[i].name.text;
                    else
                        obj.prefabNameTextMeshProUGUI.text = "(*)" + _scripts.notebook.playerNotes[i].name;
                    int number = i;
                    obj.prefabButton.onClick.AddListener(delegate { _scripts.notebook.ReadNote(number); });
                }

                _notesAdaptiveScrollView.UpdateContentSize();
                break;
            case 3: // НПС
                pageNpc.gameObject.SetActive(true);
                foreach (Transform child in npcContainer.transform)
                    Destroy(child.gameObject);

                for (int i = 0; i < _scripts.player.familiarNpc.Count; i++)
                {
                    var obj = Instantiate(buttonNpcPrefab, npcContainer.transform).GetComponent<PrefabInfo>();
                    Npc selectedNpc = _scripts.player.familiarNpc[i];
                    obj.prefabNameTextMeshProUGUI.text = selectedNpc.nameOfNpc.text;
                    obj.prefabImage.sprite = selectedNpc.GetStyleIcon(NpcIcon.IconMood.Standart);
                    int number = i;
                    obj.prefabButton.onClick.AddListener(delegate { _scripts.notebook.ReadNote(number, 2); });
                }

                _npcAdaptiveScrollView.UpdateContentSize();
                break;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
            ManageActivationMenu();
    }
}