using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public event Action<string> GameStarted;

    public IEnumerable<HighScore> HighScores { get; set; }

    [SerializeField] private MainMenuScreen mainMenuPrefab;
    [SerializeField] private HighScoresScreen highScoresPrefab;

    private MainMenuScreen mainMenuScreen;
    private HighScoresScreen highScoresScreen;

    public void Open()
    {
        mainMenuScreen.gameObject.SetActive(true);
    }

    private void Start()
    {
        mainMenuScreen = Instantiate(mainMenuPrefab);
        mainMenuScreen.GameStarted += StartGame;
        mainMenuScreen.HighScoresOpened += OpenHighScores;
        mainMenuScreen.Quit += Quit;
    }

    private void OpenHighScores()
    {
        if (highScoresScreen == null)
        {
            highScoresScreen = Instantiate(highScoresPrefab);
            highScoresScreen.BackPressed += CloseHighScores;
            highScoresScreen.HighScores = HighScores;
        }
        else
        {
            highScoresScreen.gameObject.SetActive(true);
            highScoresScreen.UpdateScores();
        }

        mainMenuScreen.gameObject.SetActive(false);
    }

    private void CloseHighScores()
    {
        highScoresScreen.gameObject.SetActive(false);
        mainMenuScreen.gameObject.SetActive(true);
    }

    private void StartGame(string playerType)
    {
        mainMenuScreen.gameObject.SetActive(false);
        GameStarted.Invoke(playerType);
    }

    private void Quit()
    {
#if UNITY_STANDALONE
        Application.Quit();
#endif
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
