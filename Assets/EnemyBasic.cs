using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBasic : MonoBehaviour
{
    public Vector3[] PathingPos;
    public int currentEnd;
    public float speed;
    public bool isGrabbed;
    public int Health = 3;
    public Transform grabbedTransform;
    BoxCollider2D bC;
    Rigidbody2D rb;
    EnemyTracker tracker;

    // Start is called before the first frame update
    void Start()
    {

        tracker = FindObjectOfType<EnemyTracker>();
        bC = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        currentEnd = 0;
        transform.position = PathingPos[0];
    }

    private void FixedUpdate()
    {
       if(Health <= 0)
        {
            tracker.ReduceEnemyCount();
            Destroy(gameObject); return;
        }
        if( !isGrabbed )
        {
            bC.enabled = true;
            if (transform.position != PathingPos[currentEnd])
            {
                transform.position = Vector2.MoveTowards(transform.position, PathingPos[currentEnd], speed * Time.deltaTime);
            }
            else if (transform.position != PathingPos[currentEnd] && Mathf.Abs(Vector3.Distance(transform.position, PathingPos[currentEnd])) < 0.25)
            {
                transform.position = PathingPos[currentEnd];
            }
            else
            {
                currentEnd++;
                if (currentEnd >= PathingPos.Length)
                {
                    currentEnd = 0;
                }

            }
        }

        else
        {
            bC.enabled = false;
            transform.position = grabbedTransform.position;
        }
        
    }

    



}
