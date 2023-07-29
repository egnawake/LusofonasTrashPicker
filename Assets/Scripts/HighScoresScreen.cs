using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HighScoresScreen : MonoBehaviour
{
    public event Action BackPressed;

    public IEnumerable<HighScore> HighScores { get; set; }

    [SerializeField] private Button backButton;

    private IList<HighScoreEntry> scoreEntries;

    public void UpdateScores()
    {
        int i = 0;
        foreach (HighScore score in HighScores.Take(scoreEntries.Count))
        {
            scoreEntries[i].Score = score.Score;
            scoreEntries[i].AIScore = score.PlayedByAI;
            i++;
        }
    }

    private void Start()
    {
        backButton.onClick.AddListener(() => BackPressed.Invoke());

        scoreEntries = GetComponentsInChildren<HighScoreEntry>();

        foreach (HighScoreEntry entry in scoreEntries)
        {
            entry.Clear();
        }

        UpdateScores();
    }
}
