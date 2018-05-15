using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    Vector2 mouseLook;
    public Transform target = null;
    Rigidbody parentRb;
    public bool UseMouseLook { get; set; }

    // Use this for initialization
    void Start () {
        parentRb = GetComponentInParent<Rigidbody>();
        UseMouseLook = true;
    }
	
	// Update is called once per frame
	void Update () {
        if (target == null && UseMouseLook)
        {
            Vector2 mouse = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
            mouse *= 2f;
            mouseLook += mouse;
            transform.localRotation = Quaternion.AngleAxis(Mathf.Clamp(-mouseLook.y, -90, 90), Vector3.right); //up and down

            Vector3 eulerRot = new Vector3(0f, mouseLook.x, 0f);
            transform.parent.localEulerAngles = eulerRot;
        }
        else if(target != null)
        {
            Vector3 dirXZ = new Vector3(target.position.x - transform.parent.position.x, 0f, target.position.z - transform.parent.position.z);
            transform.parent.rotation = Quaternion.LookRotation(dirXZ);
        }
        
    }

    public void LockCameraOnTarget(Transform target)
    {
        if (this.target != null && target == null)
        {
            mouseLook.x = transform.parent.eulerAngles.y;
        }
        this.target = target;
    }

}
