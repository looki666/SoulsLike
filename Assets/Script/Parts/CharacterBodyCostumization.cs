using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBodyCostumization : MonoBehaviour {

    private ArmPart armPart;
    private TorsoPart torsoPart;
    private LegPart legPart;
    private HandleUIBars uiBars;

    private ECombatInputType noInput = ECombatInputType.NONE;
    private Vector3 speed;
    private bool isJumping;
    private bool isSprinting;

    private int currHp;
    private int currStamina;

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
        currHp = torsoPart.maxHp;
        currStamina = torsoPart.maxStamina;
        uiBars = GetComponent<HandleUIBars>();
        uiBars.UpdateBarMaxValue(0, torsoPart.maxHp);
        uiBars.UpdateBarMaxValue(1, torsoPart.maxStamina);
        uiBars.UpdateBarValue(0, currHp);
        uiBars.UpdateBarValue(1, currStamina);
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
