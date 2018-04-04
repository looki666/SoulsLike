using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Basic : MonoBehaviour, IFightStyleSolver
{
    private Animator animator;

    [SerializeField]
    [ReadOnly]
    private bool isAttacking;
    public bool IsAttacking
    {
        set
        {
            isAttacking = value;
        }
    }

    private CombatInput input;
    private ECombatInputType currentAttack;
    public ECombatInputType CurrentAttack
    {
        get
        {
           return currentAttack;
        }
        set
        {
            currentAttack = value;
        }
    }

    // Use this for initialization
    void Start()
    {
        input = null;
        isAttacking = false;
    }

	// Update is called once per frame
	void Update () {
		
	}

    public void SetAnimator(Animator animator)
    {
        this.animator = animator;
    }

    public void ReceiveInput(CombatInput input)
    {
        if (this.input == null)
        {
            this.input = input;
        }
        HandleInput();
    }

    public void HandleInput()
    {
        if (input != null)
        {
            if (ECombatInputType.WEAK_ATTACK.Equals(input.combatType))
            {
                animator.SetTrigger("attack");
            } else if (ECombatInputType.STRONG_ATTACK.Equals(input.combatType))
            {
                animator.SetTrigger("strongAttack");
            }else if (ECombatInputType.BOTH_ATTACKS.Equals(input.combatType))
            {
                animator.SetTrigger("bothAttack");
            }

            input = null;
        }
    }

    public void HandleSuccessfulHits()
    {

    }

}
