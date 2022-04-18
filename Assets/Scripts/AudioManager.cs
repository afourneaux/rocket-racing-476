using UnityEngine.Audio;
using UnityEngine;
using System; 

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    public Sound[] sounds;
    public AudioMixer audioMixer;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
            
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.outputAudioMixerGroup = s.output;
        }
    }

    private void Start()
    {
        Play("Theme");
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        s.source.Play();
    }

    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("musicVolume", volume);
    }

    public void SetSoundEffectVolume(float volume)
    {
        audioMixer.SetFloat("soundEffectVolume", volume);
    }    

}
