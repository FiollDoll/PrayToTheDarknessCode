using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using TMPro;
using UnityEngine.Serialization;

public class ManageLocation : MonoBehaviour
{
    public static ManageLocation Instance { get; private set; }
    [Header("Текущая локация")] public Location totalLocation;
    [Header("Все локации")] public Location[] locations = new Location[0];

    [Header("Отображение взаимодействий")] [SerializeField]
    private GameObject labelTextPrefab;

    [SerializeField] private GameObject containerOfLabels;
    private readonly Dictionary<string, Location> _locationsDict = new Dictionary<string, Location>();
    private GameObject _player;
    private CinemachineConfiner _cinemachineConfiner;
    public NpcController[] npсs;

    private void Awake()
    {
        Instance = this;
    }

    public void Initialize()
    {
        foreach (Location location in locations)
        {
            location.UpdateLocationGameInScene();
            _locationsDict.TryAdd(location.gameName, location);
            location.UpdateSpawnsDict();
        }

        _player = Player.Instance.gameObject;
        _cinemachineConfiner = Player.Instance.virtualCamera.GetComponent<CinemachineConfiner>();
        npсs = FindObjectsByType<NpcController>(FindObjectsSortMode.None);

        LocationInteraction[] locationInteractions = FindObjectsByType<LocationInteraction>(FindObjectsSortMode.None);
        foreach (LocationInteraction locationInteraction in locationInteractions)
            locationInteraction.Initialize();
    }

    public void FastMoveToLocation(string locationName)
    {
        StartCoroutine(ActivateLocation(locationName, "fromStairs")); // Для кнопок лестницы
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
    public Location GetLocation(string locationName)
    {
        return _locationsDict.GetValueOrDefault(locationName);
    }

    public IEnumerator ActivateLocation(string locationName, string spawn = "")
    {
        totalLocation = GetLocation(locationName);
        GameMenuManager.Instance.noViewPanel.color = Color.black;
        yield return null;
        
        if (spawn == "")
            spawn = totalLocation.spawns[0].name;
        
        Vector3 newPosition = totalLocation.GetSpawn(spawn).position;
        _player.transform.position = newPosition;
        _cinemachineConfiner.m_BoundingVolume = totalLocation.wallsForCamera;
        Player.Instance.canMove = false;
        Player.Instance.virtualCamera.m_Lens.OrthographicSize =
            CameraManager.Instance.StartCameraSize + totalLocation.modifCamera;


        foreach (NpcController totalNpc in npсs)
        {
            if (totalNpc.moveToPlayer)
                totalNpc.gameObject.transform.position = newPosition;
        }
        
        yield return null;
        SaveAndLoadManager.Instance.SaveGame();

        yield return new WaitForSeconds(1.5f);
        GameMenuManager.Instance.DisableNoVision();
        yield return null;
        
        Player.Instance.canMove = true;
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Player.Instance.canMove = false;
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
            Player.Instance.canMove = true;
            foreach (Transform labels in containerOfLabels.transform)
                Destroy(labels.gameObject);
        }
    }
}