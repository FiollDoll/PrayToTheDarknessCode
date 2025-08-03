using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NotebookUI : MonoBehaviour
{
    public static NotebookUI Instance { get; private set; }
    private NotesManager _notesManager;
    [SerializeField] private GameObject buttonNpc;
    [Header("NotesPage")][SerializeField] private GameObject pageNotes;
    [SerializeField] private GameObject notesContainer;
    private AdaptiveScrollView _notesAdaptiveScrollView;

    [Header("NpcPage")][SerializeField] private GameObject pageNpc;
    [SerializeField] private GameObject npcContainer;
    private AdaptiveScrollView _npcAdaptiveScrollView;

    [Header("PersonsPage")][SerializeField] private GameObject pagePerson;
    [SerializeField] private GameObject personsContainer;

    [Header("Prefabs")][SerializeField] private GameObject buttonNotePrefab;
    [SerializeField] private GameObject buttonNpcPrefab;

    [Header("Other")][SerializeField] private GameObject pageReadNote;
    [SerializeField] private GameObject pageReadHuman;
    [SerializeField] private TextMeshProUGUI headerMain, noteMain;
    [SerializeField] private TextMeshProUGUI headerHuman, noteHuman, relationshipTextHuman;
    [SerializeField] private Image iconHuman;

    private void Awake() => Instance = this;

    private void Start()
    {
        _notesManager = NotesManager.Instance;
        _notesAdaptiveScrollView = pageNotes.transform.Find("Scroll View").GetComponent<AdaptiveScrollView>();
        _npcAdaptiveScrollView = pageNpc.transform.Find("Scroll View").GetComponent<AdaptiveScrollView>();
    }

    public async void OpenNoteMenu()
    {
        pageNotes.SetActive(true);
        foreach (Transform child in notesContainer.transform)
            Destroy(child.gameObject);

        await Task.Delay(10);

        for (int i = 0; i < _notesManager.PlayerNotes.Count; i++)
        {
            var obj = Instantiate(buttonNotePrefab, notesContainer.transform).GetComponent<PrefabInfo>();
            int number = i;
            obj.prefabButton.onClick.AddListener(delegate { ReadNote(number); });
            await Task.Delay(1);
        }

        await Task.Delay(10);
        _notesAdaptiveScrollView.UpdateContentSize();
    }

    public async Task OpenNpcMenu()
    {
        pageNpc.SetActive(true);
        foreach (Transform child in npcContainer.transform)
            Destroy(child.gameObject);

        await Task.Delay(10);

        for (int i = 0; i < PlayerStats.FamiliarNpc.Count; i++)
        {
            var obj = Instantiate(buttonNpcPrefab, npcContainer.transform).GetComponent<PrefabInfo>();
            Npc selectedNpc = PlayerStats.FamiliarNpc[i];
            obj.prefabNameTextMeshProUGUI.text = selectedNpc.nameOfNpc.Text;
            obj.prefabImage.sprite = selectedNpc.GetStyleIcon(Enums.IconMood.Standard);

            if (!NpcManager.Instance.npcTempInfo[selectedNpc].meetWithPlayer)
                obj.prefabImage.color = Color.black;
            int number = i;
            obj.prefabButton.onClick.AddListener(delegate { ReadNote(number, 1); });

            await Task.Delay(1);
        }
        await Task.Delay(10);
        _npcAdaptiveScrollView.UpdateContentSize();
    }

    public async Task OpenPersonMenu()
    {
        pagePerson.SetActive(true);
        await Task.Delay(10);
        int totalPerson = 1;

        foreach (Transform child in personsContainer.transform)
        {
            PlayerPerson person = Player.Instance.playerPersons[totalPerson];
            child.transform.Find("TextName").GetComponent<TextMeshProUGUI>().text = person.npcEntity.nameOfNpc.Text;
            child.transform.Find("TextInfo").GetComponent<TextMeshProUGUI>().text = new LanguageSetting("Влияние: ", "Influence: ").Text + NpcManager.Instance.npcTempInfo[person.npcEntity].influenceToPlayer + "%" + new LanguageSetting("\nУважение: ", "\nRespect: ").Text + NpcManager.Instance.npcTempInfo[person.npcEntity].respectToPlayer + "%";
            totalPerson++;
        }
    }

    private void ReadNote(int num, int mode = 0)
    {
        switch (mode)
        {
            case 0:
                {
                    pageNotes.SetActive(false);
                    pageReadNote.gameObject.SetActive(true);
                    headerMain.text = _notesManager.PlayerNotes[num].noteName.Text;
                    noteMain.text = _notesManager.PlayerNotes[num].description.Text;
                    break;
                }
            case 1:
                pageNpc.SetActive(false);
                pageReadHuman.gameObject.SetActive(true);
                Npc selectedNpc = PlayerStats.FamiliarNpc[num];
                headerHuman.text = selectedNpc.nameOfNpc.Text;
                iconHuman.sprite = selectedNpc.GetStyleIcon(Enums.IconMood.Standard);
                noteHuman.text = NpcManager.Instance.GetDescriptionOfNpc(selectedNpc);

                if (NpcManager.Instance.npcTempInfo[selectedNpc].meetWithPlayer)
                {
                    iconHuman.color = Color.white;
                    NpcTempInfo npcInfo = NpcManager.Instance.npcTempInfo[selectedNpc];
                    relationshipTextHuman.text = NpcManager.Instance.GetNpcRelationName(selectedNpc);
                }
                else
                {
                    iconHuman.color = Color.black;
                    relationshipTextHuman.text = "???";
                }
                break;
        }
    }

    public void CloseNotes()
    {
        pageNotes.SetActive(false);
        pageNpc.SetActive(false);
        pageReadHuman.SetActive(false);
        pageReadNote.SetActive(false);
        pagePerson.SetActive(false);
    }
}