using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour {

    public bool startLocked = true;
    public Vector3 windDirection;
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

        if (Input.GetKeyDown(KeyCode.I))
        {
            Cursor.lockState = SwitchCursorConfinedState();
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

    /*
 * Switch cursor mode between none or screen center.
 */
    private CursorLockMode SwitchCursorConfinedState()
    {
        if (CursorLockMode.None.Equals(Cursor.lockState))
        {
            return CursorLockMode.Confined;
        }
        return CursorLockMode.None;
    }


}
