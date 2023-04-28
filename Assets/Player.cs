using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//1
//https://awesometuts.com/blog/unity-vectors/

//2
//make run animation only run once.

//3
//make animation stop coming in and out.



// // right direction
// Vector3 right = new Vector3(1, 0, 0);

// // left direction
// Vector3 left = new Vector3(-1, 0, 0);

// // up direction
// Vector3 up = new Vector3(0, 1, 0);

// // down direction
// Vector3 down = new Vector3(0, -1, 0);

// // forward direction
// Vector3 forward = new Vector3(0, 0, 1);

// // backward direction
// Vector3 backward = new Vector3(0, 0, -1);

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    private CapsuleCollider2D cc;

    private bool IsTouchingFloor = true;

    // [SerializeField]
    // private float StopMovementSpeed;

    // [SerializeField]
    // private float rayLength;

    // [SerializeField]
    // private float rayPositionOffset;

    [SerializeField]
    private float jumpForce;

    // Vector3 RayPositionCenter;

    // Vector3 RayPositionLeft;

    // Vector3 RayPositionRight;

    // Vector3 PositionBeforeMovement;

    // RaycastHit2D[] GroundHitsCenter;

    // RaycastHit2D[] GroundHitsLeft;

    // RaycastHit2D[] GroundHitsRight;

    //why does adding [3] fix error?
    //a jagged array?
    // RaycastHit2D[][] AllRaycastHits = new RaycastHit2D[1][];

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
    }

    private void CheckGround() 
    {

        // LayerMask = LayerMask.NameToLayer("Floor");

        // Vector2 newPos = transform.position - Vector3 (new Vector2(0.0f, .18f));
        Vector2 newPos = transform.position - (Vector3)(new Vector2(0.0f, .18f));

        // groundCheck.position = newPos;

        isGrounded = Physics2D.OverlapCircle(newPos, groundCheckRadius, whatIsGround);

        if(rb.velocity.y <= 0.0f)
        {
            isJumping = false;
        }

        if(isGrounded && !isJumping && slopeDownAngle <= maxSlopeAngle)
        {
            canJump = true;
        }

        // Debug.Log("touching ground " + isGrounded);

        // OnDrawGizmosSelected();
    }

    private void OnDrawGizmos()
    {


        Vector2 newPos = transform.position - (Vector3)(new Vector2(0.0f, .18f));

        // Gizmos.DrawWireSphere(newPos, groundCheckRadius);
    }
    
    private void ApplyMovement() 
    {
        // anim.SetTrigger("playerRun");
        if (isGrounded && !isOnSlope && !isJumping) //if not on slope
        {
            // Debug.Log("This one");
            newVelocity.Set(movementSpeed * xInput, 0.0f);
            rb.velocity = newVelocity;
        }
        else if (isGrounded && isOnSlope && canWalkOnSlope && !isJumping) //If on slope
        {
            newVelocity.Set(movementSpeed * slopeNormalPerp.x * -xInput, movementSpeed * slopeNormalPerp.y * -xInput);
            rb.velocity = newVelocity;
        }
        else if (!isGrounded) //If in air
        {
            newVelocity.Set(movementSpeed * xInput, rb.velocity.y);
            rb.velocity = newVelocity;
        }
    }

    

    private void SlopeCheck()
    {
        Vector2 checkPos = transform.position - (Vector3)(new Vector2(0.0f, capsuleColliderSize.y / 2));

        SlopeCheckHorizontal(checkPos);
        SlopeCheckVertical(checkPos);
    }

    private void SlopeCheckHorizontal(Vector2 checkPos)
    {
        RaycastHit2D slopeHitFront = Physics2D.Raycast(checkPos, transform.right, slopeCheckDistance, whatIsGround);
        RaycastHit2D slopeHitBack = Physics2D.Raycast(checkPos, -transform.right, slopeCheckDistance, whatIsGround);

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
         RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, slopeCheckDistance, whatIsGround);

        if (hit)
        {

            slopeNormalPerp = Vector2.Perpendicular(hit.normal).normalized;            

            slopeDownAngle = Vector2.Angle(hit.normal, Vector2.up);

            if(slopeDownAngle != lastSlopeAngle)
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

        if (xInput == 1 && facingDirection == -1)
        {
            Flip();
        }
        else if (xInput == -1 && facingDirection == 1) 
        {            
            Flip();
        }       

        // // if (Input.GetButtonDown("Jump"))
        // if(Input.GetButtonDown("Jump")sss)
        // // if (Input.GetKeyDown(KeyCode.UpArrow))
        // // if (Input.GetButtonDown("Jump"))
        
        //     Jump();

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


        
        // if(Input.GetKeyDown(attack1))
        // {
        //     anim.SetTrigger("playerRun");
        // }

        
    }


    void Update()
    {
        // drawRayCast();

        xInput = Input.GetAxisRaw("Horizontal");

        // if (xInput == 1 && facingDirection == -1)
        // {
        //     Flip();
        // }
        // else if (xInput == -1 && facingDirection == 1) 
        // {            
        //     Flip();
        // }       

        // if(Input.GetKeyDown(attack1))
        if(Input.GetKeyDown(KeyCode.UpArrow))
        // if(xInput)
        {
            anim.SetTrigger("playerRun");
            // anim.SetTrigger("playerRun");
            if(Input.GetKeyUp(KeyCode.UpArrow))
            {
                anim.SetTrigger("playerRun");
            }
            
        }

        CheckInput();

        // if (Input.GetKeyDown(KeyCode.RightArrow))
        // {
        //     transform.Translate(Vector3.up * 4 * Time.deltaTime);
        //     transform.Translate(Vector3.right * 4 * Time.deltaTime);
        // }
        // if (Input.GetKeyDown(KeyCode.LeftArrow))
        // {
        //     transform.Translate(Vector3.up * 4 * Time.deltaTime);
        //     transform.Translate(Vector3.left * 4 * Time.deltaTime);
        // }
        // if (Input.GetKeyDown(KeyCode.UpArrow))
        // {
        //     transform.Translate(Vector3.up * 30 * Time.deltaTime);
        // }
    }
}


























