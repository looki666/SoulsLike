using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PartType { Arms, Legs, Torso }

public interface IPart  {

    void Equip(CharacterBodyCostumization body);
    Sprite GetSprite();
    string GetName();
    PartType GetType();
}
