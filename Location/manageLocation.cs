using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ManageLocation : MonoBehaviour
{
    public Location totalLocation;
    public Location[] locations = new Location[0];
    private readonly Dictionary<string, Location> _locationsDict = new Dictionary<string, Location>();
    private AllScripts _scripts;
    private GameObject _player;
    private CinemachineConfiner2D _cinemachineConfiner2D;
    private NpcController[] _npсs;

    private void Start()
    {
        foreach (Location location in locations)
        {
            _locationsDict.TryAdd(location.gameName, location);
            location.UpdateSpawnsDict();
        }

        _scripts = GameObject.Find("scripts").GetComponent<AllScripts>();
        _player = GameObject.Find("Player");
        _cinemachineConfiner2D = _scripts.player.virtualCamera.GetComponent<CinemachineConfiner2D>();
        _scripts.manageLocation = this;
        totalLocation = GetLocation("mainMark");
        _npсs = FindObjectsByType<NpcController>(FindObjectsSortMode.None);
        
        LocationInteraction[] locationInteractions = FindObjectsByType<LocationInteraction>(FindObjectsSortMode.None);
        foreach (LocationInteraction locationInteraction in locationInteractions)
            locationInteraction.Initialize();
    }

    /// <summary>
    /// Установить настройки при переходе на локацию. Выноска для затемнения
    /// </summary>
    /// <param name="location"></param>
    /// <param name="spawn"></param>
    private void LocationSetup(Location location, string spawn = "")
    {
        if (spawn == "")
            spawn = location.spawns[0].name;
        Vector3 newPosition = location.GetSpawn(spawn).position;
        _player.transform.position = newPosition;
        _cinemachineConfiner2D.m_BoundingShape2D = location.wallsForCamera as PolygonCollider2D;
        _scripts.player.canMove = false;
        _scripts.player.virtualCamera.m_Lens.OrthographicSize =
            _scripts.main.startCameraSize + location.modifCamera;


        foreach (NpcController totalNpc in _npсs)
        {
            if (totalNpc.moveToPlayer)
                totalNpc.gameObject.transform.position = newPosition;
        }
    }

    /// <summary>
    /// Активировать новую локацию
    /// </summary>
    /// <param name="locationName"></param>
    /// <param name="spawn"></param>
    /// <param name="withFade"></param>
    public void ActivateLocation(string locationName, string spawn = "", bool withFade = true)
    {
        totalLocation = GetLocation(locationName);
        if (withFade)
        {
            _scripts.main.ActivateNoVision(1f, () => LocationSetup(totalLocation, spawn),
                () => { _scripts.player.canMove = true; });
        }
        else
        {
            LocationSetup(totalLocation, spawn);
            _scripts.player.canMove = true;
        }
    }

    public void FastMoveToLocation(string locationName)
    {
        ActivateLocation(locationName, "fromStairs"); // Для кнопок лестницы
        _scripts.interactions.floorChangeMenu.gameObject.SetActive(false);
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
    public Location GetLocation(string locationName)
    {
        return _locationsDict.GetValueOrDefault(locationName);
    }
}