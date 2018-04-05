using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Part/Leg/BasicLeg")]
public class BasicLegScriptable : ScriptableObject {

    public float movementSpeed;
    public float flashStepSpeed;
    public int flashStaminaCost;
    public float runningSpeed;
    public float runningStaminaRate;
    public int runningStaminaCost;
    public float jumpHeight;
    public int jumpNumber;
    public int jumpStaminaCost;
    public bool wallRunHorizontal;
    public bool wallRunVertical;

    public void MovementAbility()
    {
        Debug.Log("Ability Movement Leg");
    }

}
