using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ManageLocation
{
    public static ManageLocation Instance { get; private set; }
    public Location TotalLocation;

    private readonly Dictionary<string, Location> _locationsDict = new Dictionary<string, Location>();
    private GameObject _player;
    private CinemachineConfiner _cinemachineConfiner;
    public NpcController[] npсs;

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
        //npсs = FindObjectsByType<NpcController>(FindObjectsSortMode.None);
    }

    public void FastMoveToLocation(string locationName)
    {
        CoroutineContainer.Instance.StartCoroutine(ActivateLocation(locationName, "fromStairs")); // Для кнопок лестницы
        Interactions.Instance.floorChangeMenu.gameObject.SetActive(false);
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
    

    public IEnumerator ActivateLocation(string locationName, string spawn = "")
    {
        TotalLocation = GetLocation(locationName);
        GameMenuManager.Instance.noViewPanel.color = Color.black;
        yield return null;
        
        if (spawn == "")
            spawn = TotalLocation.spawns[0].name;
        
        Vector3 newPosition = TotalLocation.GetSpawn(spawn).position;
        _player.transform.position = newPosition;
        _cinemachineConfiner.m_BoundingVolume = TotalLocation.wallsForCamera;
        Player.Instance.canMove = false;
        Player.Instance.virtualCamera.m_Lens.OrthographicSize =
            CameraManager.Instance.StartCameraSize + TotalLocation.modifCamera;

        // Todo: переделать
        //foreach (NpcController totalNpc in npсs)
        //{
            //if (totalNpc.moveToPlayer)
                //totalNpc.gameObject.transform.position = newPosition;
        //}
        
        yield return null;
        // Инициализируем взаимодействия, если требуется
        foreach (KeyValuePair<GameObject, IInteractable> locationInteraction in TotalLocation.LocationInteractableObjects)
            locationInteraction.Value.Initialize();
        
        yield return null;
        SaveAndLoadManager.Instance.SaveGame();

        yield return new WaitForSeconds(1.5f);
        GameMenuManager.Instance.DisableNoVision();
        yield return null;
        
        Player.Instance.canMove = true;
    }
}