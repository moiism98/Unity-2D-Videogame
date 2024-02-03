using UnityEngine;

[System.Serializable]
public class Sound
{
    private AudioSource source;
    [SerializeField] private AudioClip clip;
    [SerializeField] private string clipName;
    [SerializeField] [Range(0f, 2f)] private float volume = 1f;
    [SerializeField] [Range(.1f, 3f)] private float pitch = 1f;
    [SerializeField] private bool loop;

    public void SetSource(AudioSource source)
    {
        this.source = source;
    }

    public void SetVolume(float volume)
    {
        this.volume = volume;
    }

    public AudioSource GetSource()
    {
        return this.source;
    }

    public string GetClipName()
    {
        return this.clipName;
    }

    public AudioClip GetClip()
    {
        return this.clip;
    }

    public float GetVolume()
    {
        return this.volume;
    }

    public float GetPitch()
    {
        return this.pitch;
    }

    public bool GetLoop()
    {
        return this.loop;
    }
}
