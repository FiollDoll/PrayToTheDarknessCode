using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cinemachine;
public class manageLocation : MonoBehaviour
{
    public Location totalLocation;
    public Location[] locations = new Location[0];
    [SerializeField] private Image noViewPanel;
    [SerializeField] private allScripts scripts;
    private GameObject player;

    private void Start()
    {
        // отключено для разработки
        // totalLocation = GetLocation("lea2");
        // Dev значение
        totalLocation = GetLocation("mainMark");
        player = GameObject.Find("Player");
    }

    public void ActivateLocation(string name, string spawn, bool withFade = true)
    {
        void locationSetup(Location location)
        {
            player.transform.position = location.GetSpawn(spawn).position;
            scripts.player.virtualCamera.GetComponent<CinemachineConfiner2D>().m_BoundingShape2D = location.wallsForCamera as PolygonCollider2D;
            scripts.player.canMove = false;
            scripts.player.virtualCamera.GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize = scripts.main.startCameraSize + location.modifCamera;

            NPC_movement[] NPCs = GameObject.FindObjectsOfType<NPC_movement>();
            foreach (NPC_movement totalNPC in NPCs)
            {
                if (totalNPC.moveToPlayer)
                    totalNPC.gameObject.transform.position = location.GetSpawn(spawn).position;
            }
        }

        totalLocation = GetLocation(name);
        if (withFade)
        {
            Sequence sequence = DOTween.Sequence();
            Tween fadeAnimation = noViewPanel.DOFade(100f, 0.2f).SetEase(Ease.InQuart);
            fadeAnimation.OnComplete(() =>
            {
                locationSetup(totalLocation);
            });
            sequence.Append(fadeAnimation);
            sequence.Append(noViewPanel.DOFade(100f, 0.5f).SetEase(Ease.InQuart)); // Задержка
            sequence.Append(noViewPanel.DOFade(0f, 0.5f).SetEase(Ease.OutQuart));
            sequence.Insert(0, transform.DOScale(new Vector3(1, 1, 1), sequence.Duration()));
            sequence.OnComplete(() => { scripts.player.canMove = true; });
        }
        else
        {
            locationSetup(totalLocation);
            scripts.player.canMove = true;
        }
    }

    public void FastMoveToLocation(string name)
    {
        ActivateLocation(name, "fromStairs"); // Для кнопок лестницы
        scripts.interactions.floorChangeMenu.gameObject.SetActive(false);
    }

    public void SetLockToLocation(string nameLocation, bool lockLocation) => GetLocation(nameLocation).locked = lockLocation;


    public Location GetLocation(string name)
    {
        foreach (Location location in locations)
        {
            if (location.gameName == name)
                return location;
        }
        return null;
    }
}