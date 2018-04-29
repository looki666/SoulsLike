using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Part/Torso/BasicTorso")]
public class BasicTorsoScriptable : ScriptableObject
{
    public string name;
    public Sprite image;

    public int maxHp;
    public int maxStamina;
    public float staminaRegen;
}
