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

        if (!isAttacking)
        {
            HandleInput();
        } 
    }

    public void HandleInput()
    {
        if (input != null)
        {
            animator.SetTrigger("attack");
            input = null;
        }
    }
}
