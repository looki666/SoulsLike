using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Part/Arm/BasicArm")]
public class BasicArmScriptable : ScriptableObject {

    public string name;
    public Sprite image;

    public int damage;
    public int normalStaminaCost;
    public int heavyDamage;
    public int heavyStaminaCost;

    public void AttackAbility()
    {
        Debug.Log("Ability Arm Combat");
    }

}
