using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlatformSpeed : MonoBehaviour {

    Rigidbody rb;
	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        RaycastHit info;
        Physics.Raycast(transform.position, -transform.up, out info, 4f);
        rb.MovePosition(rb.position + info.transform.GetComponent<Rigidbody>().velocity * Time.deltaTime);
    }
}
