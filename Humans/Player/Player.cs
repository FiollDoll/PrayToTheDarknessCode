using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.U2D.Animation;

public class Player : MonoBehaviour, IHumanable
{
    public static Player Instance { get; private set; }
    public Npc npcEntity { get; set; }
    public string selectedStyle { get; set; } = "standard";

    [Header("Stats")] public bool canMove;
    public bool blockMoveZ;
    private bool _run;
    [SerializeField] private float moveSpeed;
    [HideInInspector] public float changeSpeed;
    [HideInInspector] public List<Npc> familiarNpc = new List<Npc>();

    [Header("Preferences")] [SerializeField]
    private GameObject model;
    public CinemachineVirtualCamera virtualCamera;

    private Rigidbody _rb;
    private Animator _animator;

    private void Awake() => Instance = this;

    public void Initialize()
    {
        _rb = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
    }

    public void ChangeStyle(string newStyle)
    {
        selectedStyle = newStyle;
        foreach (Transform child in model.transform)
        {
            SpriteResolver sr = child.GetComponent<SpriteResolver>();
            if (sr)
                sr.SetCategoryAndLabel(selectedStyle, sr.GetLabel());
        }
    }

    public void MoveTo(Transform target) =>
        NpcManager.Instance.MoveTo(target, moveSpeed, transform, model, _animator);

    private void Update()
    {
        float horiz = Input.GetAxis("Horizontal");
        float vert = 0f;
        if (!blockMoveZ)
            vert = Input.GetAxis("Vertical");

        if (Input.GetKeyDown(KeyCode.LeftShift))
            _run = true;
        if (Input.GetKeyUp(KeyCode.LeftShift))
            _run = false;

        if (canMove)
        {
            if (!_run)
            {
                _rb.linearVelocity =
                    new Vector3(horiz * (moveSpeed + changeSpeed), 0, vert * (moveSpeed + changeSpeed));
                _animator.SetBool("walk", horiz != 0 || vert != 0);
                _animator.SetBool("run", false);
            }
            else
            {
                _rb.linearVelocity =
                    new Vector3(horiz * (moveSpeed + changeSpeed + 2.5f), 0, vert * (moveSpeed + changeSpeed + 2.5f));
                _animator.SetBool("walk", false);
                _animator.SetBool("run", (horiz != 0 || vert != 0));
            }

            model.transform.localScale = horiz switch
            {
                > 0 => new Vector3(-1, 1, 1),
                < 0 => new Vector3(1, 1, 1),
                _ => model.transform.localScale
            };
        }
        else
        {
            _rb.linearVelocity = Vector3.zero;
            _animator.SetBool("walk", false);
        }
    }
}