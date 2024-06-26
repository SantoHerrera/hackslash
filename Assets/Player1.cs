using UnityEngine;
using System.Collections;
using System.Collections.Generic;
// using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

//1
//https://awesometuts.com/blog/unity-vectors/
//2
//make run animation only run once.
//3
//make animation stop coming in and out.
public class Player1 : MonoBehaviour
{
    private Rigidbody2D rb;

    private CapsuleCollider2D cc;

    [SerializeField]
    private float jumpForce;

    private Vector2 newVelocity;

    private Vector2 newForce;

    private Vector2 slopeNormalPerp;

    bool[] playerNotActive = new bool[4];
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

    public int maxHealth = 100;
    public int currentHealth;

    public HealthBar healthbar;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // anim = GetComponent<Animator>();

        // currentHealth = maxHealth;

        // healthbar.SetMaxHealth(maxHealth);

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

    private bool isPlayerNotActive()
    {
        bool result = playerNotActive.All(x => x == false);
        return result;
    }

    private void ApplyMovement()
    {
        //movement speed is always .07
        // Debug.Log("movementspeed: " + newVe);
        // anim.SetTrigger("playerRun");
        // if not on slope
        if (isPlayerNotActive() && isGrounded)
        {
            rb.Sleep();
        }

        if (isGrounded && !isOnSlope && !isJumping)
        {
            newVelocity.Set(movementSpeed * xInput, 0.0f);
            rb.velocity = newVelocity;
            // Debug.Log("newVelocity: " + newVelocity);
        }
        else if (!isGrounded)
        {
            newVelocity.Set(movementSpeed * xInput, rb.velocity.y);
            rb.velocity = newVelocity;
        }
        // //If on slope
        // else if (isGrounded && isOnSlope && canWalkOnSlope && !isJumping)
        // {
        //     newVelocity.Set(
        //         movementSpeed * slopeNormalPerp.x * -xInput,
        //         movementSpeed * slopeNormalPerp.y * -xInput
        //     );
        //     rb.velocity = newVelocity;
        // }
        // //If in air

        // else if (!isGrounded)
        // {
        //     newVelocity.Set(movementSpeed * xInput, rb.velocity.y);
        //     rb.velocity = newVelocity;
        // }
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

    private void logPlayerNotActiveArray()
    {
        string result = "bools list: ";

        foreach (var item in playerNotActive)
        {
            result += item.ToString() + ", ";
        }

        Debug.Log(result);
        Debug.Log("isPlayerNotActive " + isPlayerNotActive());
    }

    private void CheckInput()
    {
        // xInput = Input.GetAxisRaw("Horizontal");

        // if (xInput == 1 && facingDirection == -1)
        // {
        //     Flip();
        //     playerNotActive[2] = true;
        // }
        // else if (xInput == -1 && facingDirection == 1)
        // {
        //     Flip();
        //     playerNotActive[3] = true;
        // }

        // if (Input.GetKeyDown(KeyCode.W))
        // {
        //     Jump();
        //     playerNotActive[1] = true;
        // }
        // else
        // {
        //     playerNotActive[1] = false;
        // }




        if (Input.GetKey("a"))
        {
            playerNotActive[2] = true;
            // startAnim();
            if (facingDirection != -1)
            {
                Flip();
            }
            xInput = -1;
            // Flip();
        }
        else
        {
            playerNotActive[2] = false;
        }

        if (Input.GetKey("d"))
        {
            playerNotActive[3] = true;
            // startAnim();
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

        if (Input.GetKey("w"))
        {
            Jump();
            logPlayerNotActiveArray();
        }

        if (Input.GetKey("w"))
        {
            Jump();
            playerNotActive[1] = true;
        }
        else
        {
            playerNotActive[1] = false;
        }

        if (Input.GetKeyUp("a") || Input.GetKeyUp("d"))
        {
            xInput = 0;
            logPlayerNotActiveArray();
        }
    }

    void TakeDamage(int damage)
    {
        currentHealth -= damage;

        healthbar.SetHealth(currentHealth);
    }

    void Update()
    {
        // transform.position = myPlay.position + myPos;

        xInput = Input.GetAxisRaw("Horizontal");

        // Debug.Log("Text: ");



        // if (xInput != 0)
        // {
        //     // Debug.Log("animation should be running now");
        //     anim.SetBool("startRunAnim", true);
        // }

        if (Input.GetKeyDown("c"))
        {
            TakeDamage(20);
        }

        // Debug.Log(movementSpeed);

        // if(movementSpeed > .05)
        // {
        //     Debug.Log("turn on isIdling and start idling animation");
        // }


        //  if(xInput == 0)
        // {
        //     anim.SetBool("startRunAnim", false);
        //     Debug.Log("is this ever being called?");
        //     anim.SetBool("isIdling", true);
        // }


        // anim.SetBool("startRunAnim", false);

        // if (Input.GetKeyDown(KeyCode.UpArrow))input
        // if (Input.GetButton("ightArrow"))
        // {

        //     Debug.Log("this hoe working as im jumping");

        //     // anim.SetTrigger("startRunAnim");

        //      anim.SetBool("startRunAnim", true);
        //     // Debug.Log("Text: " );

        //     // if (Input.GetKeyUp(KeyCode.UpArrow))
        //     // {
        //     //     anim.SetTrigger("startRunAnim");
        //     // }
        // }



        CheckInput();
    }
}
