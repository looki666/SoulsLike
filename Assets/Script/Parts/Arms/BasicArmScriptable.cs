using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Part/Arm/BasicArm")]
public class BasicArmScriptable : ScriptableObject {

    public int damage;
    public int heavyDamage;

    public void AttackAbility()
    {
        Debug.Log("Ability Arm Combat");
    }

}
