using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManageDialog : MonoBehaviour {

    public LoadSoundsDialog loader;
    public Image panel;
    public Text interactionText;
    public Text convText;
    private string currLine;
    private string[] currParsedText;
    private AudioClip[] allSoundsDialog;

    ConversationData currConversation;

    public int tickPosition;
    public int currConversationLine;
    public bool conversationAlive;

    float timer;

    public bool InteractionState
    {
        get { return interactionText.enabled; }
        set { interactionText.enabled = value; }
    }

    // Use this for initialization
    void Start () {
        conversationAlive = false;
        ResetTick();
        currConversationLine = 0;
    }

    public void StartDialog(ConversationData currConversation)
    {
        conversationAlive = true;

        //Reset tick for dialog and voice
        ResetTick();

        //UI prepare
        panel.enabled = true;
        InteractionState = false;
        
        //Prepare character
        this.currConversation = currConversation;
        HandleCurrentDialogLine();

        //Retrieve sounds of current character
        allSoundsDialog = loader.GetDialogSounds(currConversation.conv.voiceOffset);
    }

    public bool NextLineConversation(bool autoFinish)
    {
        bool finishedDialog = false;

        currConversationLine++;
        //If its not the last line move text to following line
        if (currConversationLine < currConversation.GetSizeConversation())
        {
            HandleCurrentDialogLine();
            ResetTick();
        }
        else
        {
            currConversationLine = 0;
            finishedDialog = true;
            if (autoFinish)
            {
                StopConversation();
            }
        }
        return finishedDialog;
    }

    public void StopConversation()
    {
        convText.text = "";
        panel.enabled = false;
        currConversationLine = 0;
    }

    private void ResetTick()
    {
        timer = 0f;
        tickPosition = 0;
    }

    private void HandleCurrentDialogLine()
    {
        currLine = currConversation.GetConversation(currConversationLine);
        currParsedText = currLine.Split(' ');
    }

    // Update is called once per frame
    void Update () {

        //once conversation starts
        if (conversationAlive)
        {
            timer += Time.deltaTime;
            //depending on voice speed of character move the tick letter by letter
            if (timer >= currConversation.conv.voiceSpeed)
            {
                if (tickPosition == 0)
                {
                    convText.text = "";
                }
                if (tickPosition >= currLine.Length)
                {
                    if (NextLineConversation(false)){
                        conversationAlive = false;
                        return;
                    }
                    else 
                    {
                        timer = -1f;
                        tickPosition = 0;
                        return;
                    }
                }

                char letter = currLine[tickPosition];
                convText.text += letter;

                tickPosition++;
                timer = 0f;

                int numberAlphabet = char.ToUpper(letter) - 64;
                if (numberAlphabet < 0 || numberAlphabet > 24)
                {
                    return;
                }

                if (tickPosition % 4 == 0)
                {
                    currConversation.PlayVoice(allSoundsDialog[numberAlphabet]);
                }

            }



        }



    }
}
