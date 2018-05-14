using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemUIHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public AnimationCurve curve;

    private Vector3 originalScale;
    private Vector3 maxScale;
    private float time;
    private bool hovered;

    // Use this for initialization
    void Start () {
        originalScale = transform.localScale;
        maxScale = originalScale + originalScale * curve.Evaluate(curve.length);
        time = 0;
        hovered = false;
    }
	

	// Update is called once per frame
	void Update () {
 
        if (hovered && time < curve.length)
        {
            time = Mathf.Min(curve.length, time + Time.deltaTime);
            Vector3 newScale = new Vector3(transform.localScale.x, transform.localScale.y);
            newScale.x = originalScale.x + originalScale.x * curve.Evaluate(time);
            transform.localScale = newScale;
            //Debug.Log("scale up " + newScale.x + " base value " + maxScale.x + " plus " + originalScale.x * curve.Evaluate(time));
        }
        else if(!hovered)
        {
            time = Mathf.Min(curve.length, time + Time.deltaTime);
            Vector3 newScale = new Vector3(transform.localScale.x, transform.localScale.y);
            newScale.x = maxScale.x - originalScale.x * curve.Evaluate(time);
            transform.localScale = newScale;
            //Debug.Log("scale down " + newScale.x + " base value " + maxScale.x + " minus " + originalScale.x * curve.Evaluate(time));
        }


    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        time = 0;
        hovered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hovered = false;
        time = 0;
    }

}
