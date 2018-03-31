﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAnimatorController : StateMachineBehaviour {

    private IFightStyleSolver fightSolver;

	 // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (fightSolver == null)
        {
            fightSolver = animator.GetComponent<IFightStyleSolver>();
        }

        if(stateInfo.IsName("Punch") || stateInfo.IsName("StrongPunch") || stateInfo.IsName("2Handed"))
        {
            fightSolver.IsAttacking = true;
            if (stateInfo.IsName("Punch")) {
                fightSolver.CurrentAttack = ECombatInputType.WEAK_ATTACK;
            } else if (stateInfo.IsName("StrongPunch")) {
                fightSolver.CurrentAttack = ECombatInputType.STRONG_ATTACK;
            } else if (stateInfo.IsName("2Handed")) {
                fightSolver.CurrentAttack = ECombatInputType.BOTH_ATTACKS;
            }
        }
    }

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	//override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if(fightSolver == null)
        {
            fightSolver = animator.GetComponent<IFightStyleSolver>();
        }

        if (stateInfo.IsName("Punch") || stateInfo.IsName("StrongPunch") || stateInfo.IsName("2Handed"))
        {
            fightSolver.IsAttacking = false;
            fightSolver.CurrentAttack = ECombatInputType.NONE;
            fightSolver.HandleInput();
        }
    }

	// OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
	//override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
	//override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}
}