//might need these later
// private void OnMouseDown()
//     {
//         GetComponent<SpriteRenderer>().color = Color.red;
//     }

//     private void OnMouseUp()
//     {
//         GetComponent<SpriteRenderer>().color = Color.white;
//     }

// private void StopMovement()
    // {
    //     Rigidbody2D body = GetComponent<Rigidbody2D>();

    //     if (IsTouchingFloor)
    //     {
    //         body.Sleep();
    //     }
    // }

     // void FixedUpdate()
    // {
    //     // if (IsTouchingFloor == true)
    //     // {
    //     //     StopMovement();
    //     // }
    // }

    // private bool GroundCheck(RaycastHit2D[][] GroundHits)
    // {
    //     foreach (RaycastHit2D[] HitList in GroundHits)
    //     {
    //         foreach (RaycastHit2D hit in HitList)
    //         {
    //             if (hit.collider != null)
    //             {
    //                 if (hit.collider.tag == "Floor")
    //                 {
    //                     // Debug.Log("is touching floor " + IsTouchingFloor );
    //                     return true;
    //                 }
    //             }
    //         }
    //     }
    //     return false;
    // }

    // private void drawRayCast()
    // {
    //     //what the next line does is it adds first with first's and second's and third's
    //     //ex. transornposition = (1,1,5) and new vector3 = (1,2,3) then the total would be (2,3,8)
    //     RayPositionCenter =
    //         transform.position + new Vector3(-rayPositionOffset, -.243f, 0);

    //     GroundHitsCenter =
    //         Physics2D.RaycastAll(RayPositionCenter, Vector2.right, rayLength);

    //     AllRaycastHits[0] = GroundHitsCenter;

    //     IsTouchingFloor = GroundCheck(AllRaycastHits);

    //     Debug.DrawRay(RayPositionCenter, Vector2.right * rayLength, Color.red);
    // }



