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
       /* else if (currentSceneName == "2-Scene")
        {
            PlayMusic("Theme3");
        }*/
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
            else if (currentSceneName == "2-Scene")
            {
                PlayMusic("Theme3");
            }
        }

        // Check if the current level is 2
        if (Grid.currentLevel == 2)
        {
            PlayMusic("Theme4");
        }
        else if (Grid.currentLevel == 3)
        {
            PlayMusic("Theme5");
        }
        else if (Grid.currentLevel == 4)
        {
            PlayMusic("Theme5");
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
        // Stops the music currently playing
        if (musicSource.isPlaying)
        {
            musicSource.Stop();
        }
    }
}
