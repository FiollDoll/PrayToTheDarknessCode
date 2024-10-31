using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class player : MonoBehaviour
{
    public float karma;
    [SerializeField] private float moveSpeed;
    [HideInInspector] public float changeSpeed;
    public bool canMove;
    public GameObject playerMenu;
    public CinemachineVirtualCamera virtualCamera;
    public List<NPC> familiarNPC = new List<NPC>();
    [SerializeField] private allScripts scripts;
    private Rigidbody2D rb;
    private Animator animator;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        ChangeVisual(1);
    }

    public void ChangeVisual(int num)
    {
        animator.SetBool("noClothStyle", false);
        animator.SetBool("standartStyle", false);
        if (num == 0)
            animator.SetBool("standartStyle", true);
        else
            animator.SetBool("noClothStyle", true);
    }

    public void MoveTo(Transform target) => scripts.main.MoveTo(target, moveSpeed, transform, GetComponent<SpriteRenderer>(), animator);

    public void ChoicePagePlayerMenu(int page)
    {
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
