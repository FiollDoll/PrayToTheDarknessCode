using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.U2D.Animation;
using System.Linq;

public class NpcController : MonoBehaviour, IHumanable
{
    [Header("MainInfo")] public Npc NpcEntity { get; set; }
    public string SelectedStyle { get; set; }
    public string totalLocation;

    [Header("Schedule")]
    private ScheduleCase totalScheduleCase;
    public Dictionary<int, ScheduleCase> scheduleHourDict = new Dictionary<int, ScheduleCase>();

    [Header("Preference")]
    [SerializeField] private float speed;
    [SerializeField] private bool timeImmunity;

    [SerializeField] private GameObject model;

    [Header("Move To Player Preference")] public bool moveToPlayer;

    [Header("Move To Point Preference")] public bool moveToPoint;
    public bool waitPlayerAndMove;

    // Далее - информация о движении
    private Transform point;
    private Transform totalPoint;
    private Transform totalSpawn;
    private List<Location> pointWay = new List<Location>();

    private int totalWay;
    private string locationOfPointName;
    private Location locationPointTarget;

    private bool _playerInCollider;
    private Transform _playerTransform;
    private Animator _animator;

    public Task Initialize()
    {
        if (!model) return Task.CompletedTask; // Если модели нет - нахуй
        _animator = GetComponent<Animator>();
        _playerTransform = GameObject.Find("Mark").transform;

        SelectedStyle = NpcEntity.styles[0].nameOfStyle;
        ChangeStyle(SelectedStyle);
        DayProcess.Instance.EveryHourAction += UpdateHourSchedule;
        return Task.CompletedTask;
    }

#if UNITY_EDITOR

    [ContextMenu("FirstStyle")]
    public void ChangeFirstStyle() => ChangeStyle(NpcEntity.styles[0].nameOfStyle);

    [ContextMenu("SecondStyle")]
    public void ChangeSecondStyle() => ChangeStyle(NpcEntity.styles[1].nameOfStyle);
#endif

    public void ActivateAnimatorState(string state) => _animator.Play(state);

    public void ChangeStyle(string newStyle)
    {
        SelectedStyle = newStyle;
        foreach (Transform child in model.transform)
        {
            SpriteResolver sr = child.GetComponent<SpriteResolver>();
            if (sr)
                sr.SetCategoryAndLabel(SelectedStyle, sr.GetLabel());
        }
    }

    public void MoveTo(Transform target) => NpcManager.Instance.MoveTo(target, speed, transform, model, _animator);

    public async Task GenerateSchedule()
    {
        try
        {
            scheduleHourDict = new Dictionary<int, ScheduleCase>();
            for (int h = 0; h < 24; h++)
            {
                if (scheduleHourDict.ContainsKey(h)) continue; // Если уже добавлено

                ScheduleCase selectedScheduleCase = null;

                // Ставим объязательное действие
                foreach (ScheduleCase sc in NpcEntity.requireSchedule)
                {
                    if (sc.dayStart == DayProcess.Instance.Day && sc.hourStart == h)
                    {
                        selectedScheduleCase = sc;
                        continue;
                    }
                }

                // Ставим обычное действое
                if (selectedScheduleCase == null)
                {
                    // Если действие только в определенное время
                    foreach (ScheduleCase sc in NpcEntity.standardSchedule)
                    {
                        if (sc.hourStart != -1 && sc.hourStart == h)
                        {
                            selectedScheduleCase = sc;
                            continue;
                        }
                    }
                }

                // Если всё таки дело не нашлось - ищем рутину
                if (selectedScheduleCase == null)
                {
                    bool find = false;
                    while (!find)
                    {
                        selectedScheduleCase = NpcEntity.standardSchedule[Random.Range(0, NpcEntity.standardSchedule.Length)];
                        if (selectedScheduleCase.hourStart == -1)
                            find = true;
                    }
                }

                scheduleHourDict.TryAdd(h, selectedScheduleCase);

                // Если действие длится больше часа - ставим его на несколько часов
                for (int i = 1; i < (selectedScheduleCase.durationInMinutes / 60); i++)
                    scheduleHourDict.TryAdd(h + i, selectedScheduleCase);
            }

#if UNITY_EDITOR
        string n = NpcEntity.nameInWorld + " schedule " + DayProcess.Instance.Day + " day\n";
        for (int h = 0; h < 24; h++)
            n += "\n" + h + ":00 | " + scheduleHourDict[h].action.ToString();
        Debug.Log(n);
#endif
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Schedule generate error " + ex);
        }
    }

