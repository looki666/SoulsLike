﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {

    Dictionary<int, GameObject> items;
    Dictionary<int, GameObject> instanciatedItems;

    public InventoryUI inventoryUI;
    CharacterBodyCostumization body;

	// Use this for initialization
	void Start () {
        items = new Dictionary<int, GameObject>();
        instanciatedItems = new Dictionary<int, GameObject>();
        body = GetComponent<CharacterBodyCostumization>();
        inventoryUI.Inventory = this;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void AddItem(GameObject item, Sprite image, string text)
    {
        items.Add(item.GetHashCode(), item);
        inventoryUI.AddItemEntry(item.GetHashCode(), image, text);
    }

    public void Equip(int id)
    {
        Debug.Log(id);
        GameObject value;
        if (instanciatedItems.ContainsKey(id))
        {
            Debug.Log("Is instantiated");
            instanciatedItems.TryGetValue(id, out value);
            value.SetActive(true);
        } else
        {
            Debug.Log("Is not instantiated");
            items.TryGetValue(id, out value);
            value = Instantiate(value);
            Debug.Log(value.name);
            instanciatedItems.Add(id, value);
        }

        value.GetComponent<IPart>().Equip(body);
    }

}
