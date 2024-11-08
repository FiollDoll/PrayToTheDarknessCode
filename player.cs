using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class player : MonoBehaviour
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
    [SerializeField] private allScripts scripts;
    private Rigidbody2D rb;
    private Animator animator;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        ChangeVisual(3);
        ChoicePagePlayerMenu(0);
    }

    public void ChangeVisual(int num)
    {
        foreach (string style in playerVisuals)
            animator.SetBool(style, false);

        animator.SetBool(playerVisuals[num], true);
    }

    public void MoveTo(Transform target) => scripts.main.MoveTo(target, moveSpeed, transform, GetComponent<SpriteRenderer>(), animator);

    public void ChoicePagePlayerMenu(int page)
    {
        foreach (RectTransform rt in buttonsPlayerMenu)
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, 189.4f);
        buttonsPlayerMenu[page].anchoredPosition = new Vector2(buttonsPlayerMenu[page].anchoredPosition.x, 195f);

        scripts.notebook.ChoicePage(-1);
        scripts.inventory.ManageInventoryPanel(false);
        if (page == 0)
            scripts.inventory.ManageInventoryPanel(true);
        else if (page == 1)
            scripts.notebook.ChoicePage(1);
        else if (page == 2)
            scripts.notebook.ChoicePage(0);
        else if (page == 3)
            scripts.notebook.ChoicePage(2);
    }

    private void FixedUpdate()
    {
        float horiz = Input.GetAxis("Horizontal");
        float vert = Input.GetAxis("Vertical");

        if (canMove)
        {
            rb.velocity = new Vector2(horiz * (moveSpeed + changeSpeed), vert * (moveSpeed + changeSpeed));
            animator.SetBool("walk", horiz != 0 || vert != 0);
            if (horiz > 0)
                GetComponent<SpriteRenderer>().flipX = false;
            else if (horiz < 0)
                GetComponent<SpriteRenderer>().flipX = true;
        }
        else
        {
            rb.velocity = Vector2.zero;
            animator.SetBool("walk", false);
        }
    }
}
