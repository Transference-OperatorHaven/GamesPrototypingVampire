using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class TestMovement : MonoBehaviour
{
    // Start is called before the first frame update


    private Vampire vampireControls;
    Vector2 playerMove;
    bool hasEnemyGrabbed;
    public float playerSpeed;
    public float rotationSpeed;
    Rigidbody2D rb;


    private void Awake()
    {
        vampireControls = new Vampire();
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        vampireControls.Enable();
        vampireControls.Player.Button.canceled += Button;
    }

    private void OnDisable()
    {
        vampireControls.Player.Button.canceled -= Button;
        vampireControls.Disable();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        playerMove = vampireControls.Player.Move.ReadValue<Vector2>();
        rb.AddRelativeForce(Vector2.up * (playerMove.y * playerSpeed));
        Rotate();
    }
    void Rotate()
    {
        rb.MoveRotation(transform.rotation * Quaternion.Euler(0,0, -playerMove.x * rotationSpeed * Time.deltaTime));
    }

    public void Button(InputAction.CallbackContext obj)
    {
        if(!hasEnemyGrabbed) //if currently not grabbing an enemy
        {
            if(playerMove.y > 0) // if moving forward lunge attack
            {
                Debug.Log("lunge attack");
            }
            else if(playerMove.y < 0) // if moving backward dashback
            {
                Debug.Log("Dashback");
            }
            else //else do simple grab
            {
                Debug.Log("Grab");
            }
        }
        else
        {
            Debug.Log("keep hold of enemy attempt");
        }
    }
    
}
