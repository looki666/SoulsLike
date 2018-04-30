using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleConversation : MonoBehaviour {

    public ManageDialog dialog;

    private bool occupied;
    Rigidbody rb;
    Collider[] nearbyNpcs;
    ConversationData currConversation;
    bool inConversation;
    bool pressedInteract;
    CharacterBodyCostumization body;

    private void Awake()
    {
        inConversation = false;
        rb = GetComponent<Rigidbody>();
        nearbyNpcs = new Collider[1];
        pressedInteract = false;
        body = GetComponent<CharacterBodyCostumization>();
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
        int nNpcsNearby = Physics.OverlapSphereNonAlloc(rb.position, 5f, nearbyNpcs, 1 << 11);

        //If close but not in a conversation
        if (nNpcsNearby == 1 && !inConversation)
        {
            //Start conversation
            if (pressedInteract)
            {
                pressedInteract = false;
                StartInteraction();
            } else
            {
                //Display interaction message
                dialog.InteractionState = true;
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
                    pressedInteract = false;
                    NextInteraction();
                }

                //moving away disables conversation
                if (nNpcsNearby < 1)
                {
                    StopInteraction();
                }
            }

            //If far away and have prompt to interact, disable
            if (nNpcsNearby < 1 && dialog.InteractionState)
            {
                dialog.InteractionState = false; ;
            }
        }
    }

    private void StartInteraction()
    {
        currConversation = nearbyNpcs[0].GetComponent<ConversationData>();
        dialog.StartDialog(currConversation);
        inConversation = true;
    }

    private void NextInteraction()
    {
        bool finished = dialog.NextLineConversation();
        if (finished)
        {
            StopInteraction();
        }
    }

    private void StopInteraction()
    {
        currConversation.TimesTalked++;
        dialog.StopConversation();
        inConversation = false;

        GameObject[] gifts = currConversation.GetGifts();
        if (gifts!= null && gifts.Length > 0)
        {
            body.AddItemInventory(currConversation.GetGifts());
        }

    }

}
