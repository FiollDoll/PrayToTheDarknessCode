using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class NpcController : MonoBehaviour, IHumanable
{
    [Header("MainInfo")] public Npc NpcEntity { get; set; }
    public string SelectedStyle { get; set; }
    public string totalLocation;

    [Header("Preference")] [SerializeField]
    private float speed;

    [SerializeField] private GameObject model;

    private bool _playerInCollider;
    private Transform _playerTransform;
    private Animator _animator;

    [Header("Move To Player Preference")] public bool moveToPlayer;

    [Header("Move To Point Preference")] public bool moveToPoint;
    public bool waitPlayerAndMove;
    [HideInInspector] public Transform point;
    [HideInInspector] public string locationOfPointName;

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

    private void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.tag)
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
                if (point != null && other.gameObject.name == point.gameObject.name)
                    moveToPoint = false;
                break;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
            _playerInCollider = false;
    }

    // Переделать под корутины
    private void FixedUpdate()
    {
        if (!_animator || !model) return;

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