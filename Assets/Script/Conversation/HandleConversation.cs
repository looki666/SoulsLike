using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleConversation : MonoBehaviour {

    Rigidbody rb;
    Collider[] nearbyNpcs;
    public ManageDialog dialog;
    bool inConversation;
    bool pressedInteract;


    private void Awake()
    {
        inConversation = false;
        rb = GetComponent<Rigidbody>();
        nearbyNpcs = new Collider[1];
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
        int nNpcsNearby = Physics.OverlapSphereNonAlloc(rb.position, 5f, nearbyNpcs, 1 << 11);

        //If close but not in a conversation
        if (nNpcsNearby == 1 && !inConversation)
        {
            //Start conversation
            if (pressedInteract)
            {
                StartInteraction();
            } else
            {
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
        ConversationData currConversation = nearbyNpcs[0].GetComponent<ConversationData>();
        dialog.StartDialog(currConversation);
        pressedInteract = false;
        inConversation = true;
    }

    private void NextInteraction()
    {
        bool finished = dialog.NextLineConversation();
        pressedInteract = false;
        if (finished)
        {
            inConversation = false;
            pressedInteract = false;
        }
    }

    private void StopInteraction()
    {
        dialog.StopConversation();
        inConversation = false;
        pressedInteract = false;
    }

}
