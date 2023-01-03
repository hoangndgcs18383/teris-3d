using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] private Button speedUpBtn;
    [SerializeField] private Button skill01;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text layersText;

    [SerializeField] private GameObject GameOverWindow;
    [SerializeField] private Image fillImgAnim;
    [SerializeField] private TMP_Text coundownText;
    [SerializeField] private Image fillcoundownImg;
    
    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        speedUpBtn.onClick.AddListener(OnSpeedUp);
        skill01.onClick.AddListener(SetCountdown);
    }

    private void OnDisable()
    {
        speedUpBtn.onClick.RemoveListener(OnSpeedUp);
    }

    private int score;
    
    public void UpdateGameLevel()
    {
        score = PlayerPrefs.GetInt("score");
        SetScoreText(score);
        SetLayersClearedText(PlayerPrefs.GetInt("layers"));
        UpdateLevelText(PlayerPrefs.GetInt("level"));
    }

    public void SetScoreText(int score)
    {
        scoreText.text = $"Score: {score}";
    }

    public void SetLayersClearedText(int layer)
    {
        layersText.text = $"Blocks Cleared: {layer}";
    }

    public void UpdateLevelText(int level)
    {
        levelText.text = $"Level: {level}";
    }

    public void OnSpeedUp()
    {
        UIButton.Instance.OnHighSpeed();
    }

    public void SetCountdown()
    {
        Playground.Instance.UsingSkill(0);
        StartCoroutine(IEDoFill());
    }

    public IEnumerator IEDoFill()
    {
        int counter = 0;
        while (counter >= 1)
        {
            counter += 1/60;
            fillcoundownImg.fillAmount = Mathf.Clamp01(counter);
            
            yield return new WaitForSeconds(1f);
        }
    }



    public void SetActiveGameOverWindow(bool enable)
    {
        GameOverWindow.SetActive(enable);
    }
}
