using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorsoPart : MonoBehaviour {

    public BasicTorsoScriptable torsoData;

    public int maxHp;
    public int maxStamina;
    public float staminaRegen;

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
