using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorsoPart : MonoBehaviour, IPart {

    public BasicTorsoScriptable torsoData;

    public int maxHp;
    public int maxStamina;
    public float staminaRegen;

    public void Equip(CharacterBodyCostumization body)
    {
        body.TorsoPart = this;
    }

    public string GetName()
    {
        return torsoData.name;
    }

    public Sprite GetSprite()
    {
        return torsoData.image;
    }

    // Use this for initialization
    void Awake () {
        maxHp = torsoData.maxHp;
        maxStamina = torsoData.maxStamina;
        staminaRegen = torsoData.staminaRegen;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
