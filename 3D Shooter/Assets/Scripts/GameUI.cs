/*
Copyright (c) Shubham Saudolla
https://github.com/shubham-saudolla
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    public Image fadePlane;
    public float fadeTime = 1f;
    public GameObject gameOverUI;
    public RectTransform newWaveBanner;
    public Text newWaveTitle;
    public Text newWaveEnemyCount;
    public float bannerSpeed = 2f;

    private Spawner spawner;

    void Start()
    {
        FindObjectOfType<Player>().OnDeath += OnGameOver;
    }

    void Awake()
    {
        spawner = FindObjectOfType<Spawner>();
        spawner.OnNewWave += OnNewWave;
    }

    void OnNewWave(int waveNumber)
    {
        string[] numbers = { "One", "Two", "Three", "Four", "Five" };
        newWaveTitle.text = "Wave " + numbers[waveNumber - 1];
        string enemyCountString = ((spawner.waves[waveNumber - 1].infinite) ? "Infifinte" : spawner.waves[waveNumber - 1].enemyCount + "");
        newWaveEnemyCount.text = "Enemies: " + enemyCountString;

        StopCoroutine("AnimateNewWaveBanner");      // to improve animation while in developer mode
        StartCoroutine("AnimateNewWaveBanner");
    }

    IEnumerator AnimateNewWaveBanner()
    {
        float animatePercent = 0;
        float delayTime = 1f;
        float endDelayTime = Time.time + 1 / bannerSpeed + delayTime;
        int dir = 1;

        while (animatePercent >= 0)
        {
            animatePercent += Time.deltaTime * bannerSpeed * dir;

            if (animatePercent >= 1)
            {
                animatePercent = 1;

                if (Time.time > endDelayTime)
                {
                    dir = -1;
                }
            }

            newWaveBanner.anchoredPosition = Vector2.up * Mathf.Lerp(-242, -26, animatePercent);
            yield return null;
        }
    }

    void OnGameOver()
    {
        StartCoroutine(Fade(Color.clear, Color.black, fadeTime));
        gameOverUI.SetActive(true);
        Cursor.visible = true;                  // curson is visible only on the gameover UI
    }

    public IEnumerator Fade(Color from, Color to, float time)
    {
        float speed = 1 / time;
        float percent = 0;

        while (percent < 1)
        {
            percent += Time.deltaTime * speed;
            fadePlane.color = Color.Lerp(from, to, percent);
            yield return null;
        }
    }

    // UI input
    public void StartNewGame()
    {
        Cursor.visible = false;
        SceneManager.LoadScene("Main");
    }
}
