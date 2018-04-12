using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadSoundsDialog : MonoBehaviour {

    private const int alphSize = 25;

    private Object[] sounds;
    private AudioClip[] dialogSounds;

    // Use this for initialization
    void Start () {
        sounds = Resources.LoadAll("SoundsFX/Conversation/Audio", typeof(AudioClip));
        dialogSounds = new AudioClip[alphSize];
    }
	
	public AudioClip[] GetDialogSounds(int offset)
    {
        int j = 0;
        for (int i = offset; i < offset + alphSize; i++)
        {
            dialogSounds[j] = (AudioClip)sounds[i % alphSize];
            j++;
        }

        return dialogSounds;
    }


}
