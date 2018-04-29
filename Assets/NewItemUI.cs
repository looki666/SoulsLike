using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewItemUI : MonoBehaviour {

    private Image bar;
    public Image itemIcon;
    public Text itemName;

	// Use this for initialization
	void Start () {
        bar = GetComponent<Image>();
        DisableNewItemUI();
    }

    public void EnableNewItemUI(Sprite img, string name)
    {
        bar.enabled = true;
        itemIcon.enabled = true;
        itemName.enabled = true;
        itemIcon.sprite = img;
        itemName.text = name;

    }

    public void DisableNewItemUI()
    {
        bar.enabled = false;
        itemIcon.enabled = false;
        itemName.enabled = false;
    }

}
