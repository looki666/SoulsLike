using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmDynAnimation : MonoBehaviour {

    public Transform body;
    public Transform camera;
    private CameraController cameraController;

    public float bounceFactor = 20;
    public float wobbleFactor = 1;

    public float maxTranslation = 0.05f;
    public float maxRotationDegrees = 5;

    private Quaternion oldBoneWorldRotation;
    private Quaternion animatedBoneWorldRotation;

    // Use this for initialization
    void Awake()
    {
        oldBoneWorldRotation = camera.rotation;
        cameraController = camera.GetComponent<CameraController>();
    }

    // Update is called once per frame
    void LateUpdate () {
        if (cameraController.target == null)
        {
            JiggleBonesUpdate();
        }

    }

    //Vector3.SmoothDamp

    void JiggleBonesUpdate()
    {
        Debug.DrawRay(transform.position, transform.forward, Color.red);
        Debug.DrawRay(camera.position, camera.forward, Color.green);

        //Mesh has just been animated
        animatedBoneWorldRotation = transform.rotation;

        Quaternion goalRotation = Quaternion.SlerpUnclamped(oldBoneWorldRotation, camera.rotation, Time.deltaTime * wobbleFactor);

        transform.rotation = Quaternion.RotateTowards(animatedBoneWorldRotation, goalRotation, maxRotationDegrees);

        oldBoneWorldRotation = transform.rotation;
    }


}
