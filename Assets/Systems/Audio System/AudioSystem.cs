using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSystem : GameObjectPool
{
    public AudioSystem Instance;
    private Dictionary<Type, List<AudioSource>> audios = new Dictionary<Type, List<AudioSource>>();
    public Dictionary<Type, List<AudioSource>> Audios
    { get { return audios; } }

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
    public void PlayAudio(AudioData audio, Vector3? location)
    {
        // Checks if the type of audio exists inside the dictionary 
        if (!audios.ContainsKey(audio.GetType()))
        {
            audios.Add(audio.GetType(), new List<AudioSource>());
        }

        GameObject obj = GetObject(); // Gets an object with an attached audio source
        obj.transform.position = location ?? Vector3.zero;

        AudioSource source = obj.GetComponent<AudioSource>(); // Gets a reference to the audio source from the object

        source.clip = audio.Clip;
        source.volume = audio.Volume;
        source.pitch = audio.Pitch;
        source.loop = audio.IsLooping;

        if(audio is SFXAudioData audio3D)
        {
            source.spatialize = true; 
            source.minDistance = audio3D.MinDistance;
            source.maxDistance = audio3D.MaxDistance;
        }

        audios[GetType()].Add(source);
    }
    public void AdjustMasterVolume()
    {
        
    }
    public void AdjustVolumeByType(Type audioType)
    {
        foreach()
    }
}
public abstract class AudioData
{
    public AudioClip Clip;
    public float Volume;
    public float Pitch;
    public bool IsLooping;

    public AudioData(AudioClip clip, float volume = 1f, float pitch = 1f, bool isLooping = false)
    {
        Clip = clip;
        Volume = Mathf.Clamp01(volume);
        Pitch = Mathf.Clamp(pitch, -3f, 3f);
        IsLooping = isLooping;
    }
}

public class BGMAudioData : AudioData
{ 
    public BGMAudioData(AudioClip clip, float volume = 1f, float pitch = 1f, bool isLooping = false) : base(clip, volume, pitch, isLooping)
    {

    }
}
public class SFXAudioData : AudioData
{
    public bool Spatialize;
    public float MinDistance; 
    public float MaxDistance;

    public SFXAudioData(AudioClip clip, float volume = 1f, float pitch = 1f, bool isLooping = false, bool spatialize = true, float minDistance = 1f, float maxDistance = 500f) : base (clip, volume, pitch, isLooping)
    {
        Spatialize = spatialize;
        MinDistance = minDistance;
        MaxDistance = maxDistance;
    }
}
public class UIAudioData : AudioData
{
    public UIAudioData(AudioClip clip, float volume = 1f, float pitch = 1f, bool isLooping = false) : base(clip, volume, pitch, isLooping)
    {

    }
}
public class DialogueAudioData : AudioData
{
    public DialogueAudioData(AudioClip clip, float volume = 1f, float pitch = 1f, bool isLooping = false) : base(clip, volume, pitch, isLooping)
    {

    }
}