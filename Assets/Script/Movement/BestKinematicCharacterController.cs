using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BestKinematicCharacterController : MonoBehaviour
{
    Rigidbody rb;
    CapsuleCollider col;

    /**
     * Character Components.
     */
    private Animator animator;
    private CharacterBodyCostumization bodyParts;
    private CameraController myCamera;
    private KinematicMotor kinMotor;


    public float radiusOfLock = 3f;
    [SerializeField]
    [ReadOnly]
    private bool isLocked;
    private bool pressedLocking;

    private bool dodge;
    [SerializeField]
    [ReadOnly]
    private bool isDodging;

    /**
     * Variables that handle jumping movement.
     */
    [SerializeField]
    [ReadOnly]
    private float JumpSpeed;
    private bool jump;
    public bool canJump = true;

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
    private bool becameGrounded;

    [SerializeField]
    [ReadOnly]
    Vector3 speed;

    Vector3 input;

    RaycastHit hit;
    Collider[] nearbyEnemies;
    Transform closestEnemy;

    private const string WalkingAnimationState = "Walking";
    private const string FightStanceAnimationState = "FightStance";
    private const string SprintingAnimationState = "Sprinting";
    private const string JumpFallAnimationState = "onAir";

    private const int layerMaskCollision = ~((1 << 8) | (1 << 13));

    float timerDodge = 0f;

    // Use this for initialization
    void Start()
    {
        becameGrounded = false;
        isSprinting = false;
        isCrouching = false;
        dodge = false;
        isDodging = false;
        isLocked = false;
        pressedLocking = false;
        jump = false;

        closestEnemy = null;
        nearbyEnemies = new Collider[5];

        myCamera = GetComponentInChildren<CameraController>();
        bodyParts = GetComponent<CharacterBodyCostumization>();
        if (bodyParts.ArmsPart != null)
        {
            animator = bodyParts.ArmsPart.animator;
        }
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();

        kinMotor = GetComponent<KinematicMotor>();
        kinMotor.LayerMaskCollision = layerMaskCollision;
    }

    void Update()
    {
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
            dodge = true;
        }

        //Jump input and is grounded or can do multiple jumps.
        if (Input.GetKeyDown(KeyCode.Space) && (kinMotor.IsGrounded || (currentJumpNumber < bodyParts.LegPart.JumpNumber)))
        {
            jump = true;
            kinMotor.JumpSpeed = bodyParts.LegPart.JumpHeight;
            currentJumpNumber++;
        }

        //If grounded and can sprint, handle sprint input.
        if (kinMotor.IsGrounded && canSprint)
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
        speed = input;
        bool justLanded = becameGrounded && !kinMotor.IsGrounded;
        bool justFell = (!kinMotor.IsJumping || !jump) && !becameGrounded && kinMotor.IsGrounded;

        becameGrounded = kinMotor.IsGrounded;

        //Can check here the landing frame.
        if (justLanded)
        {
            animator.SetBool(JumpFallAnimationState, false);
        }

        //Can check here the falling frame.
        if (justFell)
        {
            animator.SetBool(JumpFallAnimationState, true);
        }

        /*
         * If pressed lock, check for enemies close by and lock camera to closest
         */
        if (pressedLocking)
        {
            //Reset variables when clicking to lock
            closestEnemy = null;
            pressedLocking = false;
            //Try to lock to the closest enemy
            isLocked = LockCameraToClosestEnemy();
        }

        /*
         * If is in locked combat but moves too far away, disengage camera lock
         */
        if (isLocked)
        {
            float distance = Vector3.Distance(closestEnemy.transform.position, rb.position);
            if (distance > 5f)
            {
                isLocked = false;
                closestEnemy = null;
                myCamera.LockCameraOnTarget(null);
            }
        }

        //Set arms animation to locked 
        animator.SetBool(FightStanceAnimationState, isLocked);

        /*
         * Dodge press;
         */
        if (dodge)
        {
            dodge = false;
            isDodging = true;
        }

        /*
         * Check speed magnitude from leg part.
         * Dodge / Sprint / Normal run
         */
        if (isDodging)
        {
            timerDodge += Time.deltaTime;
            //timerDodge starts as 0 and ends at 200
            speed *= (bodyParts.LegPart.FlashStepSpeed - (timerDodge * timerDodge * 10 * 40));
            if (timerDodge >= .05f)
            {
                isDodging = false;
                timerDodge = 0f;
            }
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

        kinMotor.Move(speed);
        bodyParts.SetMovementState(speed, kinMotor.IsJumping, isSprinting && speed.magnitude > 0);
    }

    /**
     * Checks if there is a close enemy nearby,
     * If there is 1 or more, camera lock to it
     */
    private Boolean LockCameraToClosestEnemy()
    {
        Boolean willLock = false;

        int numNearEnemies = Physics.OverlapSphereNonAlloc(rb.position, radiusOfLock, nearbyEnemies, 1 << 9);
        float closestDistance = radiusOfLock;
        for (int i = 0; i < numNearEnemies; i++)
        {
            Vector3 targetDir = nearbyEnemies[i].transform.position - rb.position;
            if (Vector3.Angle(targetDir, transform.forward) < 45f)
            {
                float distance = Vector3.Distance(nearbyEnemies[i].transform.position, rb.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = nearbyEnemies[i].transform;
                    willLock = true;
                }
            }
        }

        myCamera.LockCameraOnTarget(closestEnemy);

        return willLock;
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