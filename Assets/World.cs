using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour {

    public bool startLocked = true;
	// Use this for initialization
	void Start () {
        if (startLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Backslash))
        {
            Cursor.lockState = SwitchCursorLockState();
        }
    }

    /*
     * Switch cursor mode between none or screen center.
     */
    private CursorLockMode SwitchCursorLockState()
    {
        if (CursorLockMode.None.Equals(Cursor.lockState))
        {
            return CursorLockMode.Locked;
        }
        return CursorLockMode.None;
    }


}
