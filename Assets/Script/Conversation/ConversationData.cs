using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConversationData : MonoBehaviour {

    public AudioSource audio;
    public SOConversation conv;
    string[] parsedConversation;

    private void Awake()
    {
        audio = GetComponent<AudioSource>();
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

    public void PlayVoice(AudioClip clip)
    {
        audio.PlayOneShot(clip, 1f);
    }

    public void PlayVoice(AudioClip clip, float speed)
    {
        audio.PlayOneShot(clip, speed);
    }

}
