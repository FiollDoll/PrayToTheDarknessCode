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

    public void Initialize()
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
    }

    public void FastMoveToLocation(string locationName)
    {
        ActivateLocation(locationName, "fromStairs"); // Для кнопок лестницы
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

    public async Task ActivateLocation(string locationName, string spawn = "")
    {
        TotalLocation = GetLocation(locationName);
        GameMenuManager.Instance.noViewPanel.color = Color.black;
        await Task.Delay(10);
        
        if (spawn == "")
            spawn = TotalLocation.spawns[0].name;
        
        Vector3 newPosition = TotalLocation.GetSpawn(spawn).position;
        _player.transform.position = newPosition;
        _cinemachineConfiner.m_BoundingVolume = TotalLocation.wallsForCamera;
        Player.Instance.canMove = false;
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
            locationInteraction.Value.Initialize();
        
        await Task.Delay(10);
        SaveAndLoadManager.Instance.SaveGame();

        await Task.Delay(1500);
        GameMenuManager.Instance.DisableNoVision();
        await Task.Delay(10);
        
        Player.Instance.canMove = true;
    }
}