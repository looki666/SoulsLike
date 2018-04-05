using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GizmoDrawText : MonoBehaviour
{
    public float yOffset = 16;
    public Color textColor = Color.cyan;
    public int fontSize = 12;

    private void OnDrawGizmos()
    {
        GizmosUtils.DrawText(GUI.skin, transform.name, transform.position, textColor, fontSize, yOffset);
    }
}