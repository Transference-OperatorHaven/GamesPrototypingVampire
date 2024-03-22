using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHunter : MonoBehaviour
{
    public Vector3[] PathingPos;
    public int currentEnd;
    public float speed;
    public bool isGrabbed;
    public bool firstTimeGrab;
    public Transform grabbedTransform;
    BoxCollider2D bC;

    // Start is called before the first frame update
    void Start()
    {
        bC = GetComponent<BoxCollider2D>();
        currentEnd = 0;
    }

    private void FixedUpdate()
    {
       
        if( !isGrabbed )
        {
            bC.enabled = true;
            if (transform.position != PathingPos[currentEnd])
            {
                transform.position = Vector2.MoveTowards(transform.position, PathingPos[currentEnd], speed * Time.deltaTime);
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
            if(firstTimeGrab)
            {
                grabbedTransform.GetComponentInParent<TestMovement>().HunterTrigger();
            }
            bC.enabled = false;
            transform.position = grabbedTransform.position;
        }
        
    }

    



}
