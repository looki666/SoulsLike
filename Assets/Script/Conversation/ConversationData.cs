using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConversationData : MonoBehaviour {

    public SOConversation conv;
    string[] parsedConversation;

    private void Awake()
    {
        parsedConversation = conv.text.Split('\n');
    }

    public string GetConversation()
    {
        return conv.text;
    }

    public string GetConversation(int line)
    {
        return parsedConversation[line];
    }

    public int GetSizeConversation()
    {
        return parsedConversation.Length;
    }

}
