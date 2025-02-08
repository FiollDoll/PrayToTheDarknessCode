using System.Collections.Generic;
using MyBox;
using UnityEngine;

[System.Serializable]
public class Location
{
    [Header("Основные настройки")] public string gameName;
    public LanguageSetting name;

    public bool locked, autoEnter;
    public Collider2D wallsForCamera;
    public float modifCamera;
    public SpawnInLocation[] spawns = new SpawnInLocation[0];
    private Dictionary<string, SpawnInLocation> _spawnsDict = new Dictionary<string, SpawnInLocation>();
    public Transform transformOfStairs; // Если есть лестница
    private GameObject _locationGameObject;
    public Dictionary<GameObject, IInteractable> LocationInteractableObjects = new Dictionary<GameObject, IInteractable>();

    public void UpdateLocationGameInScene()
    {
        _locationGameObject = GameObject.Find(gameName);
        if (_locationGameObject != null)
        {
            Transform[] children = _locationGameObject.GetComponentsInChildren<Transform>(true);

            // Перебор всех дочерних элементов
            foreach (Transform child in children)
            {
                if (child.HasComponent<IInteractable>())
                    LocationInteractableObjects.Add(child.gameObject, child.GetComponent<IInteractable>());
            }
        }
        else
            Debug.Log("Location obj dont found");
    }

    /// <summary>
    /// Метод для инициализации словаря спавнов. Только один раз!
    /// </summary>
    public void UpdateSpawnsDict()
    {
        foreach (SpawnInLocation spawn in spawns)
            _spawnsDict.TryAdd(spawn.name, spawn);
    }

    /// <summary>
    /// Возвращает Transform спавна по имени
    /// </summary>
    /// <param name="spawnName"></param>
    /// <returns></returns>
    public Transform GetSpawn(string spawnName)
    {
        return _spawnsDict[spawnName].spawn;
    }
}

[System.Serializable]
public class SpawnInLocation
{
    public string name;
    public Transform spawn;
}