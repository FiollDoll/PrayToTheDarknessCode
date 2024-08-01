using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class player : MonoBehaviour
{
    private int visualMode;
    [SerializeField] private Sprite[] visualSprites = new Sprite[0];
    public bool canMove;
    [SerializeField] private float moveSpeed;
    [HideInInspector] public float changeSpeed;
    private Animator animator;

    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        ChangeVisual(1);
    }

    public void ChangeVisual(int num)
    {
        visualMode = num;
        GetComponent<SpriteRenderer>().sprite = visualSprites[num];
    }

    private void FixedUpdate()
    {
        float horiz = Input.GetAxis("Horizontal");
        float vert = Input.GetAxis("Vertical");

        if (canMove)
        {
            rb.velocity = new Vector2(horiz * (moveSpeed + changeSpeed), vert * (moveSpeed + changeSpeed));
            if (visualMode == 0)
            {
                if (horiz != 0 || vert != 0)
                    animator.SetBool("walk", true);
                else
                    animator.SetBool("walk", false);
            }
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
