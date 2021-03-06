﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBodyCostumization : MonoBehaviour {

    private ArmPart armPart;
    private TorsoPart torsoPart;
    private LegPart legPart;
    private Inventory inventory;
    private HandleUIBars uiBars;
    private CameraController myCamera;
    public NewItemUI uiItem;

    private ECombatInputType noInput = ECombatInputType.NONE;
    private Vector3 speed;
    private bool isJumping;
    private bool isSprinting;

    private int currHp;
    private int currStamina;

    private float timer;
    private float timerRestDelay;
    private float damageAreaDelay;
    public float staminaRestDelay;
    public float timerStaminaRate;

    [SerializeField]
    [ReadOnly]
    private bool startRegeningStamina;

    public bool CanAttack { get; set; }

    public ArmPart ArmsPart
    {
        get { return armPart; }
        set {
            Transform armParent = armPart.transform.parent;
            armPart.gameObject.SetActive(false);
            armPart = value;
            armPart.transform.SetParent(armParent);
            armPart.gameObject.SetActive(true);
        }
    }
    public TorsoPart TorsoPart
    {
        get { return torsoPart; }
        set {
            torsoPart.gameObject.SetActive(false);
            torsoPart = value;
            torsoPart.transform.SetParent(transform);
            torsoPart.gameObject.SetActive(true);
        }
    }
    public LegPart LegPart {
        get { return legPart; }
        set {
            legPart.gameObject.SetActive(false);
            legPart = value;
            legPart.transform.SetParent(transform);
            legPart.gameObject.SetActive(true);
        }
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
        myCamera = GetComponentInChildren<CameraController>();
        inventory = GetComponent<Inventory>();
        armPart = GetComponentInChildren<ArmPart>();
        torsoPart = GetComponentInChildren<TorsoPart>();
        legPart = GetComponentInChildren<LegPart>();
        AddInitialItems(new GameObject[3]{ armPart.gameObject, torsoPart.gameObject, legPart.gameObject });
        currHp = torsoPart.maxHp;
        currStamina = torsoPart.maxStamina;
        uiBars = GetComponent<HandleUIBars>();
        uiBars.UpdateBarMaxValue(0, torsoPart.maxHp);
        uiBars.UpdateBarMaxValue(1, torsoPart.maxStamina);
        uiBars.UpdateBarValue(0, currHp);
        uiBars.UpdateBarValue(1, currStamina);
        startRegeningStamina = false;
        timerRestDelay = 0;
        armPart.ArmsAddedToBody(this);
        CanAttack = true;
    }
	
    // Update is called once per frame
    void Update () {
        //start delay before starting regen
        if (startRegeningStamina)
        {
            timerRestDelay += Time.deltaTime;
        }

        //can start regen
        if (timerRestDelay >= staminaRestDelay)
        {
            startRegeningStamina = false;
            if (currStamina < torsoPart.maxStamina)
            {
                timer += Time.deltaTime;
                if (timer >= torsoPart.staminaRegen)
                {
                    timer = 0;
                    CurrStamina++;
                }
            }
        }

        //Handle Block animation state.
        if (Input.GetKeyDown(KeyCode.Q))
        {
            armPart.Block();
        }
        else if (Input.GetKeyUp(KeyCode.Q))
        {
            armPart.UnBlock();
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            CanAttack = !CanAttack;
            bool ui = inventory.SwitchUI();
            myCamera.UseMouseLook = !ui;
        }

        ECombatInputType someInputWasPressed = noInput;

        if (CanAttack)
        {
            if (Input.GetMouseButtonDown(0))
            {
                someInputWasPressed = ECombatInputType.WEAK_ATTACK;
            }

            if (Input.GetMouseButtonDown(1))
            {
                if (someInputWasPressed == ECombatInputType.WEAK_ATTACK)
                {
                    someInputWasPressed = ECombatInputType.BOTH_ATTACKS;
                }
                else
                {
                    someInputWasPressed = ECombatInputType.STRONG_ATTACK;
                }
            }

            if (someInputWasPressed != ECombatInputType.NONE)
            {
                CombatInput input = new CombatInput(speed, isJumping, isSprinting, someInputWasPressed);
                armPart.AttackInput(input);
            }
        }

        if (isSprinting)
        {
            timerStaminaRate += Time.deltaTime;
            if (timerStaminaRate >= legPart.RunningStaminaRate)
            {
                CurrStamina -= legPart.RunningStaminaCost;
                timerStaminaRate = 0;
            }
        }

    }

    public void Jump()
    {
        CurrStamina -= legPart.JumpStaminaCost;
        StartStaminaRegen();
    }

    public void StartStaminaRegen()
    {
        startRegeningStamina = true;
        timerRestDelay = 0;
    }

    private void AddInitialItems(GameObject[] newItem)
    {
        inventory.SetInitialObjects(newItem);

        IPart part;
        for (int i = 0; i < newItem.Length; i++)
        {
            part = newItem[i].GetComponent<IPart>();
            inventory.AddItemAndEquip(newItem[i], part.GetSprite(), part.GetName(), part.GetType());
        }
    }

    public void AddItemInventory(GameObject[] newItem)
    {
        IPart part;
        for (int i = 0; i < newItem.Length; i++)
        {
            part = newItem[i].GetComponent<IPart>();
            inventory.AddItem(newItem[i], part.GetSprite(), part.GetName(), part.GetType());
        }

        part = newItem[0].GetComponent<IPart>();
        uiItem.EnableNewItemUI(part.GetSprite(), part.GetName());
    }

    private void StopStaminaRegen()
    {
        startRegeningStamina = false;
        timerRestDelay = 0;
    }

    public void SetMovementState(Vector3 speed, bool isJumping, bool isSprinting)
    {
        this.speed = speed;
        this.isJumping = isJumping;
        //Started sprinting
        if (isSprinting && !this.isSprinting)
        {
            StopStaminaRegen();
        }
        //Stopped Sprinting
        if (!isSprinting && this.isSprinting)
        {
            StartStaminaRegen();
        }
        this.isSprinting = isSprinting;
    }


    void OnTriggerEnter(Collider other)
    {
        DamageArea damagingArea = other.GetComponent<DamageArea>();
        if(damagingArea != null)
        {
            if (other.GetComponentInParent<Enemy>().IsAttackBlocked)
            {
                return;
            }
            damageAreaDelay = 0;
            CurrHp -= damagingArea.damageValue;
        }

    }

    void OnTriggerStay(Collider other)
    {
        DamageArea damagingArea = other.GetComponent<DamageArea>();
        if (damagingArea != null)
        {
            if (damagingArea.keepDamaging)
            {
                damageAreaDelay += Time.deltaTime;
                if (damageAreaDelay > 1f / 5f)
                {
                    damageAreaDelay = 0;
                    CurrHp -= damagingArea.damageValue;
                }
            }
        }
    }
}
