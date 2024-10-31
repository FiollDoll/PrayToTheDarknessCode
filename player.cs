using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(Rigidbody2D))]
public class player : MonoBehaviour
{
    public bool canMove;
    public GameObject playerMenu;
    [SerializeField] private float moveSpeed;
    [HideInInspector] public float changeSpeed;
    private Animator animator;
    public CinemachineVirtualCamera virtualCamera;
    [SerializeField] private allScripts scripts;
    private Rigidbody2D rb;

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
        if (page == 0) // Инв
            scripts.inventory.ManageInventoryPanel(true);
        else if (page == 1)
            scripts.notebook.ChoicePage(1);
        else if (page == 2)
            scripts.notebook.ChoicePage(0);
    }

    private void FixedUpdate()
    {
        float horiz = Input.GetAxis("Horizontal");
        float vert = Input.GetAxis("Vertical");

        if (canMove)
        {
            rb.velocity = new Vector2(horiz * (moveSpeed + changeSpeed), vert * (moveSpeed + changeSpeed));
            if (horiz != 0 || vert != 0)
                animator.SetBool("walk", true);
            else
                animator.SetBool("walk", false);
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
