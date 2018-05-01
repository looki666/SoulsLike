using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface IPart  {

    void Equip(CharacterBodyCostumization body);
    Sprite GetSprite();
    string GetName();
}
