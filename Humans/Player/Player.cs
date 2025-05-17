using System;
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
    private bool _run;
    public bool blockMoveZ;
    [SerializeField] private float moveSpeed;
    [HideInInspector] public float changeSpeed;
    [HideInInspector] public List<Npc> familiarNpc = new List<Npc>();

    [Header("Preferences")] [SerializeField] private Transform rayStart;
    [SerializeField] private GameObject model;
    [SerializeField] private LayerMask layerMaskInteractAuto;
    public CinemachineVirtualCamera virtualCamera;

    private Collider _enteredCollider;
    private Rigidbody _rb;
    private Animator _animator;
    private bool _hasInteracted;

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
                    new Vector3(horiz * (moveSpeed + changeSpeed + 2), 0, vert * (moveSpeed + changeSpeed + 2));
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

        var newColliders = Physics.OverlapSphere(rayStart.position, 0.5f, layerMaskInteractAuto);

        if (newColliders.Length > 0)
        {
            _enteredCollider = newColliders[0];
            if (_enteredCollider.GetComponent<Collider>().TryGetComponent(out IInteractable interactable))
            {
                Interactions.Instance.EnteredInteraction = interactable;
                if (Interactions.Instance.CanActivateInteraction(interactable) && interactable.autoUse &&
                    !_hasInteracted)
                {
                    interactable.DoInteraction();
                    _hasInteracted = true;
                }
            }
        }
        else
            _hasInteracted = false;
    }
}