using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmPart : MonoBehaviour {

    public BasicArmScriptable armData;
    public IFightStyleSolver fightHandler;
    public Animator animator;

    public float Damage { get { return armData.damage; } set { armData.damage = value; } }
    public float HeavyDamage { get { return armData.heavyDamage; } set { armData.heavyDamage = value; } }

    // Use this for initialization
    void Start () {
        fightHandler = GetComponent<IFightStyleSolver>();
        animator = GetComponent<Animator>();
        fightHandler.SetAnimator(animator);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void AttackAbility()
    {
        armData.AttackAbility();
    }

}
