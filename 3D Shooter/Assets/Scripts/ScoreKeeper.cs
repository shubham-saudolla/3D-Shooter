/*
Copyright (c) Shubham Saudolla
https://github.com/shubham-saudolla
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreKeeper : MonoBehaviour
{
    public static int score { get; private set; }       // only access it, not modify
    private float lastEnmyKilledTime;
    private float streakExpiryTime = 1f;
    private int streakCount;

    void Awake()
    {
        score = 0;
    }

    void Start()
    {
        Enemy.OnDeathStatic += OnEnemyKilled;
        FindObjectOfType<Player>().OnDeath += OnPLayerDeath;
    }

    void OnEnemyKilled()
    {
        if (Time.time > lastEnmyKilledTime + streakExpiryTime)
        {
            streakCount++;
        }
        else
        {
            streakCount = 0;
        }

        lastEnmyKilledTime = Time.time;

        score += 5 + (int)Mathf.Pow(2, streakCount);
    }

    void OnPLayerDeath()
    {
        Enemy.OnDeathStatic -= OnEnemyKilled;
    }
}
