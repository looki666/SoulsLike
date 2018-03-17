using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    Vector2 mouseLook;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Vector2 mouse = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        mouse *= 2f;
        mouseLook += mouse;

        transform.localRotation = Quaternion.AngleAxis(Mathf.Clamp(-mouseLook.y, -90, 90), Vector3.right); //up and down
        transform.parent.localRotation = Quaternion.AngleAxis(mouseLook.x, transform.parent.up);
	}
}
