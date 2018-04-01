using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlatform : MonoBehaviour {

    public Vector3 direction;
    public Vector3 currDir;
    public float speed;

    private Rigidbody rb;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	void Update () {
        currDir = direction * Mathf.Sin(speed);
        rb.MovePosition(rb.position + currDir * Time.deltaTime);
    }
}
