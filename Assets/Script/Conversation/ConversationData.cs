using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConversationData : MonoBehaviour {

    public AudioSource audio;
    public SOConversation conv;
    public int timesTalked = 0;
    public int TimesTalked { get { return timesTalked; } set { timesTalked = value; } }

    private void Awake()
    {
        audio = GetComponent<AudioSource>();
    }

    public string GetConversation(int group)
    {
        return conv.text[group];
    }

    public GameObject[] GetGifts()
    {
        return timesTalked == 0 ? conv.gifts : null;
    }

    public string GetConversation(int group, int line)
    {
        return conv.text[group].Split('\n')[line];
    }

    public int GetSizeConversation(int group)
    {
        return conv.text[group].Split('\n').Length;
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
