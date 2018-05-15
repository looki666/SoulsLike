using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegPart : MonoBehaviour, IPart {

    public BasicLegScriptable legData;

    public float MovementSpeed { get { return legData.movementSpeed; } set { legData.movementSpeed = value; } }
    public float RunningSpeed { get { return legData.runningSpeed; } set { legData.runningSpeed = value; } }
    public float RunningStaminaRate { get { return legData.runningStaminaRate; } set { legData.runningStaminaRate = value; } }
    public int RunningStaminaCost { get { return legData.runningStaminaCost; } set { legData.runningStaminaCost = value; } }
    public float FlashStepSpeed { get { return legData.flashStepSpeed; } set { legData.flashStepSpeed = value; } }
    public int FlashStaminaCost { get { return legData.flashStaminaCost; } set { legData.flashStaminaCost = value; } }
    public float JumpHeight { get { return legData.jumpHeight; } set { legData.jumpHeight = value; } }
    public int JumpNumber { get { return legData.jumpNumber; } set { legData.jumpNumber = value; } }
    public int JumpStaminaCost { get { return legData.jumpStaminaCost; } set { legData.jumpStaminaCost = value; } }
    public bool WallRunHorizontal { get { return legData.wallRunHorizontal; } set { legData.wallRunHorizontal = value; } }
    public bool WallRunVertical { get { return legData.wallRunVertical; } set { legData.wallRunVertical = value; } }

    public void Equip(CharacterBodyCostumization body)
    {
        body.LegPart = this;
    }

    public string GetName()
    {
        return legData.name;
    }

    public Sprite GetSprite()
    {
        return legData.image;
    }

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

    PartType IPart.GetType()
    {
        return PartType.Legs;
    }
}
