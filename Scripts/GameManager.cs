using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    public List<int> scoreTables;
    
    private int score;
    private int level;
    private int layerCleared;
    private float fallSpeed = 1f;
    private bool gameOver;
    
    
    private void Awake()
    {
        Instance = this;
    }

    public float GetFallSpeed => fallSpeed;

    public void SetScore(int amount)
    {
        score += amount;
        PlayerPrefs.SetInt("score", score);
        UpdateLevel(1000 * level);
    }

    public void LayersCleared(int amount)
    {
        SetScore(scoreTables[amount]);
        layerCleared += amount;
        PlayerPrefs.SetInt("layers", layerCleared);
    }

    void UpdateLevel(int condition)
    {
        // condition update level
        if (score >= condition)
        {
            level++;
            fallSpeed = 1f;
            //PlayerPrefs.SetFloat("fallSpeed", fallSpeed);
        }
    }

    public bool GetGameOver => gameOver;

    public void SetGameIsOver()
    {
        gameOver = true;
        UIManager.Instance.SetActiveGameOverWindow(true);
    }
}
