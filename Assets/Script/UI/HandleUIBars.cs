using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandleUIBars : MonoBehaviour {

    public Slider[] sliders;
    RectTransform[] rectTransform;
    private Vector2 size;

    void Awake()
    {
        size = new Vector2(0,0);
        rectTransform = new RectTransform[sliders.Length];
        for (int i = 0; i < sliders.Length; i++)
        {
            rectTransform[i] = sliders[i].GetComponent<RectTransform>();
        }
    }

    public void UpdateBarMaxValue(int index, int max)
    {
        size.x = max*2;
        size.y = rectTransform[index].sizeDelta.y;
        rectTransform[index].sizeDelta = size;
        sliders[index].maxValue = max;
    }

    public void UpdateBarValue(int index, int value)
    {
        sliders[index].value = value;
    }

    public void ChangeBarValueBy(int index, int value)
    {
        sliders[index].value += value;
    }

}
