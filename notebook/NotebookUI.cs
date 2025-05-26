using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NotebookUI : MonoBehaviour
{
    public static NotebookUI Instance { get; private set; }
    private NotesManager _notesManager;
    [SerializeField] private GameObject buttonNpc;
    [Header("NotesPage")] [SerializeField] private GameObject pageNotes;
    [SerializeField] private GameObject notesContainer;
    private AdaptiveScrollView _notesAdaptiveScrollView;

    [Header("NpcPage")] [SerializeField] private GameObject pageNpc;
    [SerializeField] private GameObject npcContainer;
    private AdaptiveScrollView _npcAdaptiveScrollView;

    [Header("Prefabs")] [SerializeField] private GameObject buttonNotePrefab;
    [SerializeField] private GameObject buttonNpcPrefab;

    [Header("Other")] [SerializeField] private GameObject pageReadNote;
    [SerializeField] private GameObject pageReadHuman;
    [SerializeField] private TextMeshProUGUI headerMain, noteMain;
    [SerializeField] private TextMeshProUGUI headerHuman, noteHuman, relationshipTextHuman;
    [SerializeField] private Slider sliderHuman;
    [SerializeField] private Image iconHuman;

    private void Awake() => Instance = this;
    

    public void Initialize(NotesManager notesManager)
    {
        _notesManager = notesManager;
        _notesAdaptiveScrollView = pageNotes.transform.Find("Scroll View").GetComponent<AdaptiveScrollView>();
        _npcAdaptiveScrollView = pageNpc.transform.Find("Scroll View").GetComponent<AdaptiveScrollView>();
    }

    public void OpenNotes() => StartCoroutine(OpenNoteMenu());

    public void OpenRelation() => StartCoroutine(OpenNpcMenu());

    private void ReadNote(int num, int mode = 0)
    {
        switch (mode)
        {
            case 0:
            {
                pageNotes.SetActive(false);
                pageReadNote.gameObject.SetActive(true);
                headerMain.text = _notesManager.PlayerNotes[num].noteName.text;
                noteMain.text = _notesManager.PlayerNotes[num].description.text;
                break;
            }
            case 1:
                pageNpc.SetActive(false);
                pageReadHuman.gameObject.SetActive(true);
                Npc selectedNpc = Player.Instance.familiarNpc[num];
                headerHuman.text = selectedNpc.nameOfNpc.text;
                noteHuman.text = selectedNpc.description.text;
                iconHuman.sprite = selectedNpc.GetStyleIcon(NpcIcon.IconMood.Standart);
                relationshipTextHuman.text =
                    new LanguageSetting($"Отношения({selectedNpc.NpcController.npcEntity.relationshipWithPlayer})",
                        $"Relationships({selectedNpc.NpcController.npcEntity.relationshipWithPlayer})").text;
                sliderHuman.value = selectedNpc.NpcController.npcEntity.relationshipWithPlayer;
                break;
        }
    }

    public void CloseNotes()
    {
        pageNotes.SetActive(false);
        pageNpc.SetActive(false);
        pageReadHuman.gameObject.SetActive(false);
        pageReadNote.gameObject.SetActive(false);
    }

    private IEnumerator OpenNoteMenu()
    {
        pageNotes.SetActive(true);
        foreach (Transform child in notesContainer.transform)
            Destroy(child.gameObject);

        yield return null;
        
        for (int i = 0; i < _notesManager.PlayerNotes.Count; i++)
        {
            var obj = Instantiate(buttonNotePrefab, notesContainer.transform).GetComponent<PrefabInfo>();
            int number = i;
            obj.prefabButton.onClick.AddListener(delegate { ReadNote(number); });
            yield return null;
        }
        yield return null;
        _notesAdaptiveScrollView.UpdateContentSize();
    }

    private IEnumerator OpenNpcMenu()
    {
        pageNpc.SetActive(true);
        foreach (Transform child in npcContainer.transform)
            Destroy(child.gameObject);
        
        yield return null;

        for (int i = 0; i < Player.Instance.familiarNpc.Count; i++)
        {
            var obj = Instantiate(buttonNpcPrefab, npcContainer.transform).GetComponent<PrefabInfo>();
            Npc selectedNpc = Player.Instance.familiarNpc[i];
            obj.prefabNameTextMeshProUGUI.text = selectedNpc.nameOfNpc.text;
            obj.prefabImage.sprite = selectedNpc.GetStyleIcon(NpcIcon.IconMood.Standart);
            int number = i;
            obj.prefabButton.onClick.AddListener(delegate{ReadNote(number, 1);});
            yield return null;
        }
        yield return null;
        _npcAdaptiveScrollView.UpdateContentSize();
    }
}