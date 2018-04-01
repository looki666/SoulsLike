﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTrigger : MonoBehaviour {

    ArmPart arms;

    private void Awake()
    {
        arms = GetComponentInParent<ArmPart>();
    }

    void OnTriggerEnter(Collider other)
    {
        arms.OnAttackHit(other);
    }


}
