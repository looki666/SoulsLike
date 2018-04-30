using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockTrigger : MonoBehaviour
{

    ArmPart arms;

    private void Awake()
    {
        arms = GetComponentInParent<ArmPart>();
    }

    void OnTriggerEnter(Collider other)
    {
        arms.BlockHit(other);
    }


}
