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
        }

        foreach (location location in locations)
        {
            if (location.name == name)
            {
                totalLocation = location;
                if (withFade)
                {
                    Sequence sequence = DOTween.Sequence();
                    Tween fadeAnimation = noViewPanel.DOFade(100f, 0.5f).SetEase(Ease.InQuart);
                    fadeAnimation.OnComplete(() =>
                    {
                        LocationActivated(location);
                    });
                    sequence.Append(fadeAnimation);
                    sequence.Append(noViewPanel.DOFade(0f, 0.5f).SetEase(Ease.OutQuart));
                    sequence.Insert(0, transform.DOScale(new Vector3(3, 3, 3), sequence.Duration()));
                }
                else
                    LocationActivated(location);
            }
        }
    }

    public location GetLocation(string name)
    {
        foreach (location location in locations)
        {
            if (location.name == name)
                return location;
        }
        return null;
    }
}


[System.Serializable]
public class location
{
    public string name;
    public bool locked, autoEnter;
    public Collider2D wallsForCamera;
    public Transform[] spawns; // 0 - left, 1 - right, 2 - up, 3 - down
}