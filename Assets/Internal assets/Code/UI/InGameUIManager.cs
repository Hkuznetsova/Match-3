using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class InGameUIManager : MonoBehaviour
{
    [SerializeField] RectTransform pausePanel;

    [SerializeField] Button restartButton;
    [SerializeField] Button inPauseExitButton;
    [SerializeField] Button resumeButton;
    [SerializeField] Button resumeBackgroundButton;
    [SerializeField] Button pauseButton;


    private bool pauseGameFlag;
    private int score;

    private void Awake()
    {
        inPauseExitButton.onClick.AddListener(OnExitButtonClick);
        resumeButton.onClick.AddListener(OnResumeButtonClick);
        pauseButton.onClick.AddListener(OnPauseButtonClick);
        resumeBackgroundButton.onClick.AddListener(OnResumeButtonClick);
        //restartButton.onClick.AddListener(OnRestartButtonClick);

        pausePanel.gameObject.SetActive(false);

        pauseGameFlag = false;
    }

    private void OnRestartButtonClick()
    {
        SceneManager.LoadScene("GameScene"); 
    }

    private void OnPauseButtonClick()
    {
        if (!pauseGameFlag)
        {
            pausePanel.gameObject.SetActive(true);
            Time.timeScale = 0;
            pauseGameFlag = !pauseGameFlag;
        }
        
    }

    private void OnResumeButtonClick()
    {
        if (pauseGameFlag)
        {
            pausePanel.gameObject.SetActive(false);
            Time.timeScale = 1;
            pauseGameFlag = !pauseGameFlag;
        }
    }

    private void OnExitButtonClick()
    {
        SceneManager.LoadScene("StartScene");
    }
}