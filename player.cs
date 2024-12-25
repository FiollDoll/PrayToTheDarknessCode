using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class Player : MonoBehaviour
{
    public float karma;
    [SerializeField] private float moveSpeed;
    [HideInInspector] public float changeSpeed;
    public bool canMove;
    [SerializeField] private string[] playerVisuals = new string[0];
    public GameObject playerMenu;
    [SerializeField] private RectTransform[] buttonsPlayerMenu = new RectTransform[0];
    public CinemachineVirtualCamera virtualCamera;
    public List<NPC> familiarNPC = new List<NPC>();
    [SerializeField] private AllScripts scripts;
    private Rigidbody2D _rb;
    private Animator _animator;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        ChangeVisual(1); 
        ChoicePagePlayerMenu(0);
    }

    public void ChangeVisual(int num)
    {
        foreach (string style in playerVisuals)
            _animator.SetBool(style, false);

        _animator.SetBool(playerVisuals[num], true);
    }

    public void MoveTo(Transform target) => scripts.main.MoveTo(target, moveSpeed, transform, GetComponent<SpriteRenderer>(), _animator);

    private void ChoicePagePlayerMenu(int page)
    {
        foreach (RectTransform rt in buttonsPlayerMenu)
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, 189.4f);
        buttonsPlayerMenu[page].anchoredPosition = new Vector2(buttonsPlayerMenu[page].anchoredPosition.x, 195f);

        scripts.notebook.ChoicePage(-1);
        scripts.inventory.ManageInventoryPanel(false);
        switch (page)
        {
            case 0:
                scripts.inventory.ManageInventoryPanel(true);
                break;
            case 1:
                scripts.notebook.ChoicePage(1);
                break;
            case 2:
                scripts.notebook.ChoicePage(0);
                break;
            case 3:
                scripts.notebook.ChoicePage(2);
                break;
        }
    }

    private void FixedUpdate()
    {
        float horiz = Input.GetAxis("Horizontal");
        float vert = Input.GetAxis("Vertical");

        if (canMove)
        {
            _rb.linearVelocity = new Vector2(horiz * (moveSpeed + changeSpeed), vert * (moveSpeed + changeSpeed));
            _animator.SetBool("walk", horiz != 0 || vert != 0);
            if (horiz > 0)
                GetComponent<SpriteRenderer>().flipX = false;
            else if (horiz < 0)
                GetComponent<SpriteRenderer>().flipX = true;
        }
        else
        {
            _rb.linearVelocity = Vector2.zero;
            _animator.SetBool("walk", false);
        }
    }
}
