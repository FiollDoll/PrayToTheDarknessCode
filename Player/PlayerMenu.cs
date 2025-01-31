using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MyBox;

public class PlayerMenu : MonoBehaviour
{
    [SerializeField] private RectTransform[] buttonsPlayerMenuTransform = new RectTransform[0];
    [Header("Prefabs")] [SerializeField] private GameObject buttonNotePrefab;
    [SerializeField] private GameObject buttonNpcPrefab;

    [Foldout("Scene objects", true)] public GameObject playerMenu;
    [SerializeField] private GameObject pageNote, pageQuest, pageNpc, pageInventory;
    [SerializeField] private GameObject npcContainer, notesContainer, questsContainer;

    private AllScripts _scripts;

    private void Start() => _scripts = GameObject.Find("scripts").GetComponent<AllScripts>();

    public void ChoicePagePlayerMenu(int page)
    {
        // Начальное
        pageNote.gameObject.SetActive(false);
        pageQuest.gameObject.SetActive(false);
        pageNpc.gameObject.SetActive(false);
        foreach (RectTransform rt in buttonsPlayerMenuTransform)
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, 50f);
        
        // Работа с новой страницей(page)
        buttonsPlayerMenuTransform[page].anchoredPosition =
            new Vector2(buttonsPlayerMenuTransform[page].anchoredPosition.x,
                buttonsPlayerMenuTransform[page].anchoredPosition.y + 5.5f);

        _scripts.inventoryManager.ManageInventoryPanel(false);

        // Открытие новых
        switch (page)
        {
            case 0:
                _scripts.inventoryManager.ManageInventoryPanel(true);
                break;
            case 1:
                pageQuest.gameObject.SetActive(true);
                foreach (Transform child in questsContainer.transform)
                    Destroy(child.gameObject);

                for (int i = 0; i < _scripts.questsSystem.activeQuests.Count; i++)
                {
                    var obj = Instantiate(buttonNotePrefab, Vector3.zero, Quaternion.identity,
                        questsContainer.transform);
                    TextMeshProUGUI textName = obj.transform.Find("TextName").GetComponent<TextMeshProUGUI>();
                    if (_scripts.questsSystem.activeQuests[i] == _scripts.questsSystem.totalQuest)
                        textName.text = "-> " + _scripts.questsSystem.activeQuests[i].name;
                    else
                        textName.text = _scripts.questsSystem.activeQuests[i].name.text;
                    int number = i;
                    obj.GetComponent<Button>().onClick.AddListener(delegate { _scripts.notebook.ReadNote(number, 1); });
                }

                break;
            case 2:
                pageNote.gameObject.SetActive(true);
                foreach (Transform child in notesContainer.transform)
                    Destroy(child.gameObject);

                for (int i = 0; i < _scripts.notebook.playerNotes.Count; i++)
                {
                    var obj = Instantiate(buttonNotePrefab, Vector3.zero, Quaternion.identity,
                        notesContainer.transform);
                    if (_scripts.notebook.playerNotes[i].readed)
                        obj.transform.Find("TextName").GetComponent<TextMeshProUGUI>().text =
                            _scripts.notebook.playerNotes[i].name.text;
                    else
                        obj.transform.Find("TextName").GetComponent<TextMeshProUGUI>().text =
                            "(*)" + _scripts.notebook.playerNotes[i].name;
                    int number = i;
                    obj.GetComponent<Button>().onClick.AddListener(delegate { _scripts.notebook.ReadNote(number); });
                }

                pageNote.transform.Find("Scroll View").GetComponent<AdaptiveScrollView>().UpdateContentSize();
                break;
            case 3:
                pageNpc.gameObject.SetActive(true);
                foreach (Transform child in npcContainer.transform)
                    Destroy(child.gameObject);

                for (int i = 0; i < _scripts.player.familiarNpc.Count; i++)
                {
                    var obj = Instantiate(buttonNpcPrefab, Vector3.zero, Quaternion.identity, npcContainer.transform);
                    obj.transform.Find("TextName").GetComponent<TextMeshProUGUI>().text =
                        _scripts.player.familiarNpc[i].nameOfNpc.text;
                    obj.transform.Find("Icon").GetComponent<Image>().sprite =
                        _scripts.player.familiarNpc[i].GetStyleIcon(NpcIcon.IconMood.Standart);
                    obj.transform.Find("Icon").GetComponent<Image>().SetNativeSize();
                    int number = i;
                    obj.GetComponent<Button>().onClick.AddListener(delegate { _scripts.notebook.ReadNote(number, 2); });
                }

                pageNpc.transform.Find("Scroll View").GetComponent<AdaptiveScrollView>().UpdateContentSize();
                break;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (!_scripts.main.CheckAnyMenuOpen())
            {
                playerMenu.gameObject.SetActive(true);
                // TODO: сделать выбор по клавишам(какую стр открыть)
                ChoicePagePlayerMenu(0);
            }
            else
                playerMenu.gameObject.SetActive(false);

            _scripts.player.canMove = !playerMenu.activeSelf;
        }
    }
}