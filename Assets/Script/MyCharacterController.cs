using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyCharacterController : MonoBehaviour {

    Animator animator;
    CharacterBodyCostumization bodyParts;
    Vector3 input;
    Rigidbody rb;

    Vector3 GRAVITY = new Vector3(0, -9.8f, 0);

    public bool isGrounded;
    float nJumps = 0;

    // Use this for initialization
    void Start () {
        animator = GetComponent<Animator>();
        bodyParts = GetComponent<CharacterBodyCostumization>();
        rb = GetComponent<Rigidbody>();

        Cursor.lockState = CursorLockMode.Locked;
    }
	
	// Update is called once per frame
	void Update () {

        input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        //animator.SetFloat("vertical", input.y);

        if (Input.GetKeyDown(KeyCode.Space) && (isGrounded || (bodyParts.LegPart.DoubleJump && nJumps <= 1)))
        {
            transform.Translate(0, bodyParts.LegPart.JumpHeight, 0);
            nJumps++;
        }

        if (Input.GetKeyDown("escape"))
            Cursor.lockState = CursorLockMode.None;

        RaycastHit hit;
        if(isGrounded = 
            Physics.CapsuleCast(transform.position - new Vector3(0,0.5f,0), transform.position + new Vector3(0, 0.5f, 0), 0.5f, -transform.up, out hit, 0.25f) )
        {
            nJumps = 0;
        }
      
    }


    void FixedUpdate()
    {
        Vector3 speed = (transform.forward * input.z + transform.right * input.x) * bodyParts.LegPart.MovementSpeed;

        if(!isGrounded)
            speed += GRAVITY;

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
