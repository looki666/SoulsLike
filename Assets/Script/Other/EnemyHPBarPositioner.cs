using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHPBarPositioner : MonoBehaviour {

    public Transform allEnemies;
    public GameObject hpPrefab;
    private Transform[] enemies;
    private Transform[] hpBars;
    private RectTransform canvas;
    public Transform player;
    public float distanceToDisplay;

	// Use this for initialization
	void Start () {
        enemies = new Transform[allEnemies.childCount];
        hpBars = new Transform[allEnemies.childCount];
        canvas = GetComponent<RectTransform>();
        //Get all enemies
        int childs = 0;
        foreach (Transform child in allEnemies)
        {
            enemies[childs] = child.GetComponent<Enemy>().HpBarAnchor;
            childs++;
        }

        //Create hp bar for each enemy
        for (int i = 0; i < enemies.Length; i++)
        {
            GameObject bar = (GameObject)Instantiate(hpPrefab, transform);
            hpBars[i] = bar.transform;
        }

        PositionBars();
    }

    private void DecideDisplay()
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            hpBars[i].gameObject.SetActive(Vector3.Distance(player.position, enemies[i].position) <= distanceToDisplay);
        }


    }

    private void PositionBars()
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            Vector3 cameraRelative = Camera.main.transform.InverseTransformPoint(enemies[i].position);

            if (cameraRelative.z > 0)
            {
                Vector3 screenPos = Camera.main.WorldToScreenPoint(enemies[i].position);
                hpBars[i].transform.position = screenPos;
            }
        }
    }

	// Update is called once per frame
	void Update () {
        DecideDisplay();
        PositionBars();
    }
}
