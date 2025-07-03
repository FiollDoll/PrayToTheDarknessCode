using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;
using UnityEngine.U2D.Animation;

public class Player : MonoBehaviour, IHumanable
{
    public static Player Instance { get; private set; }

    public Npc NpcEntity { get; set; }
    public string SelectedStyle { get; set; } = "standard";

    [Header("Stats")] public PlayerStats PlayerStats;
    public bool canMove;
    public bool blockMoveZ;
    private bool _run;

    [Header("Preferences")] [SerializeField]
    private GameObject model;

    public CinemachineVirtualCamera virtualCamera;

    private Rigidbody _rb;
    private Animator _animator;
    private Vector2 _moveDirection;

    private void Awake() => Instance = this;

    public Task Initialize()
    {
        _rb = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        PlayerStats = new PlayerStats(); // Временно, пока не работает система сохранений
        return Task.CompletedTask;
    }

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

    public void MoveTo(Transform target) =>
        NpcManager.Instance.MoveTo(target, PlayerStats.MoveSpeed, transform, model, _animator);


    public void OnMove(InputAction.CallbackContext context) => _moveDirection = context.ReadValue<Vector2>();

    public void OnRun(InputAction.CallbackContext context) => _run = context.action.IsPressed();

    private void Move(Vector2 direction)
    {
        if (canMove)
        {
            float runChange = 0f;
            if (_run)
                runChange = 2.5f;

            float totalChange = PlayerStats.MoveSpeed + PlayerStats.ChangeSpeed + runChange;
            _rb.linearVelocity = new Vector3(direction.x * totalChange, 0, direction.y * totalChange);
            _animator.SetBool("walk", !_run && (direction.x != 0 || direction.y != 0));
            _animator.SetBool("run", _run && (direction.x != 0 || direction.y != 0));

            model.transform.localScale = direction.x switch
            {
                > 0 => new Vector3(-1, 1, 1),
                < 0 => new Vector3(1, 1, 1),
                _ => model.transform.localScale
            };
            return;
        }

        _rb.linearVelocity = Vector3.zero;
        _animator.SetBool("walk", false);
        _animator.SetBool("run", false);
    }


    private void Update()
    {
        Move(_moveDirection);
    }
}