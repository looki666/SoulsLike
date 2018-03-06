using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyCharacterController : MonoBehaviour {

    /**
     * Character Components.
     */
    Animator animator;
    CharacterBodyCostumization bodyParts;
    Vector3 input;
    Rigidbody rb;

    /**
     * Variables that handle falling movement.
     */
    const float ACCEL = 0.5f;
    const float GRAVITY = 9.8f;
    public float fallSpeed = 1.5f;
    public float fallSpeedIncrementer = 0.1f;

    [SerializeField]
    [ReadOnly]
    private bool isGrounded;

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

    Vector3 speed;
    float gravitySpeed;
    Vector3 boxColliderDimensions;
    RaycastHit hit;

    // Use this for initialization
    void Start () {
        isGrounded = false;
        isJumping = false;
        isSprinting = false;
        isCrouching = false;

        isGrounded =
            Physics.BoxCast(transform.position, boxColliderDimensions, -transform.up, out hit, Quaternion.identity, speed.y * Time.deltaTime);
        jump = false;
        gravitySpeed = GRAVITY * fallSpeed;
        boxColliderDimensions = new Vector3(2 / 3f, 1f, 2 / 3f);

        animator = GetComponent<Animator>();
        bodyParts = GetComponent<CharacterBodyCostumization>();
        rb = GetComponent<Rigidbody>();
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

        if (canSprint)
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
            isCrouching = Input.GetKey(KeyCode.LeftControl);
        }
        if (isCrouching)
        {
            transform.localScale = new Vector3(1, 0.5f, 1);
        } else
        {
            transform.localScale = new Vector3(1, 1, 1);
        }

        speed = (transform.forward * input.z + transform.right * input.x);

        if (isSprinting)
        {
            speed *= bodyParts.LegPart.RunningSpeed;
        }
        else
        {
            speed *= bodyParts.LegPart.MovementSpeed;
        }

        if (!jump)
        {
            if (isGrounded =
                Physics.BoxCast(transform.position, boxColliderDimensions, -transform.up, out hit, Quaternion.identity, gravitySpeed * Time.deltaTime)
                )
            {
                //retrieve the normal of the ground in order to go up slopes with crossproduct
                currentJumpNumber = 0;
                isJumping = false;
            }
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
        }

        if (isJumping)
        {
            speed.y += JumpSpeed;
            JumpSpeed = Mathf.Max(0, JumpSpeed - Time.deltaTime * Time.deltaTime * 2000);

        }

        Vector3 mov = transform.position + speed * Time.deltaTime;


        /**
         * Spherecast in the direction of the movement.
         */
        float radius = .6f;
        Vector3 v2 = new Vector3(speed.x, 0f, speed.z);
        if (Physics.SphereCast(transform.position, radius, v2.normalized, out hit, speed.magnitude * Time.deltaTime))
        {
            Debug.Log("Collision");
            Vector3 dirToCollision = (hit.collider.ClosestPointOnBounds(transform.position) - transform.position);
            dirToCollision.y = 0f;
            mov = transform.position + dirToCollision.normalized * (dirToCollision.magnitude - radius);
            mov.y += speed.y * Time.deltaTime;
            Debug.DrawRay(hit.collider.ClosestPointOnBounds(transform.position), -dirToCollision.normalized * (dirToCollision.magnitude - radius), Color.cyan, 10f);
        }
        
        /**
         * Apply calculated movement; 
         */
        transform.position = mov;
        bodyParts.SetMovementState(speed, isJumping, isSprinting);
    }

    /**
     * Helper gizmos.
     */ 
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 offset = new Vector3(0, -boxColliderDimensions.y / 2, 0);
        Gizmos.DrawCube(transform.position + offset, boxColliderDimensions);

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position, .6f);
    }
    

}
