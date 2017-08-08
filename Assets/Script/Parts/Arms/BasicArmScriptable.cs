using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Part/Arm/BasicArm")]
public class BasicArmScriptable : ScriptableObject {

    public float damage;
    public float heavyDamage;

    public void AttackAbility()
    {
        Debug.Log("Ability Arm Combat");
    }

}
