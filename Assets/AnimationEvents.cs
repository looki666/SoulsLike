using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvents : MonoBehaviour
{
    private ArmPart arms;

	// Use this for initialization
	void Start ()
	{
	    arms = GetComponentInParent<ArmPart>();

	}

    //Is used in animation events
    void SpendStamina()
    {
        arms.SpendStamina();
    }

    //Is used in animation events
    void EnableStateArm(int arm)
    {
        arms.ArmColliders(true, arm);
    }

    //Is used in animation events
    void DisableStateArm(int arm)
    {
        arms.ArmColliders(false, arm);
    }


}
