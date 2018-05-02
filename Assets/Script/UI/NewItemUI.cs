using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewItemUI : MonoBehaviour {

    private Image bar;
    public Image itemIcon;
    public Text itemName;
    
    public float timerToDissapear;
    float timer;

	// Use this for initialization
	void Awake () {
        timer = 0;
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

    void Update ()
    {
        if (bar.enabled)
        {
            timer += Time.deltaTime;

            if (timer >= timerToDissapear)
            {
                timer = 0;
                DisableNewItemUI();
            }
        }



    }

}
