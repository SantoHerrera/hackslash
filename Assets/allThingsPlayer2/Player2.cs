using UnityEngine;
using System.Collections;
using System.Collections.Generic;
// using UnityEngine;
using UnityEngine.InputSystem;

//1
//https://awesometuts.com/blog/unity-vectors/
//2
//make run animation only run once.
//3
//make animation stop coming in and out.
public class Player2 : MonoBehaviour
{
    private Rigidbody2D rb;

    private CapsuleCollider2D cc;

    [SerializeField]
    private float jumpForce;

    private Vector2 newVelocity;

    private Vector2 newForce;

    private Vector2 slopeNormalPerp;

    private bool isGrounded;

    private bool isOnSlope;

    private bool canWalkOnSlope;

    private bool isJumping;

    private bool canJump;

    // private int canDoubleJump = 0;

    private float slopeSideAngle;

    private float xInput;

    private float slopeDownAngle;

    private float lastSlopeAngle;

    private int facingDirection = 1;

    [SerializeField]
    private float movementSpeed;

    [SerializeField]
    private float slopeCheckDistance;

    [SerializeField]
    private float maxSlopeAngle;

    [SerializeField]
    private PhysicsMaterial2D noFriction;

    [SerializeField]
    private PhysicsMaterial2D fullFriction;

    public Animator anim;

    public KeyCode attack1;

    public Transform groundcheck;

    public float groundCheckRadius;

    public LayerMask whatIsGround;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // anim = GetComponent<Animator>();

        // m_Rigidbody = GetComponent<Rigidbody>();
        //Stop the Rigidbody from rotating
        rb.freezeRotation = true;
    }

    private void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(groundcheck.position, groundCheckRadius, whatIsGround);

        if (rb.velocity.y <= -0.0f)
        {
            isJumping = false;
        }

        if (isGrounded && !isJumping && slopeDownAngle <= maxSlopeAngle)
        {
            canJump = true;
        }
    }

    // void OnDrawGizmosSelected()
    // {
    //     // Draw a yellow sphere at the transform's position
    //     Gizmos.color = Color.yellow;
    //     Gizmos.DrawSphere(groundcheck.position, groundCheckRadius);
    // }

    private void ApplyMovement()
    {
        //movement speed is always .07
        // Debug.Log("movementspeed: " + newVe);
        // anim.SetTrigger("playerRun");
        // if not on slope


        if (isGrounded && !isOnSlope && !isJumping)
        {
            newVelocity.Set(movementSpeed * xInput, 0.0f);
            rb.velocity = newVelocity;
            // Debug.Log("newVelocity: " + newVelocity);
        }
        //If on slope
        else if (isGrounded && isOnSlope && canWalkOnSlope && !isJumping)
        {
            newVelocity.Set(
                movementSpeed * slopeNormalPerp.x * -xInput,
                movementSpeed * slopeNormalPerp.y * -xInput
            );
            rb.velocity = newVelocity;
        }
        //If in air

        else if (!isGrounded)
        {
            newVelocity.Set(movementSpeed * xInput, rb.velocity.y);
            rb.velocity = newVelocity;
        }
    }

    private void SlopeCheck()
    {
        Vector2 checkPos = -transform.position;

        SlopeCheckHorizontal(checkPos);
        SlopeCheckVertical(checkPos);
    }

    private void SlopeCheckHorizontal(Vector2 checkPos)
    {
        RaycastHit2D slopeHitFront = Physics2D.Raycast(
            checkPos,
            transform.right,
            slopeCheckDistance,
            whatIsGround
        );
        RaycastHit2D slopeHitBack = Physics2D.Raycast(
            checkPos,
            -transform.right,
            slopeCheckDistance,
            whatIsGround
        );

        if (slopeHitFront)
        {
            isOnSlope = true;

            slopeSideAngle = Vector2.Angle(slopeHitFront.normal, Vector2.up);
        }
        else if (slopeHitBack)
        {
            isOnSlope = true;

            slopeSideAngle = Vector2.Angle(slopeHitBack.normal, Vector2.up);
        }
        else
        {
            slopeSideAngle = 0.0f;
            isOnSlope = false;
        }
    }

    private void SlopeCheckVertical(Vector2 checkPos)
    {
        RaycastHit2D hit = Physics2D.Raycast(
            checkPos,
            Vector2.down,
            slopeCheckDistance,
            whatIsGround
        );

        if (hit)
        {
            slopeNormalPerp = Vector2.Perpendicular(hit.normal).normalized;

            slopeDownAngle = Vector2.Angle(hit.normal, Vector2.up);

            if (slopeDownAngle != lastSlopeAngle)
            {
                isOnSlope = true;
            }

            lastSlopeAngle = slopeDownAngle;

            Debug.DrawRay(hit.point, slopeNormalPerp, Color.blue);
            Debug.DrawRay(hit.point, hit.normal, Color.green);
        }

        if (slopeDownAngle > maxSlopeAngle || slopeSideAngle > maxSlopeAngle)
        {
            canWalkOnSlope = false;
        }
        else
        {
            canWalkOnSlope = true;
        }

    }

    private void FixedUpdate()
    {
        CheckGround(); //can move side to side but not jump
        SlopeCheck(); //on a hill will just keep sliding down
        ApplyMovement();
    }

    private void Flip()
    {
        facingDirection *= -1;
        transform.Rotate(0.0f, 180.0f, 0.0f);
    }

    private void Jump()
    {
        // canDoubleJump++;

        // Debug.Log("canJump: " + canJump);

        if (canJump)
        {
            canJump = false;
            isJumping = true;
            newVelocity.Set(0.0f, 0.0f);
            rb.velocity = newVelocity;
            newForce.Set(0.0f, jumpForce);
            rb.AddForce(newForce, ForceMode2D.Impulse);
        }

   
    }

    private void startAnim()
    {
        anim.SetBool("startRunAnim", true);
    }

    private void CheckInput()
    {
 
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            // startAnim();
            if (facingDirection != -1)
            {
                Flip();
            }
            xInput = -1;
            // Flip();
            
        }        

        if (Input.GetKey(KeyCode.RightArrow))
        {
            // startAnim();
            if (facingDirection != 1)
            {
                Flip();
            }
            xInput = 1;
        }

        if (Input.GetKeyDown("j"))
        {
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Jump();
        }

        if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.LeftArrow) )
        {
            xInput = 0;    
        }

    }

    void Update()
    {
        CheckInput();
    }
}
