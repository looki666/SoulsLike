using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Part/Leg/BasicLeg")]
public class BasicLegScriptable : ScriptableObject {

    public float movementSpeed;
    public float runningSpeed;
    public float jumpHeight;
    public bool doubleJump;

    public void MovementAbilityAbility()
    {
        Debug.Log("Ability Movement Leg");
    }

}
