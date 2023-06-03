using UnityEngine;
using TMPro;

/// <summary>
/// Trash Picker heads-up display. Shows score and turn progress.
/// </summary>
public class HUD : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text turnText;

    /// <summary>
    /// Show a new score.
    /// </summary>
    ///
    /// <param name="value">
    /// The new score value.
    /// </param>
    public void ShowScore(int value)
    {
        scoreText.text = value.ToString();
    }

    /// <summary>
    /// Show played turns and max turns.
    /// </summary>
    ///
    /// <param name="turn">
    /// The current turn.
    /// </param>
    /// <param name="maxTurns">
    /// The maximum number of turns.
    /// </param>
    public void ShowTurn(int turn, int maxTurns)
    {
        turnText.text = $"{turn}/{maxTurns}";
    }
}
