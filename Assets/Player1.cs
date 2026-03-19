using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using System.Linq;
using UnityEngine.SceneManagement;

//1
//https://awesometuts.com/blog/unity-vectors/
//2
//make run animation only run once.
//3
//make animation stop coming in and out.
public class Player1 : MonoBehaviour
{
    public Animator anim;
    public KeyCode attack1;
    public Transform groundcheck;
    public LayerMask whatIsGround;
    public HealthBar healthbar;
    public GameObject Canvas1;
    public BoxCollider2D CastleDoor;
    public Transform canvas;

    private Rigidbody2D rb;

    private Vector2 newVelocity;
    private Vector2 newForce;
    private Vector2 slopeNormalPerp;

    bool[] playerNotActive = new bool[4];

    private CapsuleCollider2D cc;

    private bool isGrounded;
    private bool isOnSlope;

    private bool isJumping;
    private bool canJump;

    public float groundCheckRadius;
    private float slopeSideAngle;
    private float xInput;
    private float slopeDownAngle;
    private float lastSlopeAngle;

    public int maxHealth = 100;
    public int currentHealth;
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

    [SerializeField]
    private float jumpForce;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

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

    private bool isPlayerNotActive()
    {
        bool result = playerNotActive.All(x => x == false);
        return result;
    }

    private void ApplyMovement()
    {
        if (isPlayerNotActive() && isGrounded)
        {
            rb.Sleep();
        }
        if (isGrounded && !isOnSlope && !isJumping)
        {
            newVelocity.Set(movementSpeed * xInput, 0.0f);
            rb.velocity = newVelocity;
        }
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
        if (Input.GetKey("a"))
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

        if (Input.GetKey("d"))
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

        if (Input.GetKey("w"))
        {
            Jump();
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
        }

        if (rb.IsTouching(CastleDoor))
        {
            canvas.gameObject.SetActive(true);

            if (Input.GetKey("u"))
            {
                ChangeScene();
            }
        }
        else
        {
            canvas.gameObject.SetActive(false);
        }
    }

    void ChangeScene()
    {
        const string sceneToLoad = "Assets/Scenes/Menu.unity";
        var op = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);
        op.completed += (AsyncOperation obj) =>
        {
            Scene loadedScene = SceneManager.GetSceneByPath("Assets/Scenes/Menu.unity");
            //Debug.Log($"{sceneToLoad} finished loading (build index: {loadedScene.buildIndex}).");
            Debug.Log($"It has {loadedScene.rootCount} root(s).");
            //Debug.Log($"There are now {SceneManager.loadedSceneCount} Scenes open.");
        };
    }

    void Update()
    {
        xInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown("c"))
        {
            TakeDamage(20);
        }
        CheckInput();
        // canChangeScene();
    }

    void TakeDamage(int damage)
    {
        currentHealth -= damage;

        healthbar.SetHealth(currentHealth);
    }
}
