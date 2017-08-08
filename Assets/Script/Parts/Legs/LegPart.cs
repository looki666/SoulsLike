using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegPart : MonoBehaviour {

    public BasicLegScriptable legData;

    public float MovementSpeed { get { return legData.movementSpeed; } set { legData.movementSpeed = value; } }
    public float RunningSpeed { get { return legData.runningSpeed; } set { legData.runningSpeed = value; } }
    public float JumpHeight { get { return legData.jumpHeight; } set { legData.jumpHeight = value; } }
    public bool DoubleJump { get { return legData.doubleJump; } set { legData.doubleJump = value; } }


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
