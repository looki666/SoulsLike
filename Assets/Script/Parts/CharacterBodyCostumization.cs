using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBodyCostumization : MonoBehaviour {

    private ArmPart armPart;
    private TorsoPart torsoPart;
    private LegPart legPart;

    private ECombatInputType noInput = ECombatInputType.NONE;
    private Vector3 speed;
    private bool isJumping;
    private bool isSprinting;

    public ArmPart ArmsPart
    {
        get { return armPart; }
        set { armPart = value; }
    }
    public TorsoPart TorsoPart
    {
        get { return torsoPart; }
        set { torsoPart = value; }
    }
    public LegPart LegPart {
        get { return legPart; }
        set { legPart = value; }
    }
    

    // Use this for initialization
    void Awake () {
        armPart = GetComponentInChildren<ArmPart>();
        torsoPart = GetComponentInChildren<TorsoPart>();
        legPart = GetComponentInChildren<LegPart>();
    }
	
    // Update is called once per frame
    void Update () {
        ECombatInputType someInputWasPressed = noInput;

        if (Input.GetMouseButtonDown(0))
        {
            someInputWasPressed = ECombatInputType.WEAK_ATTACK;
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (someInputWasPressed == ECombatInputType.WEAK_ATTACK) {
                someInputWasPressed = ECombatInputType.BOTH_ATTACKS;
            } else
            {
                someInputWasPressed = ECombatInputType.STRONG_ATTACK;
            }

        }

        if(someInputWasPressed != ECombatInputType.NONE)
        {
            CombatInput input = new CombatInput(speed, isJumping, isSprinting, someInputWasPressed);
            armPart.fightHandler.ReceiveInput(input);
        }

    }

    public void SetMovementState(Vector3 speed, bool isJumping, bool isSprinting)
    {
        this.speed = speed;
        this.isJumping = isJumping;
        this.isSprinting = isSprinting;
    }

}
