using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using TMPro;

public class ManageLocation : MonoBehaviour
{
    [Header("Текущая локация")] public Location totalLocation;
    [Header("Все локации")] public Location[] locations = new Location[0];

    [Header("Отображение взаимодействий")] [SerializeField]
    private GameObject labelTextPrefab;

    [SerializeField] private GameObject containerOfLabels;
    private readonly Dictionary<string, Location> _locationsDict = new Dictionary<string, Location>();
    private GameObject _player;
    private CinemachineConfiner _cinemachineConfiner;
    private NpcController[] _npсs;
    private AllScripts _scripts;

    private void Start()
    {
        foreach (Location location in locations)
        {
            location.UpdateLocationGameInScene();
            _locationsDict.TryAdd(location.gameName, location);
            location.UpdateSpawnsDict();
        }

        _scripts = GameObject.Find("scripts").GetComponent<AllScripts>();
        _player = GameObject.Find("Player");
        _cinemachineConfiner = _scripts.player.virtualCamera.GetComponent<CinemachineConfiner>();
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
        _cinemachineConfiner.m_BoundingVolume = location.wallsForCamera;
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

        _scripts.saveAndLoadManager.SaveGame();
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            _scripts.player.canMove = false;
            foreach (KeyValuePair<GameObject, IInteractable> interactableObject in totalLocation
                         .LocationInteractableObjects)
            {
                var obj = Instantiate(labelTextPrefab,
                    interactableObject.Key.transform.position - new Vector3(0, 0, 1f),
                    Quaternion.identity,
                    containerOfLabels.transform);
                obj.GetComponent<TextMeshProUGUI>().text = interactableObject.Value.interLabel;
            }
        }

        if (Input.GetKeyUp(KeyCode.Z))
        {
            _scripts.player.canMove = true;
            foreach (Transform labels in containerOfLabels.transform)
                Destroy(labels.gameObject);
        }
    }
}