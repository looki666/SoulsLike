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

    private float timer;
    private float timerRestDelay;
    public float staminaRestDelay;
    private bool checkDelay;

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

    public int CurrHp
    {
        get { return currHp; }
        set { currHp = Mathf.Clamp(value, 0, torsoPart.maxHp); uiBars.UpdateBarValue(0, currHp); }
    }

    public int CurrStamina
    {
        get { return currStamina; }
        set { currStamina = Mathf.Clamp(value, 0, torsoPart.maxStamina); uiBars.UpdateBarValue(1, currStamina); }
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
        checkDelay = false;
    }
	
    // Update is called once per frame
    void Update () {
        if (checkDelay)
        {
            timerRestDelay += Time.deltaTime;
        }

        if (timerRestDelay >= staminaRestDelay)
        {
            checkDelay = false;
            if (currStamina < torsoPart.maxStamina)
            {
                timer += Time.deltaTime;
                if (timer >= (1f / torsoPart.staminaRegen))
                {
                    timer = 0;
                    CurrStamina++;
                }
            }
        }

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
            armPart.AttackInput(input);
        }

    }

    public void ResetRestDelay()
    {
        checkDelay = true;
        timerRestDelay = 0;
    }

    public void SetMovementState(Vector3 speed, bool isJumping, bool isSprinting)
    {
        this.speed = speed;
        this.isJumping = isJumping;
        this.isSprinting = isSprinting;
    }

}
