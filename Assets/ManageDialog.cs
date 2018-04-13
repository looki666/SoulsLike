using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManageDialog : MonoBehaviour {

    public LoadSoundsDialog loader;
    public Image panel;
    public Text interactionText;
    public Text convText;
    private string[] currParsedText;
    private int tickPosition;

    ConversationData currConversation;
    int currConversationLine;
    float timer;

    public bool InteractionState
    {
        get { return interactionText.enabled; }
        set { interactionText.enabled = value; }
    }

    // Use this for initialization
    void Start () {
        tickPosition = 0;
        currConversationLine = 0;
        timer = 0f;
    }

    public void StartDialog(ConversationData currConversation)
    {
        timer = 0f;
        tickPosition = 0;
        panel.enabled = true;
        InteractionState = false;
        this.currConversation = currConversation;
        convText.text = currConversation.GetConversation(currConversationLine);
        currParsedText = convText.text.Split(' ');
    }

    public bool NextLineConversation()
    {
        bool finishedDialog = false;
        currConversationLine++;
        if (currConversationLine < currConversation.GetSizeConversation())
        {
            convText.text = currConversation.GetConversation(currConversationLine);
            timer = 0f;
            tickPosition = 0;
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
        if (panel.enabled)
        {
            timer += Time.deltaTime;
            //depending on voice speed of character move the tick letter by letter
            if (timer >= currConversation.conv.voiceSpeed)
            {
                tickPosition++;
                timer = 0f;
            }

            //add a small pause at spaces, commas and dots

        }



    }
}
