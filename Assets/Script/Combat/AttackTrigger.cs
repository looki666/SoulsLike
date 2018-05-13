using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTrigger : MonoBehaviour {

    ArmPart arms;
    public Transform particleHit;

    private void Awake()
    {
        arms = GetComponentInParent<ArmPart>();
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
        PlayPunchSounds();
        Instantiate(particleHit, transform.position, Quaternion.identity);
        arms.OnAttackHit(other);
    }

    void PlayPunchSounds()
    {
        arms.GetComponent<AudioSource>().Play();
    }
}
