using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManageDialog : MonoBehaviour {

    public LoadSoundsDialog loader;
    public Image panel;
    public Text interactionText;
    public Text convText;
    private int tickPosition;

    ConversationData currConversation;
    int currConversationLine;

    public bool InteractionState
    {
        get { return interactionText.enabled; }
        set { interactionText.enabled = value; }
    }

    // Use this for initialization
    void Start () {
        currConversationLine = 0;
    }

    public void StartDialog(ConversationData currConversation)
    {
        panel.enabled = true;
        InteractionState = false;
        this.currConversation = currConversation;
        convText.text = currConversation.GetConversation(currConversationLine);
    }

    public bool NextLineConversation()
    {
        bool finishedDialog = false;
        currConversationLine++;
        if (currConversationLine < currConversation.GetSizeConversation())
        {
            convText.text = currConversation.GetConversation(currConversationLine);
        }
        else
        {
            StopConversation();
            finishedDialog = true;
        }
        return finishedDialog;
    }

    public void StopConversation()
    {
        panel.enabled = false;
        convText.text = "";
        currConversationLine = 0;
    }

    // Update is called once per frame
    void Update () {
		
        //once conversation starts
        //parse line on convText.text in words
        //time counter
        //depending on voice speed of character move the tick letter by letter
        //add a small pause at spaces, commas and dots
        

	}
}
