using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicCombo : MonoBehaviour, IFightStyleSolver
{

    private Animator animator;

    private bool isAttacking;
    public bool IsAttacking
    {
        set
        {
            isAttacking = value;
        }
    }

    private ECombatInputType currentAttack;
    public ECombatInputType CurrentAttack
    {
        set
        {
            currentAttack = value;
        }
        get
        {
            return currentAttack;
        }
    }

    // Use this for initialization
    void Start () {
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
        Debug.Log(input.combatType);
        if (!isAttacking)
        {
            HandleInput();
        }
    }

    public void HandleInput()
    {
        animator.SetTrigger("attack");
    }
}
