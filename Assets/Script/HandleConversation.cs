using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandleConversation : MonoBehaviour {

    Rigidbody rb;
    Collider[] nearbyNpcs;
    public Image panel;
    public Text interactionText;
    public Text convText;
    bool inConversation;
    bool pressedInteract;
    ConversationData currConversation;
    int currConversationLine;


    private void Awake()
    {
        currConversationLine = 0;
        inConversation = false;
        rb = GetComponent<Rigidbody>();
        nearbyNpcs = new Collider[1];
        interactionText.enabled = false;
        pressedInteract = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            pressedInteract = true;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        int nNpcsNearby = Physics.OverlapSphereNonAlloc(rb.position, 2f, nearbyNpcs, 1 << 11);

        //If close but not in a conversation
        if (nNpcsNearby == 1 && !inConversation)
        {
            //Start conversation
            if (pressedInteract)
            {
                StartConversation();
            } else
            {
                interactionText.enabled = true;
            }

        }
        else
        {
            //If in a conversation
            if (inConversation)
            {
                //F press
                if (pressedInteract)
                {
                    NextLineConversation();
                }

                //moving away disables conversation
                if (nNpcsNearby < 1)
                {
                    StopConversation();
                }
            }

            //If far away and have prompt to interact, disable
            if (nNpcsNearby < 1 && interactionText.enabled)
            {
                interactionText.enabled = false;
            }
        }
    }

    void StartConversation()
    {
        panel.enabled = true;
        inConversation = true;
        interactionText.enabled = false;
        currConversation = nearbyNpcs[0].GetComponent<ConversationData>();
        convText.text = currConversation.GetConversation(currConversationLine);
        pressedInteract = false;
    }

    void NextLineConversation()
    {
        currConversationLine++;
        if(currConversationLine < currConversation.GetSizeConversation())
        {
            convText.text = currConversation.GetConversation(currConversationLine);
        } else
        {
            StopConversation();
        }
        pressedInteract = false;
    }

    void StopConversation()
    {
        panel.enabled = false;
        inConversation = false;
        convText.text = "";
        pressedInteract = false;
        currConversationLine = 0;
    }


}