// Pos.y += Mathf.Abs(body.velocity.x) * Time.deltaTime * (body.velocity.x > 0 ? 1 : -1);

// transform.position = Pos;

// if (hit.collider != null && Mathf.Abs(hit.normal.x) > 0.1f) {
// 			Rigidbody2D body = GetComponent<Rigidbody2D>();
// 			// Apply the opposite force against the slope force
// 			// You will need to provide your own slopeFriction to stabalize movement
// 			body.velocity = new Vector2(body.velocity.x - (hit.normal.x * slopeFriction), body.velocity.y);

// 			//Move Player up or down to compensate for the slope below them
// 			Vector3 pos = transform.position;
// 			pos.y += -hit.normal.x * Mathf.Abs(body.velocity.x) * Time.deltaTime * (body.velocity.x - hit.normal.x > 0 ? 1 : -1);
// 			transform.position = pos;
// 		}

// Rigidbody2D body = GetComponent<Rigidbody2D>();
// 			// Apply the opposite force against the slope force
// 			// You will need to provide your own slopeFriction to stabalize movement
// 			body.velocity = new Vector2(body.velocity.x - (hit.normal.x * slopeFriction), body.velocity.y);

// 			//Move Player up or down to compensate for the slope below them
// 			Vector3 pos = transform.position;
//pos.y += -hit.normal.x * Mathf.Abs(body.velocity.x) * Time.deltaTime * (body.velocity.x - hit.normal.x > 0 ? 1 : -1);
// 			transform.position = pos;

// Debug.Log("body " + body.velocity);
// Debug.Log("transform " + transform.position);
// Debug.Log("StopMovement " + IsTouchingFloor);

// for (int i = 0; i < GroundHitsCenter.Length; i++)
// {
// // //     // if(this.GroundHitsCenter[i].normal.x == 0)
// // //     // {
// // //     //     continue;
// // //     // }
//     Debug.Log("whats this " + this.GroundHitsCenter[i].point);
//     Debug.Log("Body position " + RayPositionCenter);
// }

// if(body.velocity.x != 0 || body.velocity.y != 0)
// {
//     // Debug.Log("This bitch stagnat " + body.velocity.x + " " + body.velocity.y);
//     Debug.Log("body.velocity.x" + body.velocity.x );
//     Debug.Log("body.velocity.y" + body.velocity.y );
// }

// if(IsTouchingFloor)
// {
//     if(body.velocity.x != 0 || body.velocity.y != 0)
//     {

//         translate this to my needs
//             //     // Vector3 pos = transform.position;
// // 	// pos.y += -this.GroundHitsRight[i].normal.x * Mathf.Abs(body.velocity.x) * Time.deltaTime * (body.velocity.x - this.GroundHitsRight[i].normal.x > 0 ? 1 : -1);
// // 	// transform.position = pos;
//     }
// }

// Debug.DrawRay(RayPositionCenter, Vector2.down * rayLength, Color.red);

//// Set the Position
// Vector2 newPos = new Vector2(startPos.x, startPos.y + dir.y);
// transform.position = newPos;

// @NOTE Must be called from FixedUpdate() to work properly
// void NormalizeSlope () {
// 	// Attempt vertical normalization
// 	if (IsTouchingFloor) {
// 		RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up, 1f, whatIsGround);

// 		if (hit.collider != null && Mathf.Abs(hit.normal.x) > 0.1f) {
// 			Rigidbody2D body = GetComponent<Rigidbody2D>();
// 			// Apply the opposite force against the slope force
// 			// You will need to provide your own slopeFriction to stabalize movement
// 			body.velocity = new Vector2(body.velocity.x - (hit.normal.x * slopeFriction), body.velocity.y);

