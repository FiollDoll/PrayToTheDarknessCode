using System;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Player : MonoBehaviour, IHumanable
{
    public static Player Instance { get; private set; }
    private static readonly int s_Walk = Animator.StringToHash("walk");
    public Npc npcEntity { get; set; }
    public string selectedStyle { get; set; } = "standard";

    [Header("Характеристики")] public bool canMove;
    [SerializeField] private float moveSpeed;
    [HideInInspector] public float changeSpeed;
    [HideInInspector] public List<Npc> familiarNpc = new List<Npc>();

    [Header("Настройки")] [SerializeField] private Transform rayStart;
    [SerializeField] private LayerMask layerMaskInteractAuto;
    public CinemachineVirtualCamera virtualCamera;

    private Collider _enteredCollider;
    private Rigidbody _rb;
    private SpriteRenderer _sr;
    private Animator _animator;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _sr = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        ChangeStyle("sleepy");
    }

    public void ChangeStyle(string newStyle)
    {
        selectedStyle = newStyle;
        _animator.Play(npcEntity.GetNpcStyle(selectedStyle).animatorStyleName);
    }

    public void MoveTo(Transform target) =>
        Main.Instance.MoveTo(target, moveSpeed, transform, _sr, _animator);

    private void FixedUpdate()
    {
        float horiz = Input.GetAxis("Horizontal");
        float vert = Input.GetAxis("Vertical");

        if (canMove)
        {
            _rb.linearVelocity = new Vector3(horiz * (moveSpeed + changeSpeed), 0, vert * (moveSpeed + changeSpeed));
            _animator.SetBool(s_Walk, horiz != 0 || vert != 0);
            if (horiz > 0)
                _sr.flipX = true;
            else if (horiz < 0)
                _sr.flipX = false;
        }
        else
        {
            _rb.linearVelocity = Vector3.zero;
            _animator.SetBool("walk", false);
        }

        var newColliders = Physics.OverlapSphere(rayStart.position, 0.5f,
            layerMaskInteractAuto);

        if (newColliders.Length > 0)
        {
            _enteredCollider = newColliders[0];
            if (_enteredCollider.GetComponent<Collider>().TryGetComponent(out IInteractable interactable))
            {
                Interactions.Instance.EnteredInteraction = interactable;
                if (Interactions.Instance.CanActivateInteraction(interactable) && interactable.autoUse)
                    interactable.DoInteraction();
            }
        }
    }
}