using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Cinemachine;

public class ManageLocation
{
    public static ManageLocation Instance { get; private set; }
    public Location TotalLocation;
    public List<NpcController> NpcAtTotalLocation = new List<NpcController>();

    private readonly Dictionary<string, Location> _locationsDict = new Dictionary<string, Location>();
    private GameObject _player;
    private CinemachineConfiner _cinemachineConfiner;

    public Task Initialize()
    {
        Instance = this;
        Location[] locations = Resources.LoadAll<Location>("Locations/");
        foreach (Location location in locations)
        {
            location.UpdateLocationGameInScene();
            _locationsDict.TryAdd(location.gameName, location);
        }

        _player = Player.Instance.gameObject;
        _cinemachineConfiner = Player.Instance.virtualCamera.GetComponent<CinemachineConfiner>();
        return Task.CompletedTask;
    }

    public async void FastMoveToLocation(string locationName)
    {
        await ActivateLocation(locationName, "fromStairs"); // Для кнопок лестницы
        //Interactions.Instance.floorChangeMenu.gameObject.SetActive(false);
    }

    /// <summary>
    /// Блокировка/разблокировка определенной локации по имени
    /// </summary>
    /// <param name="nameLocation"></param>
    /// <param name="lockLocation"></param>
    public void SetLockToLocation(string nameLocation, bool lockLocation) =>
        GetLocation(nameLocation).locked = lockLocation;

    /// <summary>
    /// Возвращает локацию по имени
    /// </summary>
    /// <param name="locationName"></param>
    /// <returns></returns>
    public Location GetLocation(string locationName) => _locationsDict.GetValueOrDefault(locationName);

    public async Task ActivateLocation(string locationName, string spawn = "", bool fade = true)
    {
        if (string.IsNullOrEmpty(locationName)) return;

        TotalLocation = GetLocation(locationName);
        if (!TotalLocation) return;

        Player.Instance.canMove = false;
        if (fade)
            await GameMenuManager.Instance.ActivateNoVision();

        if (spawn == "")
            spawn = TotalLocation.spawns[0].name;

        Vector3 newPosition = TotalLocation.GetSpawn(spawn).position;
        _player.transform.position = newPosition;
        _cinemachineConfiner.m_BoundingVolume = TotalLocation.wallsForCamera;
        Player.Instance.virtualCamera.m_Lens.OrthographicSize =
            CameraManager.Instance.StartCameraSize + TotalLocation.modifCamera;

        NpcAtTotalLocation = new List<NpcController>();
        foreach (Npc totalNpc in NpcManager.Instance.AllNpc)
        {
            if (totalNpc.NpcController is NpcController npcController)
            {
                if (npcController.totalLocation == TotalLocation.gameName)
                    NpcAtTotalLocation.Add(npcController);

                if (npcController.moveToPlayer)
                    npcController.gameObject.transform.position = newPosition;
            }
        }

        await Task.Delay(10);
        // Инициализируем взаимодействия, если требуется
        foreach (KeyValuePair<GameObject, IInteractable> locationInteraction in TotalLocation.LocationInteractableObjects)
        {
            var interactable = locationInteraction.Value as MonoBehaviour; // Для оптимизации
            if (interactable)
                interactable.enabled = true;
        }

        await SaveAndLoadManager.Instance.SaveGame();

        await Task.Delay(1500);
        if (fade)
            await GameMenuManager.Instance.DisableNoVision();

        Player.Instance.canMove = true;
    }
}