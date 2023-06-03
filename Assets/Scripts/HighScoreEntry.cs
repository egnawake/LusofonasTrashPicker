using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Object for an entry in the high score list.
/// </summary>
public class HighScoreEntry : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private Sprite humanIcon;
    [SerializeField] private Sprite brainIcon;

    /// <summary>
    /// Sets whether this is a human or an AI score.
    /// </summary>
    public bool AIScore
    {
        set
        {
            iconImage.enabled = true;
            iconImage.sprite = value ? brainIcon : humanIcon;
        }
    }

    /// <summary>
    /// Sets a score value.
    /// </summary>
    public int Score
    {
        set => scoreText.text = value.ToString();
    }

    /// <summary>
    /// Clears the entry.
    /// </summary>
    public void Clear()
    {
        iconImage.enabled = false;
        scoreText.text = "";
    }
}
