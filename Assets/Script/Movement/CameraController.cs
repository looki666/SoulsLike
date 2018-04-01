using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    Vector2 mouseLook;
    public Transform target = null;
    public bool wasLocked = false;
    Rigidbody parentRb;

	// Use this for initialization
	void Start () {
        parentRb = GetComponentInParent<Rigidbody>();
    }
	
	// Update is called once per frame
	void Update () {
        if (target == null)
        {
            Vector2 mouse = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
            mouse *= 2f;
            mouseLook += mouse;
            transform.localRotation = Quaternion.AngleAxis(Mathf.Clamp(-mouseLook.y, -90, 90), Vector3.right); //up and down

            Vector3 eulerRot = new Vector3(0f, mouseLook.x, 0f);
            transform.parent.localEulerAngles = eulerRot;
        }
        else
        {
            transform.parent.LookAt(target);
        }
        
    }

    public void LockCameraOnTarget(Transform target)
    {
        if (this.target != null && target == null)
        {
            Debug.Log("==== Un-Target ====");
            wasLocked = true;
            mouseLook.x = transform.parent.eulerAngles.y;
        }
        this.target = target;
    }

    private void FixedUpdates()
    {
        if (target == null)
        {
            Vector2 mouse = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
            mouse *= 2f;
            mouseLook += mouse;
            if (wasLocked)
            {
                mouseLook = Vector2.zero;
                wasLocked = false;
            }

            transform.localRotation = Quaternion.AngleAxis(Mathf.Clamp(-mouseLook.y, -90, 90), Vector3.right); //up and down
            parentRb.MoveRotation(Quaternion.AngleAxis(mouseLook.x, transform.parent.up));
        }else
        {
            wasLocked = true;
            Vector3 dirXZ = new Vector3(target.position.x - transform.parent.position.x, 0f, target.position.z - transform.parent.position.z);
            Vector3 forwardXZ = Vector3.ProjectOnPlane(transform.forward, transform.up);
            parentRb.MoveRotation(Quaternion.LookRotation(dirXZ));
        }
    }


}
