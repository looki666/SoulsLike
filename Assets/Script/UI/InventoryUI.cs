using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour {

    public GameObject itemEntryPrefab;
    public GameObject panel;
    public GameObject scrollBar;
    public GameObject equipPanel;
    Image mainPanel;
    Transform panelContent;
    Inventory inv;
    public Inventory Inventory { get { return inv; } set { inv = value; } }

	// Use this for initialization
	void Awake () {
        mainPanel = GetComponent<Image>();
        panelContent = panel.transform.GetChild(0);
    }
	
	// Update is called once per frame
	void Update () {

    }

    public void AddItemEntry(int id, Sprite image, string text)
    {
        GameObject newItemEntry = Instantiate(itemEntryPrefab);

        newItemEntry.transform.GetChild(0).GetComponent<Image>().sprite = image;
        newItemEntry.GetComponentInChildren<Text>().text = text;

        newItemEntry.transform.SetParent(panelContent);
        newItemEntry.transform.localScale = itemEntryPrefab.transform.localScale;

        //TODO: Add a toggle group for the type of item (only 1 leg can be equiped etc)

        newItemEntry.GetComponent<Toggle>().onValueChanged.AddListener(delegate
        {
            inv.Equip(id); 
        });
    }

    public void FilterByType()
    {

    }

    public void SwitchUI()
    {
        bool state = !mainPanel.enabled;
        mainPanel.enabled = state;
        panel.SetActive(state);
        equipPanel.SetActive(state);
        scrollBar.SetActive(state);
    }

}
