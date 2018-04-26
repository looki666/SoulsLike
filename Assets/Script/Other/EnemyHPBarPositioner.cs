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
        Vector2 halfwayPivot = new Vector2(0.5f, 0.5f);
        foreach (Transform child in allEnemies)
        {
            Enemy enemy = child.GetComponent<Enemy>();
            enemies[childs] = enemy.HpBarAnchor;

            GameObject bar = (GameObject)Instantiate(hpPrefab, transform);
            bar.name = enemy.name;
            hpBars[childs] = bar.transform;
            bar.GetComponent<RectTransform>().pivot = halfwayPivot;
            enemy.HpBar = bar.GetComponent<Slider>();
            childs++;
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
