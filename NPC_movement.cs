using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_movement : MonoBehaviour
{
    public bool moveToPlayer;
    [SerializeField] private float speed;
    private bool playerInCollider;
    private Transform playerTransform;

    private void Start() => playerTransform = GameObject.Find("Player").transform;


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "Player")
            playerInCollider = true;
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.name == "Player")
            playerInCollider = false;
    }

    private void Update()
    {
        if (!playerInCollider && moveToPlayer)
        {
            transform.position = Vector3.MoveTowards(transform.position, playerTransform.position, speed * Time.deltaTime);
            if (playerTransform.position.x > transform.position.x)
                GetComponent<SpriteRenderer>().flipX = false;
            else
                GetComponent<SpriteRenderer>().flipX = true;
        }
    }
}
