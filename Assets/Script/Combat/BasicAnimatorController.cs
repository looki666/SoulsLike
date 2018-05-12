using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAnimatorController : StateMachineBehaviour {

    private ArmPart armPart;

	 // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (armPart == null)
        {
            armPart = animator.GetComponentInParent<ArmPart>();
        }

        if(stateInfo.IsName("Punch") || stateInfo.IsName("StrongPunch") || stateInfo.IsName("2Handed"))
        {
            if (stateInfo.IsName("Punch")) {
                armPart.StartedNewAttack(ECombatInputType.WEAK_ATTACK);
            } else if (stateInfo.IsName("StrongPunch")) {
                armPart.StartedNewAttack(ECombatInputType.STRONG_ATTACK);
            } else if (stateInfo.IsName("2Handed")) {
                armPart.StartedNewAttack(ECombatInputType.BOTH_ATTACKS);
            }
        }
    }

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	//override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if(armPart == null)
        {
            armPart = animator.GetComponentInParent<ArmPart>();
        }

        if (stateInfo.IsName("Punch") || stateInfo.IsName("StrongPunch") || stateInfo.IsName("2Handed"))
        {
            armPart.StartedNewAttack(ECombatInputType.NONE);
            //fightSolver.HandleInput();
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
