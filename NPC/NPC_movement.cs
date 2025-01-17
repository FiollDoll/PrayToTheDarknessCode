using UnityEngine;

public class NPC_movement : MonoBehaviour
{
    public string totalLocation;
    [SerializeField] private float speed;
    [SerializeField] private AllScripts scripts;
    private bool _playerInCollider;
    private Transform _playerTransform;
    private SpriteRenderer _sr;
    [HideInInspector] public Animator animator;

    [Header("Move to player")] public bool moveToPlayer;

    [Header("Move to point")] public bool moveToPoint;
    public bool waitPlayerAndMove;
    public Transform point;
    public string locationOfPointName;

    private void Start()
    {
        _playerTransform = GameObject.Find("Player").transform;
        _sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        switch (other.gameObject.name)
        {
            case "floorChange" when totalLocation != locationOfPointName && moveToPoint:
                transform.position = scripts.manageLocation.GetLocation(locationOfPointName).spawns[0].spawn.position;
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

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.name == "Player")
            _playerInCollider = false;
    }

    private void MoveTo(Transform target) => scripts.main.MoveTo(target, speed, transform, _sr, animator);

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
                    : scripts.manageLocation.GetLocation(totalLocation).transformOfStairs);
            }
            else
                animator?.SetBool("walk", false);
        }
        else
            animator?.SetBool("walk", false);
    }
}