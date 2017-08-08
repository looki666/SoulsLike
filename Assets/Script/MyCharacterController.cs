using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyCharacterController : MonoBehaviour {

    Animator animator;
    CharacterBodyCostumization bodyParts;
    Vector3 input;
    Rigidbody rb;

    const float GRAVITY = -9.8f;
    float JumpSpeed;

    public bool isGrounded;
    public bool isJumping;
    public bool isSprinting;
    public bool isSprintToggle;
    float nJumps = 0;

    // Use this for initialization
    void Start () {
        animator = GetComponent<Animator>();
        bodyParts = GetComponent<CharacterBodyCostumization>();
        rb = GetComponent<Rigidbody>();

        Cursor.lockState = CursorLockMode.Locked;
    }

	void Update () {

        input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
        //animator.SetFloat("vertical", input.y);

        RaycastHit hit;
        if (isGrounded =
            Physics.CapsuleCast(transform.position - new Vector3(0, 0.5f, 0), transform.position + new Vector3(0, 0.5f, 0), 0.5f, -transform.up, out hit, 2f))
        {
            nJumps = 0;
            isJumping = false;
        }

        if (Input.GetKeyDown(KeyCode.Space) && (isGrounded || (bodyParts.LegPart.DoubleJump && nJumps <= 1)))
        {
            isJumping = true;
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

        if (!isGrounded)
            speed.y += GRAVITY;

        if (isJumping)
        {
            speed.y += JumpSpeed;
            JumpSpeed -= Time.deltaTime*100;
        }

        Vector3 mov = rb.position + speed * Time.deltaTime;
        rb.MovePosition(mov);
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(transform.position, new Vector3(0.5f, 0.5f, 0.5f));
        Gizmos.DrawSphere(transform.position - new Vector3(0, 0.5f, 0), 0.5f);
        Gizmos.DrawSphere(transform.position + new Vector3(0, 0.5f, 0), 0.5f);

    }


}
