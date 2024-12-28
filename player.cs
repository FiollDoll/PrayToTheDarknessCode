using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Player : MonoBehaviour
{
    [Header("Характеристики")] 
    public bool canMove;
    [SerializeField] private float moveSpeed;
    [HideInInspector] public float changeSpeed;
    [SerializeField] private string[] playerVisuals = new string[0];
    [HideInInspector] public List<NPC> familiarNpc = new List<NPC>();

    [Header("Настройки")] 
    [SerializeField] private Transform rayStart;
    [SerializeField] private LayerMask layerMaskInteractAuto;
    public CinemachineVirtualCamera virtualCamera;

    [Header("Игровое меню")] 
    public GameObject playerMenu;
    [SerializeField] private RectTransform[] buttonsPlayerMenu = new RectTransform[0];
    
    private AllScripts _scripts;
    private RaycastHit _enteredCollider;
    private Rigidbody _rb;
    private SpriteRenderer _sr;
    private Animator _animator;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _sr = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _scripts = GameObject.Find("scripts").GetComponent<AllScripts>();
        _scripts.player = this;
        ChangeVisual(1);
        ChoicePagePlayerMenu(0);
    }

    public void ChangeVisual(int num)
    {
        foreach (string style in playerVisuals)
            _animator.SetBool(style, false);

        _animator.SetBool(playerVisuals[num], true);
    }

    public void MoveTo(Transform target) =>
        _scripts.main.MoveTo(target, moveSpeed, transform, _sr, _animator);

    private void ChoicePagePlayerMenu(int page)
    {
        foreach (RectTransform rt in buttonsPlayerMenu)
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, 189.4f);
        buttonsPlayerMenu[page].anchoredPosition = new Vector2(buttonsPlayerMenu[page].anchoredPosition.x, 195f);

        _scripts.notebook.ChoicePage(-1);
        _scripts.inventory.ManageInventoryPanel(false);
        switch (page)
        {
            case 0:
                _scripts.inventory.ManageInventoryPanel(true);
                break;
            case 1:
                _scripts.notebook.ChoicePage(1);
                break;
            case 2:
                _scripts.notebook.ChoicePage(0);
                break;
            case 3:
                _scripts.notebook.ChoicePage(2);
                break;
        }
    }


    private void FixedUpdate()
    {
        float horiz = Input.GetAxis("Horizontal");
        float vert = Input.GetAxis("Vertical");

        _scripts.interactions.ClearEnteredCollider();
        
        if (Physics.Raycast(rayStart.position, Vector3.up, out _enteredCollider, 12f, layerMaskInteractAuto) &&
            !_scripts.interactions.lockInter)
            _scripts.interactions.ActivateInteractionAuto(_enteredCollider);

        if (canMove)
        {
            _rb.linearVelocity = new Vector3(horiz * (moveSpeed + changeSpeed), 0, vert * (moveSpeed + changeSpeed));
            _animator.SetBool("walk", horiz != 0 || vert != 0);
            if (horiz > 0)
                _sr.flipX = false;
            else if (horiz < 0)
                _sr.flipX = true;
        }
        else
        {
            _rb.linearVelocity = Vector3.zero;
            _animator.SetBool("walk", false);
        }
    }
}