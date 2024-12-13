using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cinemachine;

public class ManageLocation : MonoBehaviour
{
    public Location totalLocation;
    public Location[] locations = new Location[0];
    [SerializeField] private Image noViewPanel;
    private AllScripts _scripts;
    private GameObject _player;

    private void Start()
    {
        // отключено для разработки
        // totalLocation = GetLocation("lea2");
        // Dev значение
        _scripts = GameObject.Find("scripts").GetComponent<AllScripts>();
        _scripts.manageLocation = this;
        totalLocation = GetLocation("mainMark");
        _player = GameObject.Find("Player");
    }

    public void ActivateLocation(string locationName, string spawn, bool withFade = true)
    {
        void LocationSetup(Location location)
        {
            _player.transform.position = location.GetSpawn(spawn).position;
            _scripts.player.virtualCamera.GetComponent<CinemachineConfiner2D>().m_BoundingShape2D =
                location.wallsForCamera as PolygonCollider2D;
            _scripts.player.canMove = false;
            _scripts.player.virtualCamera.GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize =
                _scripts.main.startCameraSize + location.modifCamera;

            NPC_movement[] NPCs = GameObject.FindObjectsOfType<NPC_movement>();
            foreach (NPC_movement totalNpc in NPCs)
            {
                if (totalNpc.moveToPlayer)
                    totalNpc.gameObject.transform.position = location.GetSpawn(spawn).position;
            }
        }

        totalLocation = GetLocation(locationName);
        if (withFade)
        {
            Sequence sequence = DOTween.Sequence();
            Tween fadeAnimation = noViewPanel.DOFade(100f, 0.2f).SetEase(Ease.InQuart);
            fadeAnimation.OnComplete(() => { LocationSetup(totalLocation); });
            sequence.Append(fadeAnimation);
            sequence.Append(noViewPanel.DOFade(100f, 0.5f).SetEase(Ease.InQuart)); // Задержка
            sequence.Append(noViewPanel.DOFade(0f, 0.5f).SetEase(Ease.OutQuart));
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