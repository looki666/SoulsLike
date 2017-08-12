using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyCharacterController : MonoBehaviour {

    Animator animator;
    CharacterBodyCostumization bodyParts;
    Vector3 input;
    Rigidbody rb;

    const float GRAVITY = 9.8f;
    float JumpSpeed;

    public bool isGrounded;
    public bool jump;
    public bool isJumping;
    public bool isSprinting;
    public bool isSprintToggle;
    float nJumps = 0;

    // Use this for initialization
    void Start () {
        isGrounded = false;
        jump = false;
        isJumping = false;
        isSprinting = false;

        animator = GetComponent<Animator>();
        bodyParts = GetComponent<CharacterBodyCostumization>();
        rb = GetComponent<Rigidbody>();

        //Cursor.lockState = CursorLockMode.Locked;
    }

	void Update () {

        input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        //animator.SetFloat("vertical", input.y);
        if (Input.GetKeyDown(KeyCode.Space) && (isGrounded || (bodyParts.LegPart.DoubleJump && nJumps <= 1)))
        {
            jump = true;
            JumpSpeed = bodyParts.LegPart.JumpHeight;
            nJumps++;
        }

        if (isSprintToggle)
        {
            if(Input.GetKeyDown(KeyCode.LeftShift))
            isSprinting = !isSprinting;
        }
        else
            isSprinting = Input.GetKey(KeyCode.LeftShift);

        if (Input.GetKeyDown("escape"))
            Cursor.lockState = CursorLockMode.None;

      
    }


    void FixedUpdate()
    {

        Vector3 speed = (transform.forward * input.z + transform.right * input.x);

        if (isSprinting)
            speed *= bodyParts.LegPart.RunningSpeed;
        else
            speed *= bodyParts.LegPart.MovementSpeed;

        RaycastHit hit;
        if (isGrounded =
            Physics.BoxCast(rb.position, new Vector3(2 / 3f, 1f, 2 / 3f), -transform.up, out hit, Quaternion.identity, GRAVITY * Time.deltaTime)
            )
        {
            //retrieve the normal of the ground in order to go up slopes with crossproduct
            nJumps = 0;
            isJumping = false;
        }

        if (!isGrounded)
            speed.y -= GRAVITY;

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

        Vector3 mov = rb.position + speed * Time.deltaTime;

        //collisions
        if (Physics.BoxCast(rb.position, new Vector3(5 / 6f, 1f, 5 / 6f), speed.normalized, out hit, Quaternion.identity, speed.magnitude * Time.deltaTime))
        {
            Debug.Log("Collision");
            mov = rb.position + (hit.transform.position - rb.position);
        }

        nextPositionGravity = rb.position -transform.up * GRAVITY * Time.deltaTime;
        nextPositionMovement = rb.position + speed * Time.deltaTime;

        rb.MovePosition(mov);
    }

    Vector3 nextPositionGravity;
    Vector3 nextPositionMovement;

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(nextPositionGravity, new Vector3(2 / 3f, 1f, 2 / 3f));
        Gizmos.color = Color.blue;
        Gizmos.DrawCube(nextPositionMovement, new Vector3(5 / 6f, 1f, 5 / 6f));

    }
    

}
