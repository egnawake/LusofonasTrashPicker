using System;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuScreen : MonoBehaviour
{
    public event Action<string> GameStarted;
    public event Action HighScoresOpened;
    public event Action Quit;

    [SerializeField] private Button startHumanGameButton;
    [SerializeField] private Button startBayesGameButton;
    [SerializeField] private Button startGeneticGameButton;
    [SerializeField] private Button openHighScoresButton;
    [SerializeField] private Button quitButton;

    private void Start()
    {
        startHumanGameButton.onClick.AddListener(
            () => GameStarted.Invoke("human"));
        startBayesGameButton.onClick.AddListener(
            () => GameStarted.Invoke("bayes"));
        startGeneticGameButton.onClick.AddListener(
            () => GameStarted.Invoke("genetic"));
        openHighScoresButton.onClick.AddListener(
            () => HighScoresOpened.Invoke());
        quitButton.onClick.AddListener(() => Quit.Invoke());
    }
}
