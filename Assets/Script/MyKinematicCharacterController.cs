using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyKinematicCharacterController : MonoBehaviour {

    /**
     * Character Components.
     */
    Animator animator;
    CharacterBodyCostumization bodyParts;
    Rigidbody rb;
    Collider col;

    /**
     * Variables that handle falling movement.
     */
    const float ACCEL = 0.5f;
    const float GRAVITY = 9.8f;
    public float fallSpeed = 1.5f;
    public float fallSpeedIncrementer = 0.1f;

    private bool becameGrounded;
    [SerializeField]
    [ReadOnly]
    private bool isGrounded;
    public float groundingAngle = 0.5f;

    /**
     * Variables that handle jumping movement.
     */
    [SerializeField]
    [ReadOnly]
    private float JumpSpeed;
    private bool jump;
    public bool canJump = true;
    [SerializeField]
    [ReadOnly]
    private bool isJumping;

    float currentJumpNumber = 0;

    /**
     * Variables that handle sprint.
     */
    public bool canSprint = true;
    public bool canSprintToggle = false;
    [SerializeField]
    [ReadOnly]
    private bool isSprinting;

    /**
     * Variables that handle crouch.
     */
    public bool canCrouch = true;
    public bool canCrouchToggle = false;
    [SerializeField]
    [ReadOnly]
    private bool isCrouching;

    public bool canSlide = true;

    Vector3 input;
    Vector3 speed;
    float gravitySpeed;
    Vector3 boxColliderDimensions;
    RaycastHit hit;
    Collider[] nearbyColliders;

    // Use this for initialization
    void Start () {
        becameGrounded = false;
        isGrounded = false;
        isJumping = false;
        isSprinting = false;
        isCrouching = false;

        jump = false;
        gravitySpeed = GRAVITY * fallSpeed;
        boxColliderDimensions = new Vector3(2 / 3f, 1f, 2 / 3f);

        animator = GetComponent<Animator>();
        bodyParts = GetComponent<CharacterBodyCostumization>();
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        nearbyColliders = new Collider[16];
    }

	void Update () {

        input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        //animator.SetFloat("vertical", input.y);
        if (Input.GetKeyDown(KeyCode.Space) && (isGrounded || (currentJumpNumber < bodyParts.LegPart.JumpNumber)))
        {
            jump = true;
            JumpSpeed = bodyParts.LegPart.JumpHeight;
            gravitySpeed = GRAVITY * fallSpeed;
            currentJumpNumber++;
        }

        if (isGrounded && canSprint)
        {
            if (canSprintToggle)
            {
                if (Input.GetKeyDown(KeyCode.LeftShift))
                    isSprinting = !isSprinting;
            }
            else
            {
                isSprinting = Input.GetKey(KeyCode.LeftShift);
            }
        }

        if (Input.GetKeyDown("escape"))
        {
            Cursor.lockState = CursorLockMode.None;
        }

        if (canCrouch) {
            if (canCrouchToggle)
            {
                if (Input.GetKey(KeyCode.LeftControl))
                    isCrouching = !isCrouching;
            } else {
                isCrouching = Input.GetKey(KeyCode.LeftControl);
            }
        }
        if (isCrouching)
        {
            transform.localScale = new Vector3(1, 0.5f, 1);
        } else
        {
            transform.localScale = new Vector3(1, 1, 1);
        }

    }

    private void FixedUpdate()
    {
        speed = (transform.forward * input.z + transform.right * input.x);
        if (isSprinting)
        {
            speed *= bodyParts.LegPart.RunningSpeed;
        }
        else
        {
            speed *= bodyParts.LegPart.MovementSpeed;
        }

        float Radius = 1f;

        int numbOfNearbyCols = Physics.OverlapSphereNonAlloc(transform.position, Radius + 0.1f,
            nearbyColliders);

        Vector3 disp = Vector3.zero;
        Vector3 platformSpeed = Vector3.zero;
        if (numbOfNearbyCols > 1) //if its colliding with something check grounding
        {
            platformSpeed = CheckGrounding();
            disp = HandleCollision(ref speed, numbOfNearbyCols);
        } else
        {
            becameGrounded = false;
        }


        bool justLanded = becameGrounded && !isGrounded;
        bool justFell = (!isJumping || !jump) && !becameGrounded && isGrounded;
        //can check here the landing frame
        if (justLanded)
        {
            Debug.Log("landed");
        }

        if (justFell)
        {
            Debug.Log("fell");
        }

        isGrounded = becameGrounded;

        if (!isGrounded)
        {
            gravitySpeed += fallSpeedIncrementer;
            speed.y -= gravitySpeed;
        }

        if (jump)
        {
            jump = false;
            isJumping = true;
            becameGrounded = false;
            isGrounded = false;
        }

        if (isJumping)
        {
            speed.y += JumpSpeed;
            JumpSpeed = Mathf.Max(0, JumpSpeed - Time.deltaTime * Time.deltaTime * 2000);
        }

        bodyParts.SetMovementState(speed, isJumping, isSprinting);

        rb.MovePosition(rb.position + disp + (platformSpeed + speed) * Time.deltaTime);
    }

    private Vector3 HandleCollision(ref Vector3 speed, int numbOfNearbyCols)
    {
        Vector3 displacement = new Vector3();
        Vector3 dir = new Vector3();
        float dist = 0;

        for (int i = 0; i < numbOfNearbyCols; i++)
        {
            if (nearbyColliders[i] == col)
            {
                continue;
            }

            if (Physics.ComputePenetration(col, transform.position, transform.rotation,
                nearbyColliders[i], 
                nearbyColliders[i].transform.position, 
                nearbyColliders[i].transform.rotation,
                out dir, out dist))
            {

                // Get outta that collider!
                displacement += dir * dist;

                // Crop down the velocity component which is in the direction of penetration
                if(Vector3.Dot(speed, dir) < 0)
                {
                    speed -= Vector3.Project(speed, dir);
                }
            }
        }

        for (var i = 0; i < nearbyColliders.Length; i++)
        {
            nearbyColliders[i] = null;
        }

        return displacement;
    }

    /**
    * Check if Character is grounded by spherecasting the ground.
    */
    private Vector3 CheckGrounding()
    {
        Vector3 platformSpeed = Vector3.zero;
        becameGrounded = false;
        RaycastHit hitInfo;
        if (Physics.SphereCast(transform.position, 0.5f, -transform.up, out hitInfo, 0.55f))
        {
            //Only walk on non-steep ground
            if(Vector3.Dot(hitInfo.normal, Vector3.up) > groundingAngle)
            {
                becameGrounded = true;
                currentJumpNumber = 0;
                isJumping = false;
                platformSpeed = AddSpeedFromPlatform(hitInfo.collider);
            }
        }

        return platformSpeed;
    }

    /**
     * When the character gets on top of a platform it should add the platform speed to its own speed.
     */
    private Vector3 AddSpeedFromPlatform (Collider col)
    {
        Vector3 platformSpeed = Vector3.zero;
        MovePlatform mover = col.gameObject.GetComponent<MovePlatform>();
        if (mover != null)
        {
            platformSpeed = mover.currDir;
        }
        else
        {
            Rigidbody colRb = col.gameObject.GetComponent<Rigidbody>();
            if(colRb != null)
            {
                platformSpeed = colRb.velocity;
            }
        }

        return platformSpeed;
    }

    /**
     * Helper gizmos.
     */
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position , 1.1f);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
        Gizmos.DrawWireSphere(transform.position - transform.up * 0.55f, 0.5f);
        Gizmos.DrawRay(transform.position, -transform.up * 1f);
    }
    

}
