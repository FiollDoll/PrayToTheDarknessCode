using UnityEngine;
[System.Serializable]
public class Location
{
    [System.Serializable]
    public class spawnInLocation
    {
        public string name;
        public Transform spawn;
    }

    public string gameName;
    [HideInInspector]
    public string name
    {
        get
        {
            if (PlayerPrefs.GetString("language") == "ru")
                return ruName;
            else
                return enName;
        }
    }
    public string ruName, enName;
    public bool locked, autoEnter;
    public Collider2D wallsForCamera;
    public float modifCamera;
    public spawnInLocation[] spawns = new spawnInLocation[0];
    public Transform transformOfStairs = null; // Если есть лестница
    public Transform GetSpawn(string name)
    {
        foreach (spawnInLocation spawn in spawns)
        {
            if (spawn.name == name)
            {
                return spawn.spawn;
            }
        }
        return null;
    }
}