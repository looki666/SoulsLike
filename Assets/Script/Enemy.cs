using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    [SerializeField]
    [ReadOnly]
    private int hp = 100;

    [SerializeField]
    [ReadOnly]
    private bool isDead = false;

	// Use this for initialization
	void Awake () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Damage(int dmg)
    {
        hp = Mathf.Max(hp - dmg, 0);
    }
}
