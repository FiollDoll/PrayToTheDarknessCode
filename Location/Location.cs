using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Location
{
    [Header("Основные настройки")]
    public string gameName;
    public string name => PlayerPrefs.GetString("language") == "ru" ? ruName : enName;
    public string ruName, enName;
    
    public bool locked, autoEnter;
    public Collider2D wallsForCamera;
    public float modifCamera;
    public SpawnInLocation[] spawns = new SpawnInLocation[0];
    private Dictionary<string, SpawnInLocation> _spawnsDict = new Dictionary<string, SpawnInLocation>();
    public Transform transformOfStairs = null; // Если есть лестница

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