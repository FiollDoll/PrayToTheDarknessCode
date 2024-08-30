using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cinemachine;
public class manageLocation : MonoBehaviour
{
    public location totalLocation;
    public location[] locations = new location[0];
    [SerializeField] private Image noViewPanel;
    [SerializeField] private allScripts scripts;
    private GameObject player;

    private void Start()
    {
        totalLocation = GetLocation("mainMark");
        player = GameObject.Find("Player");
    }

    public void ActivateLocation(string name, string spawn, bool withFade = true)
    {
        void LocationActivated(location location)
        {
            scripts.player.virtualCamera.GetComponent<CinemachineConfiner2D>().m_BoundingShape2D = location.wallsForCamera as PolygonCollider2D;
            player.transform.position = location.spawns[int.Parse(spawn)].position;
            NPC_movement[] NPCs = GameObject.FindObjectsOfType<NPC_movement>();
            foreach(NPC_movement totalNPC in NPCs)
            {
                if (totalNPC.moveToPlayer)
                    totalNPC.gameObject.transform.position = location.spawns[int.Parse(spawn)].position;
            }
        }

        totalLocation = GetLocation(name);
        if (withFade)
        {
            Sequence sequence = DOTween.Sequence();
            Tween fadeAnimation = noViewPanel.DOFade(100f, 0.5f).SetEase(Ease.InQuart);
            fadeAnimation.OnComplete(() =>
            {
                LocationActivated(totalLocation);
            });
            sequence.Append(fadeAnimation);
            sequence.Append(noViewPanel.DOFade(0f, 0.5f).SetEase(Ease.OutQuart));
            sequence.Insert(0, transform.DOScale(new Vector3(3, 3, 3), sequence.Duration()));
        }
        else
            LocationActivated(totalLocation);
    }

    public location GetLocation(string name)
    {
        foreach (location location in locations)
        {
            if (location.gameName == name)
                return location;
        }
        return null;
    }
}


[System.Serializable]
public class location
{
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
    public Transform[] spawns; // 0 - left, 1 - right, 2 - up, 3 - down
}