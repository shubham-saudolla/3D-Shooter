/*
Copyright (c) Shubham Saudolla
https://github.com/shubham-saudolla
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Range(0f, 1f)]
    public float masterVolPercent = 1f;
    [Range(0f, 1f)]
    public float sfxVolpercent = 1f;
    [Range(0f, 1f)]
    public float musicVolPercent = 1;

    private AudioSource[] musicSources;
    private int activeMusicSourceIndex;

    public static AudioManager instance;

    private Transform audioListener;
    private Transform playerTransform;

    void Awake()
    {
        instance = this;
        musicSources = new AudioSource[2];

        for (int i = 0; i < 2; i++)
        {
            GameObject newMusicSource = new GameObject("Music source " + (i + 1));
            // AddComponent returns the component that it just added
            musicSources[i] = newMusicSource.AddComponent<AudioSource>();
            newMusicSource.transform.parent = this.transform;
        }

        audioListener = FindObjectOfType<AudioListener>().transform;
        playerTransform = FindObjectOfType<Player>().transform;
    }

    void Update()
    {
        if (playerTransform != null)
        {
            audioListener.position = playerTransform.position;
        }
    }

    public void PlayMusic(AudioClip clip, float fadeDuration = 1)
    {
        // we can use this because the size of the source array is 2, will go 0, 1, 0 ,1........
        activeMusicSourceIndex = 1 - activeMusicSourceIndex;
        musicSources[activeMusicSourceIndex].clip = clip;
        musicSources[activeMusicSourceIndex].Play();

        StartCoroutine(AnimateMusicCrossfade(fadeDuration));
    }

    public void PlaySound(AudioClip clip, Vector3 pos)
    {
        if (clip != null)
        {
            // handy for short sounds
            AudioSource.PlayClipAtPoint(clip, pos, masterVolPercent * sfxVolpercent);
        }
    }

    IEnumerator AnimateMusicCrossfade(float duration)
    {
        float percent = 0;

        while (percent < 1)
        {
            percent += Time.deltaTime * 1 / duration;
            musicSources[activeMusicSourceIndex].volume = Mathf.Lerp(0, musicVolPercent * masterVolPercent, percent);
            musicSources[1 - activeMusicSourceIndex].volume = Mathf.Lerp(musicVolPercent * masterVolPercent, 0, percent);

            yield return null;					// wait for one frame
        }
    }
}
