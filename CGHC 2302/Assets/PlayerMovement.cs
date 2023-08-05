using UnityEngine;


public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private PlayerStats stats;

    private Rigidbody2D rb;
    private PlayerOnGround ground;

    public Vector2 velocity;

    private float directionX;
    private Vector2 desiredVelocity;
    private float accel;
    private float decel;
    private float turnSpeed;
    private float velocityDelta;

    public bool isPressingKey;
    public bool isOnGround;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        ground = GetComponent<PlayerOnGround>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        GetDirectionX();
        

        if (directionX != 0)
        {
            transform.localScale = new Vector3(directionX > 0 ? 1 : -1, 1, 1);
            isPressingKey = true;
        }
        else
        {
            isPressingKey = false;
        }

        desiredVelocity = new Vector2(directionX, 0f) * Mathf.Max(stats.maxSpeed, 0f);
    }

    private void FixedUpdate()
    {
        //Fixed update runs in sync with Unity's physics engine

        isOnGround = ground.isOnGround;

        //Get the Rigidbody's current velocity
        velocity = rb.velocity;
        runWithAcceleration();
    }

    void GetDirectionX()
    {
        directionX = 0f;

        // Check for A key (move left)
        if (Input.GetKey(KeyCode.A))
        {
            directionX = directionX - 1f;
        }
        // Check for D key (move right)
        if (Input.GetKey(KeyCode.D))
        {
            directionX = directionX + 1f;
        }

        //Debug.Log(directionX.ToString());
    }

    private void runWithAcceleration()
    {
        //Set our acceleration, deceleration, and turn speed stats, based on whether we're on the ground on in the air

        accel = isOnGround ? stats.accel : stats.airAccel;
        decel = isOnGround ? stats.decel : stats.airDecel;
        turnSpeed = isOnGround ? stats.turnSpeed : stats.airTurnSpeed;

        if (isPressingKey)
        {
            //If the sign (i.e. positive or negative) of our input direction doesn't match our movement, it means we're turning around and so should use the turn speed stat.
            if (Mathf.Sign(directionX) != Mathf.Sign(velocity.x))
            {
                velocityDelta = turnSpeed * Time.deltaTime;
            }
            else
            {
                //If they match, it means we're simply running along and so should use the acceleration stat
                velocityDelta = accel * Time.deltaTime;
            }
        }
        else
        {
            //And if we're not pressing a direction at all, use the deceleration stat
            velocityDelta = decel * Time.deltaTime;
        }

        //Move our velocity towards the desired velocity, at the rate of the number calculated above
        velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, velocityDelta);

        //Update the Rigidbody with this new velocity
        rb.velocity = velocity;

    }
}
