using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class NpcController : MonoBehaviour, IHumanable
{
    [Header("MainInfo")] public Npc NpcEntity { get; set; }
    public string SelectedStyle { get; set; }
    public string totalLocation;

    [Header("Schedule")]
    public Dictionary<int, ScheduleCase> scheduleHourDict = new Dictionary<int, ScheduleCase>();

    [Header("Preference")]
    [SerializeField]
    private float speed;

    [SerializeField] private GameObject model;

    [Header("Move To Player Preference")] public bool moveToPlayer;

    [Header("Move To Point Preference")] public bool moveToPoint;
    public bool waitPlayerAndMove;
    [HideInInspector] public Transform point;
    [HideInInspector] public string locationOfPointName;

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

        _animator?.Play(NpcEntity.GetNpcStyle(SelectedStyle).animatorStyleName);
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
                for (int i = 1; i < (selectedScheduleCase.durationInMinutes / 30); i++)
                    scheduleHourDict.TryAdd(h + i, selectedScheduleCase);
            }

#if UNITY_EDITOR
        string n = NpcEntity.nameInWorld + " schedule " + DayProcess.Instance.Day + " day\n";
        for (int h = 0; h < 25; h++)
            n += "\n" + h + ":00 | " + scheduleHourDict[h].action.ToString();
        Debug.Log(n);
#endif
        }
        catch (System.Exception ex)
        {
            Debug.Log("Schedule generate error " + ex);
        }
    }

    private void FixedUpdate()
    {
        if (!_animator || !model) return;

        Collider[] colliders = Physics.OverlapSphere(model.transform.position, 5f);

        _playerInCollider = false;

        foreach (Collider collider in colliders)
        {
            switch (collider.gameObject.tag)
            {
                case "floorChange" when totalLocation != locationOfPointName && moveToPoint:
                    ManageLocation.Instance.NpcAtTotalLocation.Remove(this);
                    transform.position = ManageLocation.Instance.GetLocation(locationOfPointName).spawns[0].spawn.position;
                    totalLocation = locationOfPointName;
                    ManageLocation.Instance.NpcAtTotalLocation.Add(this);
                    break;
                case "Player":
                    _playerInCollider = true;
                    break;
                default:
                    {
                        if (point != null && collider.gameObject.name == point.gameObject.name)
                            moveToPoint = false;
                        break;
                    }
            }
        }


        if (!_playerInCollider && moveToPlayer)
            MoveTo(_playerTransform);
        else if (moveToPoint)
        {
            if (!waitPlayerAndMove || (waitPlayerAndMove && _playerInCollider))
            {
                MoveTo(locationOfPointName == totalLocation
                    ? point
                    : ManageLocation.Instance.GetLocation(totalLocation).transformOfStairs);
            }
            else
                _animator?.SetBool("walk", false);
        }
        else
            _animator?.SetBool("walk", false);
    }
}