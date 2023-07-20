using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//1
//https://awesometuts.com/blog/unity-vectors/
//2
//make run animation only run once.
//3
//make animation stop coming in and out.
public class Player : MonoBehaviour
{
    private Rigidbody2D rb;

    private CapsuleCollider2D cc;

    [SerializeField]
    private float jumpForce;

    private Vector2 newVelocity;

    private Vector2 newForce;

    private Vector2 capsuleColliderSize;

    private Vector2 slopeNormalPerp;

    private bool isGrounded;

    private bool isOnSlope;

    private bool canWalkOnSlope;

    private bool isJumping;

    private bool canJump;

    private float slopeSideAngle;

    private float xInput;

    private float slopeDownAngle;

    private float lastSlopeAngle;

    private int facingDirection = 1;

    [SerializeField]
    private Transform groundCheck;

    [SerializeField]
    private float groundCheckRadius;

    [SerializeField]
    private LayerMask whatIsGround;

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

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        cc = GetComponent<CapsuleCollider2D>();

        capsuleColliderSize = cc.size;

        Debug.Log("can tnis work");
    }

    private void CheckGround()
    {
        // LayerMask = LayerMask.NameToLayer("Floor");
        // Vector2 newPos = transform.position - Vector3 (new Vector2(0.0f, .18f));
        Vector2 newPos =
            transform.position - (Vector3)(new Vector2(0.0f, .18f));

        // groundCheck.position = newPos;
        isGrounded =
            Physics2D.OverlapCircle(newPos, groundCheckRadius, whatIsGround);

        if (rb.velocity.y <= 0.0f)
        {
            isJumping = false;
        }

        if (isGrounded && !isJumping && slopeDownAngle <= maxSlopeAngle)
        {
            canJump = true;
        }

        // Debug.Log("touching ground " + isGrounded);

        // OnDrawGizmosSelected();
    }

    private void OnDrawGizmos()
    {
        Vector2 newPos =
            transform.position - (Vector3)(new Vector2(0.0f, .18f));

        // Gizmos.DrawWireSphere(newPos, groundCheckRadius);
    }

    private void ApplyMovement()
    {
        // anim.SetTrigger("playerRun");
        //if not on slope
        if (isGrounded && !isOnSlope && !isJumping)
        {
            // Debug.Log("This one");
            newVelocity.Set(movementSpeed * xInput, 0.0f);
            rb.velocity = newVelocity;
        }
        //If on slope
        else if (isGrounded && isOnSlope && canWalkOnSlope && !isJumping)
        {
            newVelocity
                .Set(movementSpeed * slopeNormalPerp.x * -xInput,
                movementSpeed * slopeNormalPerp.y * -xInput);
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
        Vector2 checkPos =
            transform.position -
            (Vector3)(new Vector2(0.0f, capsuleColliderSize.y / 2));

        SlopeCheckHorizontal (checkPos);
        SlopeCheckVertical (checkPos);
    }

    private void SlopeCheckHorizontal(Vector2 checkPos)
    {
        RaycastHit2D slopeHitFront =
            Physics2D
                .Raycast(checkPos,
                transform.right,
                slopeCheckDistance,
                whatIsGround);
        RaycastHit2D slopeHitBack =
            Physics2D
                .Raycast(checkPos,
                -transform.right,
                slopeCheckDistance,
                whatIsGround);

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
        RaycastHit2D hit =
            Physics2D
                .Raycast(checkPos,
                Vector2.down,
                slopeCheckDistance,
                whatIsGround);

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

        if (isOnSlope && canWalkOnSlope && xInput == 0.0f)
        {
            rb.sharedMaterial = fullFriction;
        }
        else
        {
            rb.sharedMaterial = noFriction;
        }
    }

    private void FixedUpdate()
    {
        CheckGround();
        SlopeCheck();
        ApplyMovement();
    }

    private void Flip()
    {
        facingDirection *= -1;
        transform.Rotate(0.0f, 180.0f, 0.0f);
    }

    private void Jump()
    {
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

    private void CheckInput()
    {
        xInput = Input.GetAxisRaw("Horizontal");

        // if (xInput == 1 && facingDirection == -1)
        // {
        //     Flip();
        // }
        // else if (xInput == -1 && facingDirection == 1)
        // {
        //     Flip();
        // }

        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            Jump();
        }
    }

    void Update()
    {
        xInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            anim.SetTrigger("playerRun");

            if (Input.GetKeyUp(KeyCode.UpArrow))
            {
                anim.SetTrigger("playerRun");
            }
        }

        CheckInput();
    }
}
