using UnityEngine;

[System.Serializable]
public class Location
{
    [System.Serializable]
    public class SpawnInLocation
    {
        public string name;
        public Transform spawn;
    }

    [Header("Основные настройки")]
    public string gameName;
    public string name => PlayerPrefs.GetString("language") == "ru" ? ruName : enName;
    public string ruName, enName;
    
    public bool locked, autoEnter;
    public Collider2D wallsForCamera;
    public float modifCamera;
    public SpawnInLocation[] spawns = new SpawnInLocation[0];
    public Transform transformOfStairs = null; // Если есть лестница

    public Transform GetSpawn(string spawnName)
    {
        foreach (SpawnInLocation spawn in spawns)
        {
            if (spawn.name == spawnName)
                return spawn.spawn;
        }
        
        Debug.Log("Spawn don`t finded");
        return null;
    }
}