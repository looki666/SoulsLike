using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Basic : MonoBehaviour, IFightStyleSolver
{

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ReceiveInput(CombatInput input)
    {
        Debug.Log(input.combatType);
    }

    public void HandleInput()
    {

    }
}
