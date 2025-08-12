using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class MiniGamesManager : MonoBehaviour
{
    public static MiniGamesManager Instance { get; private set; }
    public FastChangesController gameChangesAfterWin;
    [SerializeField] private GameObject[] miniGamesPrefabs = new GameObject[0];
    private Dictionary<string, GameObject> miniGamesDict = new Dictionary<string, GameObject>();

    private void Start()
    {
        Instance = this;
        miniGamesDict.Add("wreathGame", miniGamesPrefabs[0]);
        miniGamesDict.Add("numbersAndArrowsGame", miniGamesPrefabs[1]);
    }

    public async void StartMiniGame(string gameName, FastChangesController changesAfterWin, bool withFade = false)
    {
        GameObject miniGameObj = Instantiate(miniGamesDict[gameName], Vector3.zero, Quaternion.identity, gameObject.transform);
        miniGameObj.transform.localPosition = Vector3.zero;
        gameChangesAfterWin = changesAfterWin;
        if (withFade)
            GameMenuManager.Instance.NoVisionForTime(1.2f, ActivateMenuTask(miniGameObj));
    }

    public async Task ActivateMenuTask(GameObject menu) => menu.SetActive(true);
}
