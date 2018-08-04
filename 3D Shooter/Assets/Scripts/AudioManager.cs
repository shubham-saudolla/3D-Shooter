/*
Copyright (c) Shubham Saudolla
https://github.com/shubham-saudolla
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public enum AudioChannel { Master, Sfx, Music };

    public float masterVolPercent { get; private set; }
    public float sfxVolPercent { get; private set; }
    public float musicVolPercent { get; private set; }

    private AudioSource sfx2DSource;
    private AudioSource[] musicSources;
    private int activeMusicSourceIndex;

    public static AudioManager instance;

    private Transform audioListener;
    private Transform playerTransform;

    private SoundLibrary library;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;

            library = GetComponent<SoundLibrary>();
            DontDestroyOnLoad(gameObject);

            musicSources = new AudioSource[2];

            for (int i = 0; i < 2; i++)
            {
                GameObject newMusicSource = new GameObject("Music source " + (i + 1));
                // AddComponent returns the component that it just added
                musicSources[i] = newMusicSource.AddComponent<AudioSource>();
                newMusicSource.transform.parent = this.transform;
            }

            GameObject newSfx2Dsource = new GameObject("2D sfx source");
            // AddComponent returns the component that it just added
            sfx2DSource = newSfx2Dsource.AddComponent<AudioSource>();
            newSfx2Dsource.transform.parent = this.transform;

            audioListener = FindObjectOfType<AudioListener>().transform;

            if (FindObjectOfType<Player>() != null)
            {
                playerTransform = FindObjectOfType<Player>().transform;
            }

            masterVolPercent = PlayerPrefs.GetFloat("master vol", 1);
            sfxVolPercent = PlayerPrefs.GetFloat("sfx vol", 1);
            musicVolPercent = PlayerPrefs.GetFloat("music vol", 1);
        }
    }

    void Update()
    {
        if (playerTransform != null)
        {
            audioListener.position = playerTransform.position;
        }
    }

    public void SetVolume(float volPercent, AudioChannel channel)
    {
        switch (channel)
        {
            case AudioChannel.Master:
                masterVolPercent = volPercent;
                break;
            case AudioChannel.Sfx:
                sfxVolPercent = volPercent;
                break;
            case AudioChannel.Music:
                musicVolPercent = volPercent;
                break;
        }

        // updating the volume of the music sources so that changes reflect in them
        musicSources[0].volume = musicVolPercent * masterVolPercent;
        musicSources[1].volume = musicVolPercent * masterVolPercent;

        PlayerPrefs.SetFloat("master vol", masterVolPercent);
        PlayerPrefs.SetFloat("sfx vol", sfxVolPercent);
        PlayerPrefs.SetFloat("music vol", musicVolPercent);
        PlayerPrefs.Save();
    }

    public void PlayMusic(AudioClip clip, float fadeDuration = 1)
    {
        // we can use this because the size of the source array is 2, will go 0, 1, 0 ,1........
        activeMusicSourceIndex = 1 - activeMusicSourceIndex;
        musicSources[activeMusicSourceIndex].clip = clip;
        musicSources[activeMusicSourceIndex].Play();

        StartCoroutine(AnimateMusicCrossfade(fadeDuration));
    }

    public void PlaySound(string soundName, Vector3 pos)
    {
        PlaySound(library.GetClipFromName(soundName), pos);
    }

    public void PlaySound(AudioClip clip, Vector3 pos)
    {
        if (clip != null)
        {
            // handy for short sounds
            AudioSource.PlayClipAtPoint(clip, pos, masterVolPercent * sfxVolPercent);
        }
    }

    public void PlaySound2D(string soundName)               // to play sounds in 2D
    {
        sfx2DSource.PlayOneShot(library.GetClipFromName(soundName), sfxVolPercent * masterVolPercent);
    }

    IEnumerator AnimateMusicCrossfade(float duration)
    {
        float percent = 0;

        while (percent < 1)
        {
            percent += Time.deltaTime * 1 / duration;
            musicSources[activeMusicSourceIndex].volume = Mathf.Lerp(0, musicVolPercent * masterVolPercent, percent);
            musicSources[1 - activeMusicSourceIndex].volume = Mathf.Lerp(musicVolPercent * masterVolPercent, 0, percent);

            yield return null;                  // wait for one frame
        }
    }
}
