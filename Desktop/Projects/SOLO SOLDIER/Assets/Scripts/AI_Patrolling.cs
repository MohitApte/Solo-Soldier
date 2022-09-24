using UnityEngine;
using System.Collections;

public class AI_Patrolling   : MonoBehaviour
{

    private bool shouldPatrol = true;
    Animator anim;
    Rigidbody rb;
    public Transform[] wayPoints;
    private int currentPoint = 0;
    public float patrolSpeed;
    public Transform Player;
    public float distance;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        wayPoints[0] = transform;
        transform.position = wayPoints[0].position;
    }

    void Update()
    {
        distance = Vector3.Distance(Player.position, transform.position);
        Debug.Log(distance);

        if (distance <= 45 && distance > 25)
        {
            shouldPatrol = false;
            anim.SetBool("isWalking", true);
            transform.position = Vector3.MoveTowards(transform.position, Player.transform.position, 5 * Time.deltaTime);
            transform.LookAt(Player);
        }
        else if (distance <= 25)
        {
            anim.SetBool("isWalking", false);
            anim.SetBool("Aiming", true);
            transform.LookAt(Player);
        }
        else
        {
            anim.SetBool("Aiming", false);
            shouldPatrol = true;
        }

        if (rb.IsSleeping())
        {
            anim.SetBool("isWalking", false);
        }
        else
        {
            anim.SetBool("isWalking", true);
        }

        if (shouldPatrol == true)
        {
            if (transform.position == wayPoints[currentPoint].position)
            {

                currentPoint++;
                if (currentPoint >= wayPoints.Length)
                {

                    currentPoint = 0;

                }


            }

                transform.position = Vector3.MoveTowards(transform.position, wayPoints[currentPoint].transform.position, patrolSpeed * Time.deltaTime);
                transform.LookAt(wayPoints[currentPoint].transform);

         
        }
    }

}
