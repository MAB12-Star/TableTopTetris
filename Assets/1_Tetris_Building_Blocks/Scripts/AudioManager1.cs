using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager1 : MonoBehaviour
{
    public static AudioManager1 Instance;

    public Sound[] musicSounds;
    public Sound[] sfxSounds;
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public Grid1 Grid;
    private string currentSceneName;

    private int currentSongIndex = 0; // Keep track of the current song index
    private float musicTime = 0f; // Keep track of the current time of the music

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
        currentSceneName = SceneManager.GetActiveScene().name;

        // Play music based on the initial scene
        if (currentSceneName == "1-Scene")
        {
            PlayMusic("Theme1");
        }
    }

    private void Update()
    {
        // Check if the scene has changed
        if (currentSceneName != SceneManager.GetActiveScene().name)
        {
            currentSceneName = SceneManager.GetActiveScene().name;

            // Play music based on the scene
            if (currentSceneName == "1-Scene")
            {
                PlayMusic("Theme1");
            }
        }
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
        musicSource.time = musicTime; // Resume from the saved time
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

    public void StopMusic()
    {
        // Stops the music currently playing and save the current time
        if (musicSource.isPlaying)
        {
            musicTime = musicSource.time;
            musicSource.Stop();
        }
    }

    public void PlayNextSong()
    {
        // Increment the current song index and loop back if necessary
        currentSongIndex = (currentSongIndex + 1) % musicSounds.Length;

        // Reset the music time for the next song
        musicTime = 0f;

        // Play the next song in the list
        PlayMusic(musicSounds[currentSongIndex].name);
    }

    public void Play()
    {
        // Resume playing music from where it left off
        if (!musicSource.isPlaying && musicSource.clip != null)
        {
            musicSource.Play();
        }
    }

    public void SetVolume(float volume)
    {
        musicSource.volume = volume;
    }
}
