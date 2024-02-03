using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Sound[] sounds;
    void Awake()
    {
        AddSoundClips();
    }

    private void AddSoundClips()
    {
        foreach(Sound sound in sounds)
        {
            sound.SetSource(gameObject.AddComponent<AudioSource>());

            AudioSource source = sound.GetSource();

            source.clip = sound.GetClip();

            source.volume = sound.GetVolume();

            source.pitch = sound.GetPitch();

            source.loop = sound.GetLoop();
        }
    }

    public void PlaySound(string clipName)
    {
        Sound sound = Array.Find(sounds, sound => sound.GetClipName().Equals(clipName));

        if(sound != null)
            sound.GetSource().Play();
        else
            Debug.LogWarning("Clip " + clipName + " not found!");
    }

    public void StopSound(string clipName)
    {
        Sound sound = Array.Find(sounds, sound => sound.GetClipName().Equals(clipName));

        if(sound != null)
            sound.GetSource().Stop();
        else
            Debug.LogWarning("Clip " + clipName + " not found!");
    }
}
