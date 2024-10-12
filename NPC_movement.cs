using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_movement : MonoBehaviour
{
    public string totalLocation;
    [SerializeField] private float speed;
    [SerializeField] private allScripts scripts;
    private bool playerInCollider;
    private Transform playerTransform;
    private SpriteRenderer sr;
    private Animator animator;

    [Header("Move to player")]
    public bool moveToPlayer;

    [Header("Move to point")]
    public bool moveToPoint;
    public bool waitPlayerAndMove;
    public Transform point;
    public string locationOfPointName;

    private void Start()
    {
        playerTransform = GameObject.Find("Player").transform;
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "floorChange" && totalLocation != locationOfPointName && moveToPoint)
        {
            transform.position = scripts.locations.GetLocation(locationOfPointName).spawns[0].spawn.position;
            totalLocation = locationOfPointName;
        }
        else if (other.gameObject.name == "Player")
            playerInCollider = true;
        else if (other.gameObject.name == point.gameObject.name)
            moveToPoint = false;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.name == "Player")
            playerInCollider = false;
    }

    private void MoveTo(Transform target) => scripts.main.MoveTo(target, speed, transform, sr, animator);


    private void Update()
    {
        if (!playerInCollider && moveToPlayer)
            MoveTo(playerTransform);
        else if (moveToPoint)
        {
            if (!waitPlayerAndMove || (waitPlayerAndMove && playerInCollider))
            {
                if (locationOfPointName == totalLocation)
                    MoveTo(point);
                else
                    MoveTo(scripts.locations.GetLocation(totalLocation).transformOfStairs);
            }
            else
                GetComponent<Animator>().SetBool("walk", false);
        }
        else
            GetComponent<Animator>().SetBool("walk", false);
    }
}