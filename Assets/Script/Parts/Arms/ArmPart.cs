using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmPart : MonoBehaviour {

    public BasicArmScriptable armData;

    public float Damage { get { return armData.damage; } set { armData.damage = value; } }
    public float HeavyDamage { get { return armData.heavyDamage; } set { armData.heavyDamage = value; } }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void BasicAttack()
    {
        //animator
    }

    public void HeavyAttack()
    {
        
    }

    public void AttackAbility()
    {
        armData.AttackAbility();
    }

}
