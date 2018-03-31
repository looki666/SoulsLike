using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeFrameFXExperiment : MonoBehaviour {

    RenderTexture[] multipleFrames;
    int currentFrameIndex = 0;
    Camera freezingCam;

	// Use this for initialization
	void Start () {
        multipleFrames = new RenderTexture[6];

        for (int i = 0; i < multipleFrames.Length; i++)
        {
            multipleFrames[i] = new RenderTexture(256, 256, 16, RenderTextureFormat.ARGB32);
        }

        freezingCam = GetComponent<Camera>();
        freezingCam.enabled = false;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void FreezeFrame()
    {
        currentFrameIndex++;
        multipleFrames[currentFrameIndex].Create();
        freezingCam.targetTexture = multipleFrames[currentFrameIndex];
        freezingCam.Render();
    }

    public void ReleaseAll()
    {
        for (int i = 0; i < multipleFrames.Length; i++)
        {
            multipleFrames[i].Release();
        }
    }

}
