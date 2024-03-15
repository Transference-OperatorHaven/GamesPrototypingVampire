using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class TestMovement : MonoBehaviour
{
    // Start is called before the first frame update
    public enum PlayerState
    {
        NotAttacking,
        Lunging,
        Grabbing,
        Dashing
    }

    private Vampire vampireControls;
    PlayerState playerState;
    Vector2 playerMove;
    bool hasEnemyGrabbed;
    [SerializeField]bool canAttack = true;
    public float playerSpeed;
    public float playerLunge;
    public float playerGrab;
    public float grabDistance;
    public float lungeDistance;
    public float rotationSpeed;
    public float playerDashback;
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
    void FixedUpdate()
    {
        if(playerState == PlayerState.NotAttacking)
        {
            playerMove = vampireControls.Player.Move.ReadValue<Vector2>();
            rb.AddRelativeForce(Vector2.up * (playerMove.y * playerSpeed));
            Rotate();
        }
        
    }
    void Rotate()
    {
        rb.MoveRotation(transform.rotation * Quaternion.Euler(0,0, -playerMove.x * rotationSpeed * Time.deltaTime));
    }

    public void Button(InputAction.CallbackContext obj)
    {
        if(canAttack)
        {
            if (!hasEnemyGrabbed) //if currently not grabbing an enemy
            {
                if (playerMove.y > 0) // if moving forward lunge attack
                {
                    playerState = PlayerState.Lunging;
                    Debug.Log("lunge attack");
                    RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.up, lungeDistance);
                    if(hit.transform.tag == "Enemy")
                    {
                        hasEnemyGrabbed = true;
                    }
                    rb.AddRelativeForce(Vector2.up * playerLunge);
                    StartCoroutine(DisableMovement(0.2f));
                }
                else if (playerMove.y < 0) // if moving backward dashback
                {
                    Debug.Log("Dashback");
                    playerState = PlayerState.Dashing;
                    rb.AddRelativeForce(Vector2.down * playerDashback);
                    StartCoroutine(DisableMovement(0.1f));
                }
                else //else do simple grab
                {
                    Debug.Log("Grab");
                    RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, grabDistance);
                    if (hit.transform.tag == "Enemy")
                    {
                        hasEnemyGrabbed = true;
                    }
                    playerState = PlayerState.Grabbing;
                    rb.AddRelativeForce(Vector2.up * playerGrab);
                    StartCoroutine(DisableMovement(0.3f));
                }
            }
            else
            {
                Debug.Log("keep hold of enemy attempt");
            }

        }
        
    }
    
    IEnumerator DisableMovement(float timer)
    {
        yield return new WaitForSeconds(timer);
        playerState = PlayerState.NotAttacking;
        StartCoroutine(CooldownAttack());
    }

    IEnumerator CooldownAttack()
    {
        float t = 0;
        while(t < 2f)
        {
            t += Time.deltaTime;
            canAttack = false;
            yield return null;
        }

        canAttack = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, transform.up * lungeDistance);
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.up * grabDistance);
    }

}
