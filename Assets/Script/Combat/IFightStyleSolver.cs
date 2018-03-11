using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFightStyleSolver {

    void ReceiveInput(CombatInput input);
    void HandleInput();
    void SetAnimator(Animator animator);
    bool IsAttacking { set; }
}
