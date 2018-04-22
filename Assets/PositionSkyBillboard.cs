using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionSkyBillboard : MonoBehaviour {

    Transform characterPos;
    Vector3 offset;
    World wind;
    public bool move;

	// Use this for initialization
	void Start () {
        characterPos = GameObject.FindGameObjectWithTag("Player").transform;
        wind = GameObject.FindGameObjectWithTag("World").GetComponent<World>();
        offset = transform.position - characterPos.position;
    }
	
	// Update is called once per frame
	void Update () {
        transform.position = characterPos.position + offset;// new Vector3(characterPos.position.x + offset.x, offset.y, characterPos.position.z + offset.z);
        if (move)
        {
            offset += wind.windDirection * Time.deltaTime;
        }

        transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
    }
}
