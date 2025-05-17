using UnityEngine;

public class NpcController : MonoBehaviour, IHumanable
{
    [Header("MainInfo")] public Npc npcEntity { get; set; }
    public string selectedStyle { get; set; } = "standard";
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

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _playerTransform = GameObject.Find("Mark").transform;
    }
    
    public void ChangeStyle(string newStyle)
    {
        selectedStyle = newStyle;
        _animator.Play(npcEntity.GetNpcStyle(selectedStyle).animatorStyleName);
    }

    public void MoveTo(Transform target) => NpcManager.Instance.MoveTo(target, speed, transform, model, _animator);

    private void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "floorChange" when totalLocation != locationOfPointName && moveToPoint:
                transform.position = ManageLocation.Instance.GetLocation(locationOfPointName).spawns[0].spawn.position;
                totalLocation = locationOfPointName;
                break;
            case "Player":
                _playerInCollider = true;
                break;
            default:
            {
                if (point != null & other.gameObject.name == point.gameObject.name)
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