// 			//Move Player up or down to compensate for the slope below them
// 			Vector3 pos = transform.position;
// 			pos.y += -hit.normal.x * Mathf.Abs(body.velocity.x) * Time.deltaTime * (body.velocity.x - hit.normal.x > 0 ? 1 : -1);
// 			transform.position = pos;
// 		}
// 	}
// }

//look into physics material

// foreach (RaycastHit2D[] HitList in GroundHits)
// {
//     //what does the type of hit look like?
//     foreach(RaycastHit2D hit in HitList)
//     {
//         if(hit.collider != null)
//         {
//             if(hit.collider.tag != "PlayerCollider")
//             {
//                 return true;
//             }
//         }
//     }
// }

// This is something that happens with a number of properties in Unity. The reasoning behind it is that rigidbody.velocity it's a regular variable, but a property (a pair of set/get functions that masquerades as a variable for convenience). So if you imagine that it looked like this instead:
// Code (csharp):
// public Vector3 GetVelocity() {
//    return someVelocityValue;
// }

// GetVelocity().x = horizontalMovement.x;
// That should make it a little more clear why this isn't allowed, I hope?

// (This is something that UnityScript/Javascript had special compiler stuff to work around, so your stumbling across this is certainly a direct result of following a JS-based tutorial while coding in C#.)

// So what you'll need to do is assign the vector as a whole, which you can do easily like this:
// Code (csharp):
// rb.velocity = new Vector3(horizontalMovement.x, 0f, horizontalMovement.z);
// This same thing applies to pretty much all struct properties of Unity classes. The common structs are Vector2, Vector3, Quaternion, and Rect off the top of my head, so properties like transform.position, transform.rotation, rigidbody.velocity, rectTransform.sizeDelta, and anything along those lines needs to be done the same way.

// Haxzploid said: ↑
// Also, does some of the code belong in fixedUpdate, rather than Update?
// The general rule is: physics code should almost always go in FixedUpdate, input code always always always must go in Update, and most other code should probably go in Update. Of course there's a bit of a tangle here because this code uses input to control physics, suggesting it should go in both. The best practice here is: receive input in Update, store the important info, then use that info in FixedUpdate. This particular code is actually most of the way there; basically, those last two lines should probably be moved to FixedUpdate.

// The reasoning is basically that you may get an unpredictable number of Update's vs FixedUpdate's or physics frames, but you always know there's one FixedUpdate per physics frame. Let's imagine your character is sliding along a friction-y floor - that will slow him down a little bit every physics frame. If your game is running at a low frame rate, you'll see him moving at a different speed than if your game is running at a smooth frame rate, which is not something that should be allowed to happen. The reason is:
// (With Update, if the framerate is 100fps)
// Update() : set velocity to 5
// Update() : set velocity to 5
// physics frame: slow to 4.9
// Update() : set velocity to 5
// Update() : set velocity to 5
// physics frame: slow to 4.9
// Update() : set velocity to 5
// Update() : set velocity to 5
// physics frame: slow to 4.9
// Update() : set velocity to 5
// Update() : set velocity to 5
// physics frame: slow to 4.9
// Average speed: ~4.95 (since the superfluous Update's don't actually move the object)

// (With Update, if the framerate is below about 20fps)
// Update() : set velocity to 5
// physics frame: slow to 4.9
// physics frame: slow to 4.8
// physics frame: slow to 4.7
// Update() : set velocity to 5
// Average speed: ~4.8

// (With FixedUpdate, no matter what the framerate is)
// FixedUpdate() : set velocity to 5
// physics frame : slow to 4.9
// FixedUpdate() : set velocity to 5
// physics frame : slow to 4.9
// FixedUpdate() : set velocity to 5
// physics frame : slow to 4.9
// Average speed: ~4.95

// Does that make sense?
