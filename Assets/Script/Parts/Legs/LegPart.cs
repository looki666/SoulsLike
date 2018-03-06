using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegPart : MonoBehaviour {

    public BasicLegScriptable legData;

    public float MovementSpeed { get { return legData.movementSpeed; } set { legData.movementSpeed = value; } }
    public float RunningSpeed { get { return legData.runningSpeed; } set { legData.runningSpeed = value; } }
    public float JumpHeight { get { return legData.jumpHeight; } set { legData.jumpHeight = value; } }
    public int JumpNumber { get { return legData.jumpNumber; } set { legData.jumpNumber = value; } }
    public bool WallRunHorizontal { get { return legData.wallRunHorizontal; } set { legData.wallRunHorizontal = value; } }
    public bool WallRunVertical { get { return legData.wallRunVertical; } set { legData.wallRunVertical = value; } }

    public void MovementAbility()
    {
        legData.MovementAbility();
    }


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
