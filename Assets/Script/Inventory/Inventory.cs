using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {

    Dictionary<int, GameObject> items;
    Dictionary<int, GameObject> instanciatedItems;

    public InventoryUI inventoryUI;
    CharacterBodyCostumization body;

	// Use this for initialization
	void Awake () {
        items = new Dictionary<int, GameObject>();
        instanciatedItems = new Dictionary<int, GameObject>();
        body = GetComponent<CharacterBodyCostumization>();
        inventoryUI.Inventory = this;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void AddItem(GameObject item, Sprite image, string text, PartType partType)
    {
        items.Add(item.GetHashCode(), item);
        inventoryUI.AddItemEntry(item.GetHashCode(), image, text, partType);
    }

    public void AddItemAndEquip(GameObject item, Sprite image, string text, PartType partType)
    {
        items.Add(item.GetHashCode(), item);
        inventoryUI.AddItemEntry(item.GetHashCode(), image, text, true, partType);
        Equip(item.GetHashCode());
    }

    public void SetInitialObjects(GameObject[] objects)
    {
        for (int i = 0; i < objects.Length; i++)
        {
            instanciatedItems.Add(objects[i].GetHashCode(), objects[i]);
        }
    }

    public bool SwitchUI()
    {
        return inventoryUI.SwitchUI();
    }

    public void Equip(int id)
    {

        GameObject value;
        if (instanciatedItems.ContainsKey(id))
        {
            instanciatedItems.TryGetValue(id, out value);
            value.SetActive(true);
        } else
        {
            items.TryGetValue(id, out value);
            value = Instantiate(value);
            Debug.Log(value.name);
            instanciatedItems.Add(id, value);
        }

        value.GetComponent<IPart>().Equip(body);
    }

}
