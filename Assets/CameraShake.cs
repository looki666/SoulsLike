using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour {

    Camera camera;
    public float shake = 0f;
    float shakeAmount = 0.3f;
    float decreaseFactor = 1.0f;

    private Vector3 originalPos;

    // Use this for initialization
    void Start () {
        camera = GetComponent<Camera>();
        originalPos = camera.transform.localPosition;
    }

 
    void Update() {
            if (shake > 0)
            {
                camera.transform.localPosition += Random.insideUnitSphere * shakeAmount;
                shake -= Time.deltaTime * decreaseFactor;

            }
            else
            {
                shake = 0.0f;
                camera.transform.localPosition = originalPos;
            }
    }

    public void Shake(float time)
    {
        shake = time;
    }

    public void Shake(float time, float strength)
    {
        shake = time;
        shakeAmount = strength;
    }

}
