using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager1 : MonoBehaviour
{
    public static AudioManager1 Instance;

    public Sound[] musicSounds;
    public Sound[] sfxSounds;
    public AudioSource musicSource;
    public AudioSource sfxSource;

    private void Awake()
    {
        // Ensure only one instance of AudioManager1 exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Call PlayMusic method after setting up AudioManager1 instance
        PlayMusic("Theme1");
    }

    public void PlayMusic(string name)
    {
        Sound s = Array.Find(musicSounds, x => x.name == name);

        // Check if the sound is found
        if (s == null)
        {
            Debug.Log("Music Not Found: " + name);
            return;
        }

        // Play the music
        musicSource.clip = s.clip;
        musicSource.Play();
    }

    public void PlaySfx(string name)
    {
        Sound s = Array.Find(sfxSounds, x => x.name == name);

        // Check if the sound is found
        if (s == null)
        {
            Debug.Log("SFX Not Found: " + name);
            return;
        }

        // Play the sound effect
        sfxSource.PlayOneShot(s.clip);
    }
}
