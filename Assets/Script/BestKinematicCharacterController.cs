using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BestKinematicCharacterController : MonoBehaviour {

    /**
     * Character Components.
     */
    Animator animator;
    CharacterBodyCostumization bodyParts;
    Rigidbody rb;
    CapsuleCollider col;

    /**
     * Variables that handle falling movement.
     */
    const float ACCEL = 0.5f;
    const float GRAVITY = 9.8f;
    public float fallSpeed = 1.5f;
    public float fallSpeedIncrementer = 0.1f;

    [SerializeField]
    [ReadOnly]
    private bool isSlopeSliding;

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

    [SerializeField]
    [ReadOnly]
    Vector3 speed;

    Vector3 input;

    [SerializeField]
    [ReadOnly]
    float gravitySpeed;
    Vector3 boxColliderDimensions;
    RaycastHit hit;
    Collider[] nearbyColliders;

    Vector3 walkingVector;
    Vector3 sideWalkingVector;

    /*
     * Collision handler.
     */
    public int maxCollisionAttempts = 20;
    float surfaceOffset = .0001f;
    float backstepOffset = .001f;
    float minVelocityBreak = .001f;

    // Use this for initialization
    void Start()
    {

        isSlopeSliding = false;
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
        col = GetComponent<CapsuleCollider>();

        nearbyColliders = new Collider[16];
    }


    void Update()
    {

        input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

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

        if (canCrouch)
        {
            if (canCrouchToggle)
            {
                if (Input.GetKey(KeyCode.LeftControl))
                    isCrouching = !isCrouching;
            }
            else
            {
                isCrouching = Input.GetKey(KeyCode.LeftControl);
            }
        }

        Crouch(isCrouching);


    }


    private void FixedUpdate()
    {
        walkingVector = transform.forward;
        sideWalkingVector = transform.right;
        speed = (walkingVector * input.z + sideWalkingVector * input.x);

        if (!isGrounded || (isSlopeSliding && !isGrounded))
        {
            gravitySpeed += fallSpeedIncrementer;
            speed.y -= gravitySpeed;
        }

        Vector3 disp = Vector3.zero;
        Vector3 platformSpeed = Vector3.zero;
        bodyParts.SetMovementState(speed, isJumping, isSprinting);
        rb.MovePosition(ContinuosCollisionDetection(disp + (platformSpeed + speed) * Time.deltaTime));
    }

    private Vector3 ContinuosCollisionDetection(Vector3 movementSpeed)
    {
        Debug.Log(movementSpeed);
        Vector3 originalVelocity = movementSpeed;
        Vector3 origin = rb.position;

        for (int attempts = 0; attempts < maxCollisionAttempts; attempts++)
        {
            Vector3 originTop = origin;
            originTop.y += col.height / 2;

            Vector3 originBottom = origin;
            originBottom.y -= col.height / 2;

            Vector3 prevOrigin = origin;
            Vector3 hitNormal = Vector3.zero;

            float castDistance = speed.magnitude + backstepOffset;
            Vector3 castDirection = speed.normalized;
            Vector3 castStartBackOffsetT = originTop - (castDirection * backstepOffset);
            Vector3 castStartBackOffsetB = originBottom - (castDirection * backstepOffset);

            RaycastHit hitInfo;
            DebugExtension.DebugCapsule(castStartBackOffsetT, castStartBackOffsetB, Color.red, col.radius);
            DebugExtension.DebugCapsule(castStartBackOffsetT + castDirection* castDistance, castStartBackOffsetB + castDirection* castDistance, Color.red, col.radius);
            if (Physics.CapsuleCast(castStartBackOffsetT, castStartBackOffsetB, col.radius, castDirection, out hitInfo, castDistance))
            {
                origin = CastCenterOnCollision(origin, castDirection, hitInfo.distance);
                origin += (hitInfo.normal * surfaceOffset);
            }
            else
            {
                origin += speed;
                break;
            }

        }

        return origin;
    }

    private Vector3 CastCenterOnCollision(Vector3 origin, Vector3 directionCast, float hitInfoDistance)
    {
        return origin + (directionCast.normalized * hitInfoDistance);
    }

    private void Crouch(bool crouch)
    {
        if (crouch)
        {
            col.height = 1.5f;
            col.center = new Vector3(0f, -0.25f, 0f);
        }
        else
        {
            col.height = 2f;
            col.center = Vector3.zero;
        }
    }


}
