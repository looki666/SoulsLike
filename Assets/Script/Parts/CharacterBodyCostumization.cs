using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBodyCostumization : MonoBehaviour {

    ArmPart armPart;
    TorsoPart torsoPart;
    LegPart legPart;


    public ArmPart ArmsPart
    {
        get { return armPart; }
        set { armPart = value; }
    }
    public TorsoPart TorsoPart
    {
        get { return torsoPart; }
        set { torsoPart = value; }
    }
    public LegPart LegPart {
        get { return legPart; }
        set { legPart = value; }
    }
    

    // Use this for initialization
    void Start () {
        armPart = GetComponentInChildren<ArmPart>();
        torsoPart = GetComponentInChildren<TorsoPart>();
        legPart = GetComponentInChildren<LegPart>();
    }
	
    // Update is called once per frame
    void Update () {

        if (Input.GetMouseButtonDown(0))
        {
            armPart.BasicAttack();
        }

        if (Input.GetMouseButtonDown(1))
        {
            armPart.HeavyAttack();
        }

    }


}
