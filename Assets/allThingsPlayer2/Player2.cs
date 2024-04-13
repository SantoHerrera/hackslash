using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using System.Linq;

//todo
//make run animation run
public class Player2 : MonoBehaviour
{
    public Animator anim;
    public Transform groundcheck;
    public LayerMask whatIsGround;

    private Rigidbody2D rb;

    private Vector2 newVelocity;
    private Vector2 newForce;
    private Vector2 slopeNormalPerp;

    bool[] playerNotActive = new bool[4];

    private bool isGrounded;
    private bool isOnSlope;
    private bool isJumping;
    private bool canJump;

    private int facingDirection = 1;

    public float groundCheckRadius;
    private float slopeSideAngle;
    private float xInput;
    private float slopeDownAngle;
    private float lastSlopeAngle;

    [SerializeField]
    private float movementSpeed;

    [SerializeField]
    private float slopeCheckDistance;

    [SerializeField]
    private float maxSlopeAngle;

    [SerializeField]
    private float jumpForce;

    [SerializeField]
    private PhysicsMaterial2D noFriction;

    [SerializeField]
    private PhysicsMaterial2D fullFriction;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        //Stop the Rigidbody from rotating
        rb.freezeRotation = true;
    }

    private void startAnim()
    {
        anim.SetBool("startRunAnim", true);
    }

    private void logPlayerNotActiveArray()
    {
        string result = "bools list: ";

        foreach (var item in playerNotActive)
        {
            result += item.ToString() + ", ";
        }
        
        Debug.Log(result);
    }

    private bool noHumanInput()
    {
        return playerNotActive.All(x => x == false);
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

    private void ApplyMovement()
    {
        if (noHumanInput() && isGrounded)
        {
            rb.Sleep();
        }

        if (isGrounded && !isOnSlope && !isJumping)
        {
            newVelocity.Set(movementSpeed * xInput, 0.0f);
            rb.velocity = newVelocity;
        }
        //makes it so you can glide while mid air ----otherwise jumping while moving sideways would be straight up and down, then once grounded countinue moving sideways
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

    private void FixedUpdate()
    {
        CheckGround(); //can move side to side but not jump
        SlopeCheck(); //on a hill will just keep sliding down
        ApplyMovement();
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

    private void Flip()
    {
        facingDirection *= -1;
        transform.Rotate(0.0f, 180.0f, 0.0f);
    }

    private void CheckInput()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            playerNotActive[2] = true;
            if (facingDirection != -1)
            {
                Flip();
            }
            xInput = -1;
        }
        else
        {
            playerNotActive[2] = false;
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            playerNotActive[3] = true;
            if (facingDirection != 1)
            {
                Flip();
            }
            xInput = 1;
        }
        else
        {
            playerNotActive[3] = false;
        }

        if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.LeftArrow))
        {
            xInput = 0;
        }

        if (Input.GetKey("j"))
        {
            Jump();
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            Jump();
            playerNotActive[1] = true;
        }
        else
        {
            playerNotActive[1] = false;
        }
    }

    void Update()
    {
        CheckInput();
    }
}
