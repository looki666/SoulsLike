using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Part/Torso/BasicTorso")]
public class BasicTorsoScriptable : ScriptableObject
{
    public int maxHp;
    public int maxStamina;
    public int staminaRegen;
}
