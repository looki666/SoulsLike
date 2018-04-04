using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatInput {

    Vector3 MovementDir { get; set; }
    bool isJumping;
    bool isSprinting;
    public ECombatInputType combatType;

    public CombatInput(Vector3 MovementDir, bool isJumping, bool isSprinting, ECombatInputType combatType)
    {
        this.MovementDir = MovementDir;
        this.isJumping = isJumping;
        this.isSprinting = isSprinting;
        this.combatType = combatType;
    }

}

public enum ECombatInputType
{
    NONE,
    WEAK_ATTACK,
    STRONG_ATTACK,
    BOTH_ATTACKS
}