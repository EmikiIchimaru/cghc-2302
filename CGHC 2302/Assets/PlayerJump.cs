using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    [SerializeField] private PlayerStats stats;

    private Rigidbody2D rb;
    private PlayerOnGround ground;

    public Vector2 velocity;

    public bool isOnGround;
    private bool isJumpInput;
    private bool isPressingJump;
    private bool isCurrentlyJumping;

    private float coyoteTimeCounter = 0f;
    private float jumpBufferCounter;
    private float defaultGravityMulti;

    public float gravityMulti;
    public float jumpSpeed;
    public bool canJumpAgain = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        ground = GetComponent<PlayerOnGround>();
        defaultGravityMulti = 1f;
    }

    void Update()
    {
        GetJumpInput();
        SetPhysics();
        isOnGround = ground.isOnGround;

        //Jump buffer allows us to queue up a jump, which will play when we next hit the ground
        if (stats.jumpBuffer > 0)
        {
            //Instead of immediately turning off "desireJump", start counting up...
            //All the while, the Jump function will repeatedly be fired off
            if (isJumpInput)
            {
                jumpBufferCounter += Time.deltaTime;

                if (jumpBufferCounter > stats.jumpBuffer)
                {
                    //If time exceeds the jump buffer, turn off "desireJump"
                    isJumpInput = false;
                    jumpBufferCounter = 0;
                }
            }
        }

        //If we're not on the ground and we're not currently jumping, that means we've stepped off the edge of a platform.
        //So, start the coyote time counter...
        if (!isCurrentlyJumping && !isOnGround)
        {
            coyoteTimeCounter += Time.deltaTime;
        }
        else
        {
            //Reset it when we touch the ground, or jump
            coyoteTimeCounter = 0;
        }

        
    }

    void FixedUpdate()
    {
        //Get velocity from Kit's Rigidbody 
        velocity = rb.velocity;

        //Keep trying to do a jump, for as long as desiredJump is true
        if (isJumpInput)
        {
            Jump();
            rb.velocity = velocity;

            //Skip gravity calculations this frame, so currentlyJumping doesn't turn off
            //This makes sure you can't do the coyote time double jump bug
            return;
        }

        calculateGravity();
    }

   

    private void SetPhysics()
    {
        //Determine the character's gravity scale, using the stats provided. Multiply it by a gravMultiplier, used later
        Vector2 newGravity = new Vector2(0, (-2 * stats.jumpHeight) / (stats.timeToApex * stats.timeToApex));
        rb.gravityScale = (newGravity.y / Physics2D.gravity.y) * gravityMulti;
    }

    private void calculateGravity()
    {
        //We change the character's gravity based on her Y direction

        //If Kit is going up...
        if (rb.velocity.y > 0.01f)
        {
            if (isOnGround)
            {
                //Don't change it if Kit is standing on something (such as a moving platform)
                gravityMulti = defaultGravityMulti;
            }
            else
            {
                //If we're using variable jump height...)
                if (stats.variableJH)
                {
                    gravityMulti = (isPressingJump && isCurrentlyJumping) ? stats.upGravityMulti : stats.jumpntGravityMulti;
                    /* //Apply upward multiplier if player is rising and holding jump
                    if (pressingJump && currentlyJumping)
                    {
                        gravityMulti = upGravityMulti;
                    }
                    //But apply a special downward multiplier if the player lets go of jump
                    else
                    {
                        gravityMulti = jumpntGravityMulti;
                    } */
                }
                else
                {
                    gravityMulti = stats.upGravityMulti;
                }
            }
        }
        //Else if kit is going down...
        else if (rb.velocity.y < -0.01f) 
        {
            gravityMulti = (isOnGround) ? defaultGravityMulti : stats.downGravityMulti;
            /* if (isOnGround)
            //Don't change it if Kit is stood on something (such as a moving platform)
            {
                gravityMulti = defaultGravityMulti;
            }
            else
            {
                //Otherwise, apply the downward gravity multiplier as Kit comes back to Earth
                gravityMulti = downGravityMulti;
            } */
        }
        //Else not moving vertically at all
        else
        {
            if (isOnGround)
            {
                isCurrentlyJumping = false;
            }

            gravityMulti = defaultGravityMulti;
        }

        //Set the character's Rigidbody's velocity
        //But clamp the Y variable within the bounds of the speed limit, for the terminal velocity assist option
        rb.velocity = new Vector3(velocity.x, Mathf.Clamp(velocity.y, -stats.terminalVelocity, 100));
    }

    private void Jump()
    {
        
        if (isOnGround || (coyoteTimeCounter > 0.03f && coyoteTimeCounter < stats.coyoteTime) || canJumpAgain)
        {
            //Debug.Log("jump!");

            isJumpInput = false;
            jumpBufferCounter = 0;
            coyoteTimeCounter = 0;

            canJumpAgain = isOnGround || (stats.maxAirJumps == 1 && canJumpAgain == false);

            jumpSpeed = Mathf.Sqrt(-2f * Physics2D.gravity.y * rb.gravityScale * stats.jumpHeight);

            if (velocity.y > 0f)
            {
                //Debug.Log("velocity greater than 0");
                jumpSpeed = Mathf.Max(jumpSpeed - velocity.y, 0f);
            }
            else if (velocity.y < 0f)
            {
                //Debug.Log("velocity less than 0");
                jumpSpeed += Mathf.Abs(rb.velocity.y);
            }

            //Apply the new jumpSpeed to the velocity. It will be sent to the Rigidbody in FixedUpdate;
            //Debug.Log("velocity = " + velocity.y.ToString());
            //Debug.Log("js = " + jumpSpeed.ToString());
            velocity.y += jumpSpeed;
            //Debug.Log("new velocity = " + velocity.y.ToString());
            isCurrentlyJumping = true;
            
        }
        if (stats.jumpBuffer == 0)
        {
            //If we don't have a jump buffer, then turn off desiredJump immediately after hitting jumping
            isJumpInput = false;
        }
    }

    private void GetJumpInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isJumpInput = true;
            isPressingJump = true;
            //Debug.Log("iscont?");
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            isPressingJump = false;
        }
    }
}
