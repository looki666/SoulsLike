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
    Collider[] nearbyColliders;

    Vector3 walkingVector;
    Vector3 sideWalkingVector;

    /*
     * Collision handler.
     */
    public bool moveEvenIfFailedCollision = true;
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

        bodyParts = GetComponent<CharacterBodyCostumization>();
        animator = bodyParts.ArmsPart.animator;
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
        Vector3 platformSpeed = Vector3.zero;

        int numbOfNearbyCols = Physics.OverlapSphereNonAlloc(rb.position, 1 + 0.1f, nearbyColliders);
        //DebugExtension.DebugWireSphere(rb.position, Color.green, 1 + 0.1f);

        if (numbOfNearbyCols > 1) //if its colliding with something check grounding
        {
            //Check grounding
            RaycastHit groundInfo;
            if (becameGrounded = CheckGrounding(out groundInfo))
            {
                platformSpeed = AddSpeedFromPlatform(groundInfo.collider);
                GetWalkingVector(groundInfo, out walkingVector, out sideWalkingVector);
                Debug.DrawRay(transform.position, walkingVector * 5, Color.magenta);
            }
        }
        else
        {
            becameGrounded = false;
        }

        speed = (walkingVector * input.z + sideWalkingVector * input.x);

        if (isSprinting)
        {
            speed *= bodyParts.LegPart.RunningSpeed;
        }
        else
        {
            speed *= bodyParts.LegPart.MovementSpeed;
        }

        if(speed.magnitude > 0)
        {
            if (isSprinting)
            {
                animator.SetBool("Sprinting", true);
            } else
            {
                animator.SetBool("Walking", true);
                animator.SetBool("Sprinting", false);
            }
        } else
        {
            animator.SetBool("Walking", false);
            animator.SetBool("Sprinting", false);
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

        if (!isGrounded || (isSlopeSliding && !isGrounded))
        {
            gravitySpeed += fallSpeedIncrementer;
            speed.y -= gravitySpeed;
        }

        Vector3 disp = Vector3.zero;
        bodyParts.SetMovementState(speed, isJumping, isSprinting);
        Vector3 newPosition = ContinuosCollisionDetection((platformSpeed + speed) * Time.deltaTime);
        rb.MovePosition(newPosition);
    }

    private Vector3 ContinuosCollisionDetection(Vector3 movementSpeed)
    {
        Vector3 origin = rb.position;
        int attempts;
        for (attempts = 0; attempts < maxCollisionAttempts; attempts++)
        {
            Vector3 originTop = origin;
            originTop.y += col.height / 4;

            Vector3 originBottom = origin;
            originBottom.y -= col.height / 4;

            Vector3 prevOrigin = origin;

            float castDistance = movementSpeed.magnitude + backstepOffset;
            Vector3 castDirection = movementSpeed.normalized;
            Vector3 castStartBackOffsetT = originTop - (castDirection * backstepOffset);
            Vector3 castStartBackOffsetB = originBottom - (castDirection * backstepOffset);

            RaycastHit hitInfo;
            DebugExtension.DebugPoint(originTop, Color.red);
            DebugExtension.DebugPoint(originBottom, Color.red);
            DebugExtension.DebugWireSphere(castStartBackOffsetT + castDirection, Color.red, col.radius);
            DebugExtension.DebugWireSphere(castStartBackOffsetB + castDirection, Color.red, col.radius);
           
            if (Physics.CapsuleCast(castStartBackOffsetT, castStartBackOffsetB, col.radius, castDirection, out hitInfo, castDistance))
            {
                Debug.DrawRay(origin, castDirection * hitInfo.distance, Color.cyan, 1f);

                origin = CastCenterOnCollision(origin, castDirection, hitInfo.distance);
                origin += (hitInfo.normal * surfaceOffset);

                float remainingDistance = Mathf.Max(0, castDistance - Vector3.Distance(prevOrigin, origin));

                RaycastHit hit;
                var p = hitInfo.point + (origin - hitInfo.point).normalized * 0.01f;
                Physics.Raycast(p, -hitInfo.normal, out hit, 0.1f);

                Debug.DrawRay(p, -hitInfo.normal, Color.blue, 5f);

                RaycastHit hit2;
                p = hitInfo.point - (origin - hitInfo.point).normalized * 0.01f;
                Physics.Raycast(p, hitInfo.normal, out hit2, 0.1f);


                Debug.DrawRay(p, hitInfo.normal, Color.magenta, 5f);

                Vector3 remainingSpeed = castDirection * remainingDistance;
                movementSpeed = remainingSpeed - Vector3.Project(remainingSpeed, hit.normal);
                if (!hitInfo.collider.name.Equals("Plane"))
                {
                    Debug.Log(hit.normal + " -- " + hit2.normal);
                    Debug.DrawRay(hitInfo.point, hit.normal, Color.cyan, 5f);
                    Debug.DrawRay(hitInfo.point, hit2.normal, Color.yellow, 5f);
                    DebugExtension.DebugWireSphere(hitInfo.point, 0.02f, 10f);
                    //DebugExtension.DebugWireSphere(hitInfo.point + hit.normal, 0.05f, 10f);
                    //Debug.DrawRay(origin, movementSpeed, Color.yellow, 5f);
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
            return rb.position;
        }
        else
        {
            return origin;
        }
    }

    /**
    * Check if Character is grounded by spherecasting the ground.
    */
    private bool CheckGrounding(out RaycastHit hitInfo)
    {
        bool wasGrounded = isGrounded;
        isSlopeSliding = false;
        //DebugExtension.DebugWireSphere(transform.position + -transform.up * (col.height / 4 + speed.magnitude), Color.blue, 0.5f, 10f);
        if (Physics.SphereCast(transform.position, 0.5f, -transform.up, out hitInfo, col.height / 4 + speed.magnitude))
        {
            //Only walk on non-steep ground
            //Debug.Log(Vector3.Dot(hitInfo.normal, Vector3.up) + ">>" + groundingAngle);
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

    private void GetWalkingVector(RaycastHit groundInfo, out Vector3 walkingDir, out Vector3 sideDir)
    {
        Vector3 n = groundInfo.normal.normalized;
        sideDir = Vector3.Cross(n, transform.forward.normalized);
        walkingDir = Vector3.Cross(sideDir, n);
    }

    private Vector3 CastCenterOnCollision(Vector3 origin, Vector3 directionCast, float hitInfoDistance)
    {
        return origin + (directionCast.normalized * hitInfoDistance);
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
