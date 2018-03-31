using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmPart : MonoBehaviour {

    public BasicArmScriptable armData;
    public IFightStyleSolver fightHandler;
    public Animator animator;

    public int Damage { get { return armData.damage; } set { armData.damage = value; } }
    public int HeavyDamage { get { return armData.heavyDamage; } set { armData.heavyDamage = value; } }

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

    public void OnAttackHit(Collider enemy)
    {
        int damageDone;
        if (fightHandler.CurrentAttack == ECombatInputType.WEAK_ATTACK)
        {
            damageDone = Damage;
        } else 
        {
            damageDone = HeavyDamage;
        }
        Debug.Log(damageDone);
        //TODO: replace with Enemy having a function that handles taking damage
        enemy.GetComponent<Enemy>().hp -= damageDone;
    }

}
