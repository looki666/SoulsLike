using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour {

    public GameObject itemEntryPrefab;
    public GameObject panel;
    public GameObject scrollBar;
    Image mainPanel;
    Transform panelContent;

	// Use this for initialization
	void Start () {
        mainPanel = GetComponent<Image>();
        panelContent = panel.transform.GetChild(0);
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.I))
        {
            SwitchUI();
        }
    }

    public void AddItemEntry(Sprite image, string text)
    {
        GameObject newItemEntry = Instantiate(itemEntryPrefab);

        newItemEntry.transform.GetChild(0).GetComponent<Image>().sprite = image;
        newItemEntry.GetComponentInChildren<Text>().text = text;

        newItemEntry.transform.SetParent(panelContent);
        newItemEntry.transform.localScale = itemEntryPrefab.transform.localScale;
    }

    public void SwitchUI()
    {
        bool state = !mainPanel.enabled;
        mainPanel.enabled = state;
        panel.SetActive(state);
        scrollBar.SetActive(state);
    }

}
