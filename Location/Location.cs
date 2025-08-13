using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Location")]
public class Location : ScriptableObject
{
    [Header("Основные настройки")] public string gameName;
    public LanguageSetting name;

    public bool locked, autoEnter;
    public float modifCamera;
    public SpawnInLocation[] spawns = new SpawnInLocation[0];
    private Dictionary<string, SpawnInLocation> _spawnsDict = new Dictionary<string, SpawnInLocation>();
    public Transform transformOfStairs; // Если есть лестница
    [HideInInspector] public Collider wallsForCamera;
    [HideInInspector] public GameObject locationGameObject;
    public Dictionary<GameObject, IInteractable> LocationInteractableObjects;

    public void UpdateLocationGameInScene()
    {
        locationGameObject = GameObject.Find(gameName);
        LocationInteractableObjects = new Dictionary<GameObject, IInteractable>();
        if (locationGameObject)
        {
            wallsForCamera = locationGameObject.transform.Find("wallForCam").GetComponent<Collider>();
            Transform[] children = locationGameObject.GetComponentsInChildren<Transform>(true);

            foreach (Transform child in children)
            {
                IInteractable[] interactables = child.GetComponentsInChildren<IInteractable>();
                foreach (IInteractable interactable in interactables)
                {
                    LocationInteractableObjects.TryAdd(child.gameObject, interactable);
                    var interactableMono = interactable as MonoBehaviour; // Для оптимизации
                    if (interactableMono)
                        interactableMono.enabled = false;
                }

                // Начисляем спавны
                foreach (SpawnInLocation spawn in spawns)
                {
                    if (spawn.name != child.name) continue;
                    spawn.spawn = child;
                    break;
                }
            }

            UpdateSpawnsDict();
        }
        else
            Debug.Log("Location obj dont found");
    }

    /// <summary>
    /// Получить все локации, на которые можно выйти
    /// </summary>
    public List<Location> GetAllGatesLocations()
    {
        List<Location> gatesLocations = new List<Location>();
        foreach (IInteractable interaction in LocationInteractableObjects.Values)
        {
            if (interaction is LocationInteraction interact)
                gatesLocations.Add(ManageLocation.Instance.GetLocation(interact.locationName));
        }

        return gatesLocations;
    }

    /// <summary>
    /// Метод для инициализации словаря спавнов. Только один раз!
    /// </summary>
    private void UpdateSpawnsDict()
    {
        foreach (SpawnInLocation spawn in spawns)
            _spawnsDict.TryAdd(spawn.name, spawn);
    }

    /// <summary>
    /// Возвращает Transform спавна по имени
    /// </summary>
    public Transform GetSpawn(string spawnName) => _spawnsDict.GetValueOrDefault(spawnName).spawn;

    /// <summary>
    /// Возвращаем Transform  по локации
    /// </summary>
    public Transform GetSpawn(Location location)
    {
        foreach (IInteractable interactable in LocationInteractableObjects.Values)
        {
            if (interactable is LocationInteraction interact)
            {
                if (location.gameName == interact.locationName)
                {
                    if (interact.spawnName == "") return null;
                    return location.GetSpawn(interact.spawnName);
                }
            }
        }
        return null;
    }
}

[System.Serializable]
public class SpawnInLocation
{
    public string name;
    [HideInInspector] public Transform spawn;
}