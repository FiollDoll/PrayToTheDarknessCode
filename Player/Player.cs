using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Player : MonoBehaviour, IHumanable
{
    public Npc npcEntity { get; set; }
    public string selectedStyle { get; set; } = "standard";

    [Header("Характеристики")] public bool canMove;
    [SerializeField] private float moveSpeed;
    [HideInInspector] public float changeSpeed;
    [SerializeField] private string[] playerVisuals = new string[0];
    [HideInInspector] public List<Npc> familiarNpc = new List<Npc>();

    [Header("Настройки")] [SerializeField] private Transform rayStart;
    [SerializeField] private LayerMask layerMaskInteractAuto;
    public CinemachineVirtualCamera virtualCamera;

    [Header("Игровое меню")] public GameObject playerMenu;
    [SerializeField] private RectTransform[] buttonsPlayerMenu = new RectTransform[0];

    private AllScripts _scripts;
    private RaycastHit _enteredCollider;
    private Rigidbody _rb;
    private SpriteRenderer _sr;
    private Animator _animator;

    public void Initialize()
    {
        _rb = GetComponent<Rigidbody>();
        _sr = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _scripts = GameObject.Find("scripts").GetComponent<AllScripts>();
        ChangeStyle("standard");
        ChoicePagePlayerMenu(0);
    }

    public void ChangeStyle(string newStyle)
    {
        selectedStyle = newStyle;
        Debug.Log(npcEntity.nameOfNpc);
        Debug.Log(npcEntity.GetNpcStyle(selectedStyle).nameOfStyle);
        _animator.Play(npcEntity.GetNpcStyle(selectedStyle).animatorStyleName);
    }
    
    public void MoveTo(Transform target) =>
        _scripts.main.MoveTo(target, moveSpeed, transform, _sr, _animator);

    private void ChoicePagePlayerMenu(int page)
    {
        foreach (RectTransform rt in buttonsPlayerMenu)
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, 189.4f);
        buttonsPlayerMenu[page].anchoredPosition = new Vector2(buttonsPlayerMenu[page].anchoredPosition.x, 195f);

        _scripts.notebook.ChoicePage(-1);
        _scripts.inventoryManager.ManageInventoryPanel(false);
        switch (page)
        {
            case 0:
                _scripts.inventoryManager.ManageInventoryPanel(true);
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
        
        if (Physics.Raycast(rayStart.position, Vector3.up, out _enteredCollider, 12f,
                layerMaskInteractAuto) && !_scripts.interactions.lockInter)
        {
            if (_enteredCollider.collider.TryGetComponent(out IInteractable interactable))
            {
                _scripts.interactions.EnteredInteraction = interactable;
                if (_scripts.interactions.CheckActiveInteraction(interactable) && interactable.autoUse)
                    interactable.DoInteraction();
            }
        }

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