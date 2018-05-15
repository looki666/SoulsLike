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
    private ToggleGroup[] toggleGroup;

	// Use this for initialization
	void Awake () {
        mainPanel = GetComponent<Image>();
        panelContent = panel.transform.GetChild(0);

        int partTypeNumber = 3;
        toggleGroup = new ToggleGroup[partTypeNumber];

        //Instantiating empty objects just to hold the toggleGroup component... feels dirty
        for (int i = 0; i < partTypeNumber; i++)
        {
            GameObject group = new GameObject();
            toggleGroup[i] = group.AddComponent<ToggleGroup>();
        }

    }
	
	// Update is called once per frame
	void Update () {

    }

    public void AddItemEntry(int id, Sprite image, string text, PartType partType)
    {
        AddItemEntry(id, image, text, false, partType);
    }

    public void AddItemEntry(int id, Sprite image, string text, bool equiped, PartType partType)
    {
        GameObject newItemEntry = Instantiate(itemEntryPrefab);

        newItemEntry.transform.GetChild(0).GetComponent<Image>().sprite = image;
        newItemEntry.GetComponentInChildren<Text>().text = text;

        newItemEntry.transform.SetParent(panelContent);
        newItemEntry.transform.localScale = itemEntryPrefab.transform.localScale;

        //TODO: Add a toggle group for the type of item (only 1 leg can be equiped etc)
        Toggle toggle = newItemEntry.GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(delegate
        {
            inv.Equip(id);
        });

        toggle.group = toggleGroup[(int)partType];

        if (equiped)
        {
            newItemEntry.GetComponent<Toggle>().isOn = true;
        }
    }

    public void FilterByType()
    {

    }

    public bool SwitchUI()
    {
        bool state = !mainPanel.enabled;
        mainPanel.enabled = state;
        panel.SetActive(state);
        equipPanel.SetActive(state);
        scrollBar.SetActive(state);

        return state;
    }

}
