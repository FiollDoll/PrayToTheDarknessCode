using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cinemachine;

public class ManageLocation : MonoBehaviour
{
    public Location totalLocation;
    public Location[] locations = new Location[0];
    private Image _noViewPanel;
    private AllScripts _scripts;
    private GameObject _player;
    private CinemachineConfiner2D _cinemachineConfiner2D;
    private NPC_movement[] _NPCs;

    public void Initialize()
    {
        _scripts = GameObject.Find("scripts").GetComponent<AllScripts>();
        _player = GameObject.Find("Player");
        _noViewPanel = _scripts.main.noViewPanel;
        _cinemachineConfiner2D = _scripts.player.virtualCamera.GetComponent<CinemachineConfiner2D>();
        _scripts.manageLocation = this;
        totalLocation = GetLocation("mainMark");
        _NPCs = FindObjectsByType<NPC_movement>(FindObjectsSortMode.None);
    }

    public void ActivateLocation(string locationName, string spawn = "", bool withFade = true)
    {
        void LocationSetup(Location location)
        {
            if (spawn == "")
                spawn = location.spawns[0].name;
            _player.transform.position = location.GetSpawn(spawn).position;
            _cinemachineConfiner2D.m_BoundingShape2D = location.wallsForCamera as PolygonCollider2D;
            _scripts.player.canMove = false;
            _scripts.player.virtualCamera.m_Lens.OrthographicSize =
                _scripts.main.startCameraSize + location.modifCamera;

            
            foreach (NPC_movement totalNpc in _NPCs)
            {
                if (totalNpc.moveToPlayer)
                    totalNpc.gameObject.transform.position = location.GetSpawn(spawn).position;
            }
        }

        totalLocation = GetLocation(locationName);
        if (withFade)
        {
            Sequence sequence = DOTween.Sequence();
            Tween fadeAnimation = _noViewPanel.DOFade(100f, 0.2f).SetEase(Ease.InQuart);
            fadeAnimation.OnComplete(() => { LocationSetup(totalLocation); });
            sequence.Append(fadeAnimation);
            sequence.Append(_noViewPanel.DOFade(100f, 0.5f).SetEase(Ease.InQuart)); // Задержка
            sequence.Append(_noViewPanel.DOFade(0f, 0.5f).SetEase(Ease.OutQuart));
            sequence.Insert(0, transform.DOScale(new Vector3(1, 1, 1), sequence.Duration()));
            sequence.OnComplete(() => { _scripts.player.canMove = true; });
        }
        else
        {
            LocationSetup(totalLocation);
            _scripts.player.canMove = true;
        }
    }

    public void FastMoveToLocation(string locationName)
    {
        ActivateLocation(locationName, "fromStairs"); // Для кнопок лестницы
        _scripts.interactions.floorChangeMenu.gameObject.SetActive(false);
    }

    public void SetLockToLocation(string nameLocation, bool lockLocation) =>
        GetLocation(nameLocation).locked = lockLocation;


    public Location GetLocation(string locationName)
    {
        foreach (Location location in locations)
        {
            if (location.gameName == locationName)
                return location;
        }

        return null;
    }
}