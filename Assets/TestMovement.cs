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
    [SerializeField]bool hasEnemyGrabbed;
    [SerializeField]bool canAttack = true;
    bool hunterAttack, hunterQTEButtonPress;
    public float playerSpeed;
    public float playerLunge;
    public float playerGrab;
    public float grabDistance;
    public float lungeDistance;
    public float rotationSpeed;
    public float playerDashback;
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] Transform grabPos;
    Transform grabbedEnemy;
    Rigidbody2D rb;
    public SpriteRenderer armsSR;


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
        if(hunterAttack)
        {
            hunterQTEButtonPress = true; 
        }
        if (!canAttack) { return; }
        if (!hasEnemyGrabbed) //if currently not grabbing an enemy
        {
            if (playerMove.y > 0)
            {
                playerState = PlayerState.Lunging;
                armsSR.enabled = true;
                Debug.Log("lunge attack");
                RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.up, lungeDistance, enemyLayer);
                if (!hit) { return; }
                if (hit.transform.gameObject.CompareTag("Enemy"))
                {
                    hit.transform.GetComponent<EnemyBasic>().isGrabbed = true;
                    hit.transform.GetComponent<EnemyBasic>().grabbedTransform = grabPos;
                    hasEnemyGrabbed = true;
                }
                rb.AddRelativeForce(Vector2.up * playerLunge);
                StartCoroutine(DisableMovement(0.2f));
            } // if moving forward lunge attack
            else if (playerMove.y < 0)
            {
                Debug.Log("Dashback");
                playerState = PlayerState.Dashing;
                rb.AddRelativeForce(Vector2.down * playerDashback);
                StartCoroutine(DisableMovement(0.1f));
            }// if moving backward dashback
            else
            {
                armsSR.enabled = true;
                Debug.Log("Grab");
                RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, grabDistance, enemyLayer);
                if (!hit) { return; }
                if (hit.transform.gameObject.CompareTag("Enemy"))
                {
                    hit.transform.GetComponent<EnemyBasic>().isGrabbed = true;
                    hit.transform.GetComponent<EnemyBasic>().grabbedTransform = grabPos;
                    hasEnemyGrabbed = true;
                }
                playerState = PlayerState.Grabbing;
                rb.AddRelativeForce(Vector2.up * playerGrab);
                StartCoroutine(DisableMovement(0.3f));
            }//else do simple grab
        }
        else
        {
            Debug.Log("keep hold of enemy attempt");
        }
    }

    public void HunterTrigger()
    {
        StartCoroutine(HunterQTE());
    }

    IEnumerator HunterQTE()
    {
        float t = 0;
        hunterAttack = true;
        while (t < 0.333)
        {

            yield return null;
        }
        hunterAttack = false;
    }

    IEnumerator DisableMovement(float timer)
    {
        yield return new WaitForSeconds(timer);
        playerState = PlayerState.NotAttacking;
        StartCoroutine(CooldownAttack());
    }

    IEnumerator CooldownAttack()
    {
        armsSR.enabled = false;
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
