using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KinematicMotor : MonoBehaviour {

    /*
     * Components
     */
    Rigidbody rb;
    CapsuleCollider col;

    /*
     * Settings for Collision
     */
    public bool dePenetrate = false;
    public bool moveEvenIfFailedCollision = false;

    public int LayerMaskCollision { get; set; }
    public float groundingAngle = 0.5f;

    Collider[] nearbyColliders;

    /*
     * Grounding variables
     */
    [SerializeField]
    [ReadOnly]
    private bool isGrounded;
    public bool IsGrounded
    {
        get { return isGrounded; }
    }

    public bool Jump { get; set; }
    public bool IsJumping { get; set; }
    float currentJumpNumber;

    /*
     * Collision handler.
     */
    public int maxCollisionAttempts = 20;
    float surfaceOffset = .0001f;
    float backstepOffset = .001f;
    float minVelocityBreak = .001f;

    /**
     * Variables that handle falling movement.
     */
    const float GRAVITY = 9.8f;
    public float fallSpeed = 1.5f;
    public float fallSpeedIncrementer = 0.1f;
    public float JumpSpeed { get; set; }


    [SerializeField]
    [ReadOnly]
    float gravitySpeed;
    private Vector3 movementSpeed;

    Vector3 walkingVector;
    Vector3 sideWalkingVector;


    // Use this for initialization
    void Start () {
        isGrounded = false;
        IsJumping = false;
        gravitySpeed = GRAVITY * fallSpeed;
        nearbyColliders = new Collider[16];

        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
    }

    public void Move(Vector3 movement)
    {
        Vector3 movNorm = movement.normalized;
        movementSpeed = (walkingVector * movNorm.z + sideWalkingVector * movNorm.x) * movement.magnitude;
    }

    private void FixedUpdate()
    {
        walkingVector = transform.forward;
        sideWalkingVector = transform.right;
        Vector3 platformSpeed = Vector3.zero;

        //If not grounded add falling speed
        if (!isGrounded)
        {
            gravitySpeed += fallSpeedIncrementer;
            movementSpeed.y -= gravitySpeed;
        }

        //Check grounding.
        RaycastHit groundInfo;
        if (isGrounded = CheckGrounding(out groundInfo))
        {
            platformSpeed = AddSpeedFromPlatform(groundInfo.collider);
            GetWalkingVector(groundInfo, out walkingVector, out sideWalkingVector);
        }

        //If pressed jump, set variables for behaviour
        if (Jump)
        {
            Jump = false;
            IsJumping = true;
            isGrounded = false;
        }

        //If is Jumping add upwards movement
        if (IsJumping)
        {
            movementSpeed.y += JumpSpeed;
            JumpSpeed = Mathf.Max(0, JumpSpeed - Time.deltaTime * Time.deltaTime * 2000);
        }

        ContinuosCollisionDetection((platformSpeed + movementSpeed) * Time.deltaTime);
    }

    /**
     * Move center point to a point at the end of a direction and distance.
     */
    private Vector3 CastCenterOnCollision(Vector3 origin, Vector3 directionCast, float hitInfoDistance)
    {
        return origin + (directionCast.normalized * hitInfoDistance);
    }

    /*
     * Verify if character would penetrate and try to move just enough to not collide.
     */
    private void ContinuosCollisionDetection(Vector3 movementSpeed)
    {
        Vector3 origin = rb.position;
        int attempts;
        Color startColor = Color.blue;
        for (attempts = 0; attempts < maxCollisionAttempts; attempts++)
        {
            Vector3 prevOrigin = origin;
            float castDistance = movementSpeed.magnitude + backstepOffset;
            Vector3 castDirection = movementSpeed.normalized;
            Vector3 castStartBackOffset = origin - (castDirection * backstepOffset);

            float offSetCapsule = col.height / 2 - col.radius;
            Vector3 castStartBackOffsetTop = castStartBackOffset + transform.up * offSetCapsule;
            Vector3 castStartBackOffsetBot = castStartBackOffset - transform.up * offSetCapsule;

            RaycastHit hitInfo;
            if (Physics.CapsuleCast(castStartBackOffsetTop, castStartBackOffsetBot, col.radius, castDirection, out hitInfo, castDistance, LayerMaskCollision))
            {
                if (hitInfo.collider.isTrigger)
                {
                    origin += movementSpeed;
                    break;
                }
                origin = CastCenterOnCollision(castStartBackOffset, castDirection, hitInfo.distance);
                origin += (hitInfo.normal * surfaceOffset);

                float remainingDistance = Mathf.Max(0, castDistance - Vector3.Distance(prevOrigin, origin));

                Vector3 remainingSpeed = castDirection * remainingDistance;
                movementSpeed = remainingSpeed - Vector3.Project(remainingSpeed, hitInfo.normal);

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
            Vector3 disp = Vector3.zero;
            if (dePenetrate)
            {
                int numbOfNearbyCols = Physics.OverlapSphereNonAlloc(rb.position, 1 + 0.1f, nearbyColliders, LayerMaskCollision);
                disp = DePenetrateCollisions(ref movementSpeed, numbOfNearbyCols);
            }


            rb.MovePosition(origin + disp);
        }

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


    /**
    * Check if Character is grounded by spherecasting the ground.
    */
    private bool CheckGrounding(out RaycastHit hitInfo)
    {

        if (Physics.SphereCast(rb.position, 0.5f, -transform.up, out hitInfo, col.height / 4 + 0.01f, LayerMaskCollision))
        {
            if (hitInfo.collider.isTrigger)
            {
                return false;
            }
            if (Vector3.Dot(hitInfo.normal, Vector3.up) > groundingAngle)
            {
                currentJumpNumber = 0;
                IsJumping = false;
                return true;
            }

        }
        return false;
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

}
