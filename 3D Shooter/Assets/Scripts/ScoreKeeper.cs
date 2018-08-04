/*
Copyright (c) Shubham Saudolla
https://github.com/shubham-saudolla
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreKeeper : MonoBehaviour
{
    public static int score { get; private set; }               // other classes can access the variable but cannot alter it
    private float lastEnemyKillTime;
    private int streakCount;
    private float streakExpiryTime = 1f;

    void Start()
    {
        Enemy.OnDeathStatic += OnEnemyKilled;
        FindObjectOfType<Player>().OnDeath += OnPlayerDeath;
    }

    void OnEnemyKilled()
    {
        if (Time.time < lastEnemyKillTime + streakExpiryTime)
        {
            streakCount++;
        }
        else
        {
            streakCount = 0;
        }

        lastEnemyKillTime = Time.time;

        score += 5 + (int)Mathf.Pow(2, streakCount);
    }

    void OnPlayerDeath()
    {
        // unsubscribing from the event so that we don't score twice by killing one enemy after the player dies and respawns
        Enemy.OnDeathStatic -= OnEnemyKilled;
    }
}
