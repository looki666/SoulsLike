using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BestKinematicCharacterController : MonoBehaviour
{

    /**
     * Character Components.
     */
    Animator animator;
    CharacterBodyCostumization bodyParts;
    Rigidbody rb;
    CapsuleCollider col;

    public bool DEBUG = false;
    public bool dePenetrate = false;
    public bool moveEvenIfFailedCollision = false;

    /**
     * Variables that handle falling movement.
     */
    const float ACCEL = 0.5f;
    const float GRAVITY = 9.8f;
    public float fallSpeed = 1.5f;
    public float fallSpeedIncrementer = 0.1f;

    public float radiusOfLock = 3f;
    [SerializeField]
    [ReadOnly]
    private bool isLocked;
    private bool pressedLocking;

    [SerializeField]
    [ReadOnly]
    private bool isDodging;

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

    private const string BlockingAnimationState = "Blocking";
    private const string WalkingAnimationState = "Walking";
    private const string SprintingAnimationState = "Sprinting";

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
        isDodging = false;
        isLocked = false;
        pressedLocking = false;

        jump = false;
        gravitySpeed = GRAVITY * fallSpeed;
        boxColliderDimensions = new Vector3(2 / 3f, 1f, 2 / 3f);

        bodyParts = GetComponent<CharacterBodyCostumization>();
        animator = bodyParts.ArmsPart.animator;
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();

        nearbyColliders = new Collider[16];
    }

    void Update()
    {

        //Handle Block animation state.
        if (Input.GetKeyDown(KeyCode.Q))
        {
            animator.SetBool(BlockingAnimationState, true);
        }
        else if (Input.GetKeyUp(KeyCode.Q))
        {
            animator.SetBool(BlockingAnimationState, false);
        }

        //Check if is in locked mode.
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            pressedLocking = true;
        }

        //Movement input.
        input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

        //Dodging input.
        if (Input.GetKeyDown(KeyCode.E))
        {
            isDodging = true;
        }

        //Jump input and is grounded or can do multiple jumps.
        if (Input.GetKeyDown(KeyCode.Space) && (isGrounded || (currentJumpNumber < bodyParts.LegPart.JumpNumber)))
        {
            jump = true;
            JumpSpeed = bodyParts.LegPart.JumpHeight;
            gravitySpeed = GRAVITY * fallSpeed;
            currentJumpNumber++;
        }

        //If grounded and can sprint, handle sprint input.
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

        //Crouching input.
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
        //Crouch
        Crouch(isCrouching);
    }

    /*
     * Physics Update.
     */
    private void FixedUpdate()
    {
        walkingVector = transform.forward;
        sideWalkingVector = transform.right;
        Vector3 platformSpeed = Vector3.zero;

        //Check grounding.
        RaycastHit groundInfo;
        if (becameGrounded = CheckGrounding(out groundInfo))
        {
            platformSpeed = AddSpeedFromPlatform(groundInfo.collider);
            GetWalkingVector(groundInfo, out walkingVector, out sideWalkingVector);
            if (DEBUG)
            {
                Debug.DrawRay(transform.position, walkingVector * 5, Color.magenta);
            }
        }

        bool justLanded = becameGrounded && !isGrounded;
        bool justFell = (!isJumping || !jump) && !becameGrounded && isGrounded;

        //Can check here the landing frame.
        if (justLanded)
        {
        }

        //Can check here the falling frame.
        if (justFell)
        {
        }

        isGrounded = becameGrounded;

        speed = (walkingVector * input.z + sideWalkingVector * input.x);

        //Check speed magnitude.
        if (isDodging)
        {
            speed *= bodyParts.LegPart.FlashStepSpeed;
            isDodging = false;
        }
        else if (isSprinting)
        {
            speed *= bodyParts.LegPart.RunningSpeed;
        }
        else
        {
            speed *= bodyParts.LegPart.MovementSpeed;
        }


        //Set animation type for walking/sprinting
        if (speed.magnitude > 0)
        {
            if (isSprinting)
            {
                animator.SetBool(SprintingAnimationState, true);
            }
            else
            {
                animator.SetBool(WalkingAnimationState, true);
                animator.SetBool(SprintingAnimationState, false);
            }
        }
        else
        {
            animator.SetBool(WalkingAnimationState, false);
            animator.SetBool(SprintingAnimationState, false);
        }

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

        Vector3 disp = Vector3.zero;
        if (dePenetrate)
        {
            int numbOfNearbyCols = Physics.OverlapSphereNonAlloc(rb.position, 1 + 0.1f, nearbyColliders, ~(1 << 8));

            if (DEBUG)
            {
                if (numbOfNearbyCols > 0)
                {
                    DebugExtension.DebugWireSphere(rb.position, Color.green, 1 + 0.1f);
                }
                else
                {
                    DebugExtension.DebugWireSphere(rb.position, Color.red, 1 + 0.1f);
                }
            }
            disp = DePenetrateCollisions(ref speed, numbOfNearbyCols);
        }

        bodyParts.SetMovementState(speed, isJumping, isSprinting);
        ContinuosCollisionDetection((platformSpeed + speed) * Time.deltaTime);
    }

    /**
     * Get forward and sideways vector when standing on a slope.
     */
    private void GetWalkingVector(RaycastHit groundInfo, out Vector3 walkingDir, out Vector3 sideDir)
    {
        Vector3 n = groundInfo.normal.normalized;
        sideDir = Vector3.Cross(n, transform.forward.normalized);
        walkingDir = Vector3.Cross(sideDir, n);
    }

    /**
     * When the character gets on top of a platform it should add the platform speed to its own speed.
     */
    private Vector3 AddSpeedFromPlatform(Collider col)
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
            if (colRb != null)
            {
                platformSpeed = colRb.velocity;
            }
        }

        return platformSpeed;
    }

    /*
     * Verify if character would penetrate and try to move just enough to not collide.
     */
    private void ContinuosCollisionDetection(Vector3 movementSpeed)
    {
        Vector3 originalVelocity = movementSpeed;
        Vector3 origin = rb.position;
        int attempts;
        Color startColor = Color.blue;
        for (attempts = 0; attempts < maxCollisionAttempts; attempts++)
        {
            Vector3 originTop = origin;
            float offSetCapsule = col.height / 2 - col.radius;
            originTop.y += offSetCapsule;

            Vector3 originBottom = origin;
            originBottom.y -= offSetCapsule;

            Vector3 prevOrigin = origin;

            float castDistance = movementSpeed.magnitude + backstepOffset;
            Vector3 castDirection = movementSpeed.normalized;
            Vector3 castStartBackOffsetT = originTop - (castDirection * backstepOffset);
            Vector3 castStartBackOffsetB = originBottom - (castDirection * backstepOffset);

            if (DEBUG)
            {
                startColor.r += 0.2f;
                DebugExtension.DebugArrow(castStartBackOffsetT, castDirection * castDistance, startColor, 2f);
                DebugExtension.DebugArrow(castStartBackOffsetB, castDirection * castDistance, startColor, 2f);
            }
            RaycastHit hitInfo;
           if (Physics.CapsuleCast(castStartBackOffsetT, castStartBackOffsetB, col.radius, castDirection, out hitInfo, castDistance, ~(1<<8)))
            {
                origin = CastCenterOnCollision(origin, castDirection, hitInfo.distance);
                origin += (hitInfo.normal * surfaceOffset);

                if (DEBUG)
                {
                    Debug.DrawRay(hitInfo.point, hitInfo.normal, Color.cyan, 2f);
                    DebugExtension.DebugArrow(origin, movementSpeed, Color.black, 2f);
                }

                float remainingDistance = Mathf.Max(0, castDistance - Vector3.Distance(prevOrigin, origin));

                Vector3 remainingSpeed = castDirection * remainingDistance;
                movementSpeed = remainingSpeed - Vector3.Project(remainingSpeed, hitInfo.normal);

                if (DEBUG)
                {
                    DebugExtension.DebugArrow(origin, movementSpeed, Color.white, 2f);
                }

                if (movementSpeed.magnitude <= minVelocityBreak) break;
            }
            else
            {

                origin += movementSpeed;
                break;
            }
        }

        bool failedCollision = attempts >= maxCollisionAttempts;
        if (failedCollision) Debug.LogWarning("Failed collision handling");

        if (!moveEvenIfFailedCollision && failedCollision)
        {
            Debug.LogWarning("Aborting movement");
        }
        else
        {
            rb.MovePosition(origin);
        }

    }

    /*
     * Rotate forward and sideways toward the lock target.
     */
    private void RotateToLookAtLock(ref Vector3 fwrd, ref Vector3 sdws)
    {

    }

    /*
     * If character would penetrate a volume then move it AND depenetrate it by the right ammount.
     */
    private Vector3 DePenetrateCollisions(ref Vector3 speed, int numbOfNearbyCols)
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
                if (Vector3.Dot(dir, Vector3.up) < groundingAngle)
                {
                    dir.y = 0f;
                    dir.Normalize();
                }
                // Get outta that collider!
                displacement += dir * dist;

                // Crop down the velocity component which is in the direction of penetration
                if (Vector3.Dot(speed, dir) < 0)
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
    private bool CheckGrounding(out RaycastHit hitInfo)
    {
        bool wasGrounded = isGrounded;
        isSlopeSliding = false;

        if (Physics.SphereCast(rb.position, 0.5f, -transform.up, out hitInfo, col.height / 4))
        {
            if (Vector3.Dot(hitInfo.normal, Vector3.up) > groundingAngle)
            {
                currentJumpNumber = 0;
                isJumping = false;
                return true;
            }
            else
            {
                isSlopeSliding = true;
                return wasGrounded;
            }

        }
        return false;
    }

    /**
     * Move center point to a point at the end of a direction and distance.
     */
    private Vector3 CastCenterOnCollision(Vector3 origin, Vector3 directionCast, float hitInfoDistance)
    {
        return origin + (directionCast.normalized * hitInfoDistance);
    }

    /*
     * Modify Collider and mesh if crouching
     */
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