    private void UpdateHourSchedule() // Обновляем дело
    {
        try
        {
            if (totalScheduleCase == scheduleHourDict[DayProcess.Instance.Hour]) return; // Скипаем, если не изменилось

            totalScheduleCase = scheduleHourDict[DayProcess.Instance.Hour];

            point = GameObject.Find(totalScheduleCase.actionTargetName).transform;
            locationOfPointName = totalScheduleCase.actionTargetLocation;
            locationPointTarget = ManageLocation.Instance.GetLocation(locationOfPointName);

            Location currentLocation = ManageLocation.Instance.GetLocation(totalLocation);

            FindPathToTarget(currentLocation);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Npc {NpcEntity?.nameInWorld} {DayProcess.Instance.Hour}:{DayProcess.Instance.Minute} error: {ex}");
        }
    }

    private void FindPathToTarget(Location currentLocation)
    {
        var visitedLocations = new HashSet<Location>();
        var path = new List<Location>();

        if (FindPath(currentLocation, locationPointTarget, visitedLocations, path))
        {
#if UNITY_EDITOR
            string pathLog = $"{NpcEntity.nameInWorld} way: {string.Join(" -> ", path.Select(l => l.gameName))}";
            Debug.Log(pathLog);
#endif

            pointWay = path;
            totalWay = 1; // Чтобы не с той локи, где начало
            moveToPoint = true;
            GenerateTotalPoint();
        }
    }

    private bool FindPath(Location current, Location target, HashSet<Location> visited, List<Location> path)
    {
        if (current == target)
        {
            path.Add(current);
            return true;
        }

        if (visited.Contains(current)) return false;
        visited.Add(current);

        foreach (var neighbor in current.GetAllGatesLocations())
        {
            if (neighbor == null || neighbor == current) continue;

            if (FindPath(neighbor, target, visited, path))
            {
                path.Insert(0, current);
                return true;
            }
        }

        return false;
    }

    private void GenerateTotalPoint()
    {
        if (pointWay == null || totalWay >= pointWay.Count) return;

        Location currentLoc = ManageLocation.Instance.GetLocation(totalLocation);
        if (currentLoc == null) return;

        Location nextLoc = pointWay[totalWay];

        foreach (var interaction in currentLoc.LocationInteractableObjects.Values)
        {
            if (interaction is LocationInteraction locationInteraction &&
                ManageLocation.Instance.GetLocation(locationInteraction.locationName) == nextLoc)
            {
                totalPoint = locationInteraction.transform;
                totalSpawn = currentLoc.GetSpawn(nextLoc);
                return;
            }
        }
    }

    private void MoveToLocation(string locationName, Transform spawn = null)
    {
        transform.position = spawn ? spawn.position : ManageLocation.Instance.GetLocation(locationName).spawns[0].spawn.position;
        totalLocation = locationName;
    }

    private void FixedUpdate() // Movement
    {
        if (!_animator || !model || (DayProcess.Instance.StopTime && !timeImmunity)) return;

        Collider[] colliders = Physics.OverlapSphere(model.transform.position, 3.5f);

        _playerInCollider = false;

        // Чекаем все коллайдеры
        foreach (Collider collider in colliders)
        {
            switch (collider.gameObject.tag)
            {
                case "floorChange" when totalLocation != locationOfPointName && moveToPoint: // Если на другой этаж
                    MoveToLocation(locationOfPointName);
                    break;
                case "Player": // Если рядом с игроком
                    _playerInCollider = true;
                    break;
                default: // Другое хз
                    {
                        if (totalPoint != null && totalPoint.gameObject == collider.gameObject)
                        {
                            MoveToLocation(pointWay[totalWay].gameName, totalSpawn);
                            totalWay++;
                            GenerateTotalPoint();
                        }
                        if (point != null && collider.gameObject.name == point.gameObject.name) // Мы дошли до точки
                        {
                            moveToPoint = false;
                            model.transform.localScale = new Vector3(1, 1, 1);

                            if (totalScheduleCase.stayOnPoint)
                                transform.position = new Vector3(point.position.x, transform.position.y, point.position.z);

                            if (totalScheduleCase.startAnimationInPoint != "")
                                _animator.Play(totalScheduleCase.startAnimationInPoint);

                            if (totalScheduleCase.changeStyle != null)
                                ChangeStyle(totalScheduleCase.changeStyle);
                            totalScheduleCase.fastChangesController?.ActivateChanges();
                        }
                        break;
                    }
            }
        }

        if (!_playerInCollider && moveToPlayer)
            MoveTo(_playerTransform);
        else if (moveToPoint) // Двигаемся к точке
        {
            if (!waitPlayerAndMove || (waitPlayerAndMove && _playerInCollider))
                MoveTo(locationOfPointName == totalLocation ? point : totalPoint);
            else
                _animator?.SetBool("walk", false);
        }
        else
            _animator?.SetBool("walk", false);
    }
}