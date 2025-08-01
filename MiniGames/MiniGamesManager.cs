using UnityEngine;
using System.Threading.Tasks;

public class MiniGamesManager : MonoBehaviour
{
    public static MiniGamesManager Instance { get; private set; }
    public GameObject[] miniGamesPrefabs = new GameObject[0];

    private void Awake() => Instance = this;

    public async void StartMiniGame(GameObject miniGamePrefab)
    {
        GameMenuManager.Instance.NoVisionForTime(1.2f, ActivateMenuTask(miniGamePrefab));
    }

    public void StartWreathMiniGame() => StartMiniGame(miniGamesPrefabs[0]);

    public async Task ActivateMenuTask(GameObject menu) => menu.SetActive(true);
}
