using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConversationData : MonoBehaviour {

    public AudioSource audio;
    public SOConversation conv;
    private int timesTalked = 0;
    public int TimesTalked { get { return timesTalked; } set { timesTalked = Mathf.Min(value, conv.text.Length-1); } }

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
        return conv.gifts;
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
