using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Conversation")]
public class SOConversation : ScriptableObject
{
    [TextArea(15, 20)]
    public string text;
    string[] conversationOptions;
}
