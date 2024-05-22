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
    [SerializeField]PlayerState playerState;
    Vector2 playerMove;
    [SerializeField]bool hasEnemyGrabbed;
    [SerializeField]bool canAttack = true;
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
        if (!canAttack) return;
        if (!hasEnemyGrabbed) //if currently not grabbing an enemy
        {
            if (playerMove.y > 0)
            {
                armsSR.enabled = true;
                Debug.Log("lunge");
                RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, lungeDistance, enemyLayer);
                if (hit && hit.transform.gameObject.CompareTag("Enemy"))
                    {
                        grabbedEnemy = hit.transform;
                        hit.transform.GetComponent<EnemyBasic>().isGrabbed = true;
                        hit.transform.GetComponent<EnemyBasic>().grabbedTransform = grabPos;
                        hasEnemyGrabbed = true;
                        playerState = PlayerState.Lunging;
                        rb.AddRelativeForce(Vector2.up * playerLunge);
                        DisableMovement(2f);
                        armsSR.enabled = false;
                    }
                playerState = PlayerState.Lunging;
                rb.AddRelativeForce(Vector2.up * playerLunge);
                DisableMovement(0.5f);
                armsSR.enabled = false;

            } // if moving forward lunge attack
            else if (playerMove.y < 0 && playerState != PlayerState.Dashing)
            {
                Debug.Log("Dashback");
                playerState = PlayerState.Dashing;
                rb.AddRelativeForce(Vector2.down * playerDashback);
                DisableMovement(0.75f);

            }// if moving backward dashback
            else
            {
                armsSR.enabled = true;
                Debug.Log("Grab");
                RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, grabDistance, enemyLayer);
                if (hit && hit.transform.gameObject.CompareTag("Enemy"))
                {
                    grabbedEnemy = hit.transform;
                    hit.transform.GetComponent<EnemyBasic>().isGrabbed = true;
                    hit.transform.GetComponent<EnemyBasic>().grabbedTransform = grabPos;
                    hasEnemyGrabbed = true;
                }
                playerState = PlayerState.Grabbing;
                rb.AddRelativeForce(Vector2.up * playerGrab);
                DisableMovement(0.5f);
                armsSR.enabled = false;
            }//else do simple grab
        }
        else
        {
            if (grabbedEnemy == null)
            {
                hasEnemyGrabbed = false;
                return;
            } //if enemy is dead you are not holding an enemy
            else
            {
                Debug.Log(grabbedEnemy.transform.GetComponent<EnemyBasic>().Health - 1);
                grabbedEnemy.transform.GetComponent<EnemyBasic>().Health -= 1;
                DisableMovement(0.5f);
            }
        }//when enemy is held drinkl blood
    }

    void DisableMovement(float timer)
    {
        StopAllCoroutines();
        StartCoroutine(DisableCoroutine(timer));
    }

    IEnumerator DisableCoroutine(float timer)
    {
        canAttack = false;
        yield return new WaitForSeconds(timer);
        playerState = PlayerState.NotAttacking;
        StartCoroutine(CooldownAttack());
    }

    IEnumerator CooldownAttack()
    {
        armsSR.enabled = false;
        float t = 0;
        while(t < 0.5f)
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
