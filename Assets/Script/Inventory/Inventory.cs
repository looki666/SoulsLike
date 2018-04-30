using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {

    [SerializeField]
    [ReadOnly]
    List<GameObject> items;

    public InventoryUI inventoryUI;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void AddItem(GameObject item, Sprite image, string text)
    {
        items.Add(item);
        inventoryUI.AddItemEntry(image, text);
    }

}
