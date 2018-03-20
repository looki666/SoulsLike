using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Part/Leg/BasicLeg")]
public class BasicLegScriptable : ScriptableObject {

    public float movementSpeed;
    public float flashStepSpeed;
    public float runningSpeed;
    public float jumpHeight;
    public int jumpNumber;
    public bool wallRunHorizontal;
    public bool wallRunVertical;

    public void MovementAbility()
    {
        Debug.Log("Ability Movement Leg");
    }

